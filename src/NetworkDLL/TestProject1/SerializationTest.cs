using NetworkDLL;
using NetworkDLL.Message;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace TestProject1
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestMessageHeaderSerialization()
        {

            MessageHeader header = new MessageHeader
            {
                MessageType = 2,
                MessageSize = 3,
                CRC = 0,
            };

            byte[] data = header.Serialize();

            Assert.AreEqual(MessageHeader.FixedPartSize, data.Length);

            MessageHeader header2 = new MessageHeader();

            using(MemoryStream ms = new MemoryStream(data))
            {
                int pos = header2.Deserialize(ms);
                Assert.AreEqual(MessageHeader.FixedPartSize, pos);
            }

            Assert.AreEqual(header.MessageType, header2.MessageType);
            Assert.AreEqual(header.CRC, header2.CRC);
            Assert.AreEqual(header.MessageSize, header2.MessageSize);
        }

        [Test]
        public void TestGetFileRequestSerialization()
        {
            GetFileRequest msg = new GetFileRequest
            {
                FileName = "Long Long text............................",
            };

            byte[] rawData = UTF8Encoding.UTF8.GetBytes(msg.FileName);

            byte[] data = msg.Serialize();

            Assert.AreEqual(rawData.Length + GetFileRequest.FixedPartSize, data.Length);

            GetFileRequest msg2 = new GetFileRequest();

            using (MemoryStream ms = new MemoryStream(data))
            {
                int pos = msg2.Deserialize(ms, 0);
                Assert.AreEqual(rawData.Length + GetFileRequest.FixedPartSize, pos);
            }

            Assert.AreEqual(msg.FileName, msg2.FileName);
        }

        [Test]
        public void TestGotFileResponseSerialization()
        {
            GotFileResponse msg = new GotFileResponse
            {
                FileName = "Long Long text............................",
                FileLength = 12345678L
            };

            byte[] rawData = UTF8Encoding.UTF8.GetBytes(msg.FileName);

            byte[] data = msg.Serialize();

            Assert.AreEqual(rawData.Length + GotFileResponse.FixedPartSize, data.Length);

            GotFileResponse msg2 = new GotFileResponse();

            using (MemoryStream ms = new MemoryStream(data))
            {
                int pos = msg2.Deserialize(ms, 0);
                Assert.AreEqual(rawData.Length + GotFileResponse.FixedPartSize, pos);
            }

            Assert.AreEqual(msg.FileName, msg2.FileName);
            Assert.AreEqual(msg.FileLength, msg2.FileLength);
        }

        [Test]
        public void TestBadRequsetResponseSerialization()
        {
            BadRequestResponse msg = new BadRequestResponse
            {
                ErrorCode = RequestErrorCode.FILE_NOT_FOUND,
                FileName = "Long Long text............................"
            };

            byte[] rawData = UTF8Encoding.UTF8.GetBytes(msg.FileName);

            byte[] data = msg.Serialize();

            Assert.AreEqual(rawData.Length + BadRequestResponse.FixedPartSize, data.Length);

            BadRequestResponse msg2 = new BadRequestResponse();

            using (MemoryStream ms = new MemoryStream(data))
            {
                int pos = msg2.Deserialize(ms, 0);
                Assert.AreEqual(rawData.Length + BadRequestResponse.FixedPartSize, pos);
            }

            Assert.AreEqual(msg.ErrorCode, msg2.ErrorCode);
            Assert.AreEqual(msg.FileName, msg2.FileName);
        }

        [Test]
        public void TestGetFileRequestSerializationWithSerializer()
        {
            GetFileRequest msg = new GetFileRequest
            {
                FileName = "Long Long text............................",
            };

            byte[] data = Serializer.Serialize(msg);

            IMessage msg2 = Serializer.Desirialize(data);

            Assert.True(msg2 is GetFileRequest);

            Assert.AreEqual(MessageTypeEnum.GET_FILE_REQUEST, msg2.MessageType);

            Assert.AreEqual(msg.FileName, ((GetFileRequest)msg2).FileName);
        }

        [Test]
        public void TestGotFileResponseSerializationWithSerializer()
        {
            GotFileResponse msg = new GotFileResponse
            {
                FileName = "Long Long text............................",
                FileLength = 12345678L
            };

            byte[] data = Serializer.Serialize(msg);

            IMessage msg2 = Serializer.Desirialize(data);

            Assert.True(msg2 is GotFileResponse);

            Assert.AreEqual(MessageTypeEnum.GOT_FILE_RESPONSE, msg2.MessageType);

            Assert.AreEqual(msg.FileLength, ((GotFileResponse)msg2).FileLength);
            Assert.AreEqual(msg.FileName, ((GotFileResponse)msg2).FileName);
        }

        [Test]
        public void TestBadRequsetResponseSerializationWithSerializer()
        {
            BadRequestResponse msg = new BadRequestResponse
            {
                ErrorCode = RequestErrorCode.FILE_NOT_FOUND,
                FileName = "Long Long text............................"
            };

            byte[] data = Serializer.Serialize(msg);

            IMessage msg2 = Serializer.Desirialize(data);

            Assert.True(msg2 is BadRequestResponse);

            Assert.AreEqual(MessageTypeEnum.BAD_REQUEST_RESPONSE, msg2.MessageType);

            Assert.AreEqual(msg.ErrorCode, ((BadRequestResponse)msg2).ErrorCode);
            Assert.AreEqual(msg.FileName, ((BadRequestResponse)msg2).FileName);
        }

        [Test]
        public void TestSerializeUnknownMessage()
        {
            GetFileRequest msg = new GetFileRequest
            {
                FileName = "Long Long text............................",
            };

            byte[] data = Serializer.Serialize(msg);

            byte[] wrongTypeBytes = BitConverter.GetBytes(((UInt32)MessageTypeEnum.END + 1));

            Array.Copy(wrongTypeBytes, data, sizeof(UInt32));

            Exception ex = Assert.Throws<ArgumentException>(() => Serializer.Desirialize(data));
        }
    }
}