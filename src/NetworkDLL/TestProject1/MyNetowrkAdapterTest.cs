using Moq;
using NetworkDLL;
using NetworkDLL.File;
using NetworkDLL.Message;
using NetworkDLL.Network;
using NUnit.Framework;
using System;
using System.IO;

namespace TestProject1
{
    class MyNetowrkAdapterTest
    {
        [SetUp]
        public void Setup()
        {            
        }


        [Test]
        public void TestWriteGotFileResponseIntoNetworkStream()
        {
            var fileStreamMock = new Mock<IMyFileStream>();
            fileStreamMock.Setup(m => m.Length).Returns(100L);

            var fileUtilMock = new Mock<IFileUtil>();
            fileUtilMock.Setup(m => m.CreateFileStream(It.IsAny<string>(), It.IsAny<FileMode>()))
                .Returns(fileStreamMock.Object);

            IMessage sentMsg = null;

            var networkConnectionMock = new Mock<INetworkConnection>();
            networkConnectionMock.Setup(m => m.WriteMessage(It.IsAny<IMessage>()))
                .Callback((IMessage msg) => { sentMsg = msg; });

            MyNetworkAdapter adapter = new MyNetworkAdapter();

            adapter.WriteGotFileResponseIntoNetworkStream("abc", fileUtilMock.Object, networkConnectionMock.Object);

            Assert.AreEqual(MessageTypeEnum.GOT_FILE_RESPONSE, sentMsg.MessageType);
            Assert.AreEqual("abc", ((GotFileResponse)sentMsg).FileName);
            Assert.AreEqual(100L, ((GotFileResponse)sentMsg).FileLength);
        }

        [Test]
        public void TestWriteFileNotFoundIntoNetworkStream()
        {
            MyNetworkAdapter adapter = new MyNetworkAdapter();

            IMessage sentMsg = null;

            var networkConnectionMock = new Mock<INetworkConnection>();
            networkConnectionMock.Setup(m => m.WriteMessage(It.IsAny<IMessage>()))
                .Callback((IMessage msg) => { sentMsg = msg; });

            adapter.WriteFileNotFoundResponseIntoNetworkStream("abc", networkConnectionMock.Object);

            Assert.AreEqual(MessageTypeEnum.BAD_REQUEST_RESPONSE, sentMsg.MessageType);
            Assert.AreEqual("abc", ((BadRequestResponse)sentMsg).FileName);
            Assert.AreEqual(RequestErrorCode.FILE_NOT_FOUND, ((BadRequestResponse)sentMsg).ErrorCode);            
        }

        [Test]
        public void TestWriteGetFileRequestIntoNetworkStream()
        {
            MyNetworkAdapter adapter = new MyNetworkAdapter();

            IMessage sentMsg = null;

            var networkConnectionMock = new Mock<INetworkConnection>();
            networkConnectionMock.Setup(m => m.WriteMessage(It.IsAny<IMessage>()))
                .Callback((IMessage msg) => { sentMsg = msg; });

            adapter.WriteGetFileRequestIntoNetworkStream("abc", networkConnectionMock.Object);

            Assert.AreEqual(MessageTypeEnum.GET_FILE_REQUEST, sentMsg.MessageType);
            Assert.AreEqual("abc", ((GetFileRequest)sentMsg).FileName);
        }

        [Test]
        public void TestWriteGotFileResponseWithNameAndSizeIntoNetworkStream()
        {
            MyNetworkAdapter adapter = new MyNetworkAdapter();

            IMessage sentMsg = null;

            var networkConnectionMock = new Mock<INetworkConnection>();
            networkConnectionMock.Setup(m => m.WriteMessage(It.IsAny<IMessage>()))
                .Callback((IMessage msg) => { sentMsg = msg; });

            adapter.WriteGotFileResponseIntoNetworkStream("abc", 200L, networkConnectionMock.Object);

            Assert.AreEqual(MessageTypeEnum.GOT_FILE_RESPONSE, sentMsg.MessageType);
            Assert.AreEqual("abc", ((GotFileResponse)sentMsg).FileName);
            Assert.AreEqual(200L, ((GotFileResponse)sentMsg).FileLength);
        }

        [Test]
        public void LoadFileIntoNetwork()
        {
            var connMock = new Mock<INetworkConnection>();
            var fileUtilMock = new Mock<IFileUtil>();
            var fileStreamMock = new Mock<IMyFileStream>();
            IMyNetworkAdapter adapter = new MyNetworkAdapter();

            fileUtilMock.Setup(m => m.CreateFileStream(It.IsAny<string>(), It.IsAny<FileMode>()))
                .Returns(fileStreamMock.Object);

            int byteLen = 0;

            int fileBytesLength = Consts.BUFFER_SIZE * 3 + 1;
            byte[] fileBytes = new byte[fileBytesLength];

            byte val = 0;

            for(int i=0;i< fileBytesLength; i++)
            {
                fileBytes[i] = val++;
            }

            fileStreamMock.Setup(m => m.Length).Returns(() => fileBytesLength);

            using (MemoryStream filems = new MemoryStream(fileBytes))
            using (BinaryReader br = new BinaryReader(filems))
            using (MemoryStream networkms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(networkms))
            {
                fileStreamMock.Setup(m => m.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Callback<byte[], int, int>((buffer, offset, size) =>
                    {
                        byte[] bytes = br.ReadBytes(size);
                        Array.Copy(bytes, 0, buffer, offset, bytes.Length);
                        
                        byteLen = bytes.Length;
                    })
                    .Returns(() => byteLen);

                connMock.Setup(m => m.Write(It.IsAny<byte[]>()))
                                    .Callback<byte[]>((data) => bw.Write(data));
                connMock.Setup(m => m.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                                    .Callback<byte[], int, int>((data, index, count) => bw.Write(data, index, count));


                adapter.LoadFileIntoNetworkStream("abc", fileUtilMock.Object, connMock.Object);

                fileUtilMock.Verify(m => m.CreateFileStream(It.IsAny<string>(), It.IsAny<FileMode>()), Times.Once());
                fileStreamMock.Verify(m => m.Length, Times.Once);

                byte[] rawData = networkms.ToArray();

                Assert.AreEqual(fileBytesLength, rawData.Length);

                CollectionAssert.AreEquivalent(fileBytes, rawData);
            }            
        }

        [Test]
        public void SaveFileFromNetworkStream()
        {
            var connMock = new Mock<INetworkConnection>();
            var fileUtilMock = new Mock<IFileUtil>();
            var fileStreamMock = new Mock<IMyFileStream>();
            IMyNetworkAdapter adapter = new MyNetworkAdapter();

            fileUtilMock.Setup(m => m.CreateFileStream(It.IsAny<string>(), It.IsAny<FileMode>()))
                .Returns(fileStreamMock.Object);

            int fileBytesLength = Consts.BUFFER_SIZE * 3 + 1;
            byte[] fileBytes = new byte[fileBytesLength];

            byte val = 0;

            for (int i = 0; i < fileBytesLength; i++)
            {
                fileBytes[i] = val++;
            }

            fileStreamMock.Setup(m => m.Length).Returns(() => fileBytesLength);

            using (MemoryStream filems = new MemoryStream())
            using (MemoryStream networkms = new MemoryStream(fileBytes))
            using (BinaryReader br = new BinaryReader(networkms))
            using (BinaryWriter bw = new BinaryWriter(filems))
            {
                fileStreamMock.Setup(m => m.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Callback<byte[], int, int>((buffer, offset, count) =>
                    {
                        bw.Write(buffer, offset, count);
                    });

                int byteLen = 0;

                connMock.Setup(m => m.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Callback<byte[], int, int>((buffer, offset, size) =>
                    {
                        byte[] bytes = br.ReadBytes(size);
                        Array.Copy(bytes, 0, buffer, offset, bytes.Length);

                        byteLen = bytes.Length;
                    })
                    .Returns(() => byteLen);               

                adapter.SaveFileFromNetworkStream("abc", fileBytesLength, fileUtilMock.Object, connMock.Object);

                fileUtilMock.Verify(m => m.CreateFileStream(It.IsAny<string>(), It.IsAny<FileMode>()), Times.Once());

                byte[] rawData = filems.ToArray();

                Assert.AreEqual(fileBytesLength, rawData.Length);

                CollectionAssert.AreEquivalent(fileBytes, rawData);
            }
        }

        [Test]
        public void TestWriteFileCannotReadIntoNetworkStream()
        {
            MyNetworkAdapter adapter = new MyNetworkAdapter();

            IMessage sentMsg = null;

            var networkConnectionMock = new Mock<INetworkConnection>();
            networkConnectionMock.Setup(m => m.WriteMessage(It.IsAny<IMessage>()))
                .Callback((IMessage msg) => { sentMsg = msg; });

            adapter.WriteFileCannotReadResponseIntoNetworkStream("abc", networkConnectionMock.Object);

            Assert.AreEqual(MessageTypeEnum.BAD_REQUEST_RESPONSE, sentMsg.MessageType);
            Assert.AreEqual("abc", ((BadRequestResponse)sentMsg).FileName);
            Assert.AreEqual(RequestErrorCode.CANNOT_READ, ((BadRequestResponse)sentMsg).ErrorCode);
        }
    }
}
