using Moq;
using NetworkDLL.Network;
using NUnit.Framework;
using System;
using System.IO;

namespace TestProject1
{
    class MyNetworkStreamExtensionTest
    {
        [SetUp]
        public void Setup()
        {
        }

        //[Test]
        //public void TestWriteMessage()
        //{
        //    var networkStreamMock = new Mock<IMyNetworkStream>();

        //    using (MemoryStream ms = new MemoryStream())
        //    using (BinaryWriter bw = new BinaryWriter(ms))
        //    {
        //        networkStreamMock.Setup(m => m.Write(It.IsAny<byte[]>()))
        //            .Callback<byte[]>((data) => bw.Write(data));

        //        GetFileRequest oldMessage = new GetFileRequest
        //        {
        //            FileName = "abc"
        //        };


        //        networkStreamMock.Object.WriteMessage(oldMessage);

        //        byte[] rawData = ms.ToArray();

        //        UInt32 len = BitConverter.ToUInt32(rawData, 0);

        //        Assert.AreEqual(len + sizeof(UInt32), rawData.Length);

        //        byte[] payLoad = new byte[len];
        //        Array.Copy(rawData, sizeof(UInt32), payLoad, 0, len);

        //        IMessage message = Serializer.Desirialize(payLoad);

        //        Assert.AreEqual(MessageTypeEnum.GET_FILE_REQUEST, message.MessageType);
        //        Assert.AreEqual(oldMessage.FileName, ((GetFileRequest)message).FileName);
        //    }
        //}

        [Test]
        public void TestReadPacket()
        {
            var networkStreamMock = new Mock<IMyNetworkStream>();

            byte[] rawDataLen = BitConverter.GetBytes((UInt32)2);

            byte[] data = new byte[6];

            Array.Copy(rawDataLen, data, sizeof(UInt32));
            data[4] = 1;
            data[5] = 2;

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

                byte[] packet = networkStreamMock.Object.ReadPacket();

                Assert.AreEqual(2, packet.Length);
                Assert.AreEqual(1, packet[0]);
                Assert.AreEqual(2, packet[1]);
            }
        }

        [Test]
        public void TestReadPacketEarlyClose()
        {
            var networkStreamMock = new Mock<IMyNetworkStream>();

            byte[] rawDataLen = BitConverter.GetBytes((UInt32)4);

            byte[] data = new byte[6];

            Array.Copy(rawDataLen, data, sizeof(UInt32));
            data[4] = 1;
            data[5] = 2;

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

                byte[] packet = networkStreamMock.Object.ReadPacket();

                Assert.IsNull(packet);
            }
        }
    }
}
