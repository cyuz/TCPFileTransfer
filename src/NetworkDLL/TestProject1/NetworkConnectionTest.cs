using Moq;
using NetworkDLL;
using NetworkDLL.Message;
using NetworkDLL.Network;
using NUnit.Framework;
using System;
using System.IO;

namespace TestProject1
{
    class NetworkConnectionTest
    {
        [Test]
        public void TestWriteMessage()
        {
            var clientMock = new Mock<INetworkClient>();
            var networkStreamMock = new Mock<IMyNetworkStream>();
            NetworkConnection conn = new NetworkConnection(clientMock.Object, networkStreamMock.Object);

            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                networkStreamMock.Setup(m => m.Write(It.IsAny<byte[]>()))
                    .Callback<byte[]>((data) => bw.Write(data));

                GetFileRequest oldMessage = new GetFileRequest
                {
                    FileName = "abc"
                };


                conn.WriteMessage(oldMessage);

                byte[] rawData = ms.ToArray();

                UInt32 len = BitConverter.ToUInt32(rawData, 0);

                Assert.AreEqual(len + sizeof(UInt32), rawData.Length);

                byte[] payLoad = new byte[len];
                Array.Copy(rawData, sizeof(UInt32), payLoad, 0, len);

                IMessage message = Serializer.Desirialize(payLoad);

                Assert.AreEqual(MessageTypeEnum.GET_FILE_REQUEST, message.MessageType);
                Assert.AreEqual(oldMessage.FileName, ((GetFileRequest)message).FileName);
            }
        }

        [Test]
        public void TestReadMessage()
        {
            var clientMock = new Mock<INetworkClient>();
            var networkStreamMock = new Mock<IMyNetworkStream>();
            NetworkConnection conn = new NetworkConnection(clientMock.Object, networkStreamMock.Object);

            GetFileRequest request = new GetFileRequest { FileName = "abc" };

            byte[] payLoad = Serializer.Serialize(request);

            byte[] rawDataLen = BitConverter.GetBytes((UInt32)payLoad.Length);

            byte[] data = new byte[payLoad.Length + sizeof(UInt32)];

            Array.Copy(rawDataLen, data, sizeof(UInt32));
            Array.Copy(payLoad, 0, data, sizeof(UInt32), payLoad.Length);

            using (MemoryStream ms = new MemoryStream(data))
            using (BinaryReader br = new BinaryReader(ms))
            {
                int byteLen = 0;

                networkStreamMock.Setup(m => m.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Callback<byte[], int, int>((buffer, offset, size) =>
                    {
                        byte[] bytes = br.ReadBytes(size);
                        Array.Copy(bytes, 0, buffer, offset, bytes.Length);

                        byteLen = bytes.Length;
                    })
                    .Returns(() => byteLen);

                IMessage message = conn.ReadMessage();

                Assert.AreEqual(MessageTypeEnum.GET_FILE_REQUEST, message.MessageType);
                Assert.AreEqual(request.FileName, ((GetFileRequest)message).FileName);
            }
        }

        [Test]
        public void TestReadMessageEarlyClose()
        {
            var clientMock = new Mock<INetworkClient>();
            var networkStreamMock = new Mock<IMyNetworkStream>();
            NetworkConnection conn = new NetworkConnection(clientMock.Object, networkStreamMock.Object);

            GetFileRequest request = new GetFileRequest { FileName = "abc" };

            byte[] payLoad = Serializer.Serialize(request);

            byte[] rawDataLen = BitConverter.GetBytes((UInt32)payLoad.Length);

            byte[] data = new byte[payLoad.Length - 1 + sizeof(UInt32)];

            Array.Copy(rawDataLen, data, sizeof(UInt32));
            Array.Copy(payLoad, 0, data, sizeof(UInt32), payLoad.Length - 1);

            using (MemoryStream ms = new MemoryStream(data))
            using (BinaryReader br = new BinaryReader(ms))
            {
                int byteLen = 0;

                networkStreamMock.Setup(m => m.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Callback<byte[], int, int>((buffer, offset, size) =>
                    {
                        byte[] bytes = br.ReadBytes(size);
                        Array.Copy(bytes, 0, buffer, offset, bytes.Length);

                        byteLen = bytes.Length;
                    })
                    .Returns(() => byteLen);

                IMessage message = conn.ReadMessage();

                Assert.IsNull(message);
            }
        }

        [Test]
        public void TestDisconnect()
        {
            var clientMock = new Mock<INetworkClient>();
            var networkStreamMock = new Mock<IMyNetworkStream>();
            NetworkConnection conn = new NetworkConnection(clientMock.Object, networkStreamMock.Object);

            conn.Disconnect();

            clientMock.Verify(m => m.Disconnect());
        }

        [Test]
        public void TestProp()
        {
            var clientMock = new Mock<INetworkClient>();
            var networkStreamMock = new Mock<IMyNetworkStream>();
            networkStreamMock.Setup(m => m.IP).Returns("127.0.0.1");
            networkStreamMock.Setup(m => m.Port).Returns(12345);
            NetworkConnection conn = new NetworkConnection(clientMock.Object, networkStreamMock.Object);

            Assert.AreEqual("127.0.0.1", conn.IP);
            Assert.AreEqual(12345, conn.Port);
        }
    }
}
