using System;

namespace NetworkDLL.Network
{
    public static class IMyNetworkStreamExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientStream"></param>
        /// <returns>If the packet cannot read complete, will drop it and reutrn null</returns>
        public static byte[] ReadPacket(this IMyNetworkStream clientStream)
        {
            byte[] buffer = new byte[Consts.BUFFER_SIZE];

            int bytesRead = clientStream.Read(buffer, 0, sizeof(UInt32));

            if (bytesRead == 0)
            {
                return null;
            }

            int bytesToRead = BitConverter.ToInt32(buffer, 0);
            int offset = 0;

            byte[] frame = new byte[bytesToRead];

            while (bytesToRead > 0)
            {
                var n = clientStream.Read(frame, offset, bytesToRead);
                if(n == 0)
                {
                    break;
                }

                offset += n;
                bytesToRead -= n;
            }

            if (bytesToRead > 0)
            {
                return null;
            }

            return frame;
        }

        //public static IMessage ReadMessage(this IMyNetworkStream clientStream)
        //{
        //    byte[] data = clientStream.ReadPacket();

        //    if (data == null)
        //    {
        //        return null;
        //    }

        //    IMessage message = Serializer.Desirialize(data);

        //    return message;
        //}

        //public static void WriteMessage(this IMyNetworkStream clientStream, IMessage message)
        //{
        //    byte[] data = Serializer.Serialize(message);

        //    byte[] lengthData = BitConverter.GetBytes((UInt32)data.Length);

        //    clientStream.Write(lengthData);
        //    clientStream.Write(data);
        //    clientStream.Flush();
        //}
    }
}
