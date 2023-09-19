using NetworkDLL.File;
using NetworkDLL.Message;
using System.IO;

namespace NetworkDLL.Network
{
    public class MyNetworkAdapter : IMyNetworkAdapter
    {
        public MyNetworkAdapter()
        {
        }

        public void WriteGotFileResponseIntoNetworkStream(string filePath, IFileUtil fileUtil, INetworkConnection conn)
        {
            IMyFileStream fs = fileUtil.CreateFileStream(filePath, FileMode.Open);

            string fileName = Path.GetFileName(filePath);

            IMessage response = new GotFileResponse
            {
                FileName = fileName,
                FileLength = fs.Length
            };

            fs.Close();

            conn.WriteMessage(response);
        }

        public void LoadFileIntoNetworkStream(string filePath, IFileUtil fileUtil, INetworkConnection conn)
        {
            IMyFileStream fs = fileUtil.CreateFileStream(filePath, FileMode.Open);

            long bufferCount = (fs.Length + (Consts.BUFFER_SIZE - 1)) / Consts.BUFFER_SIZE;

            byte[] buffer = new byte[Consts.BUFFER_SIZE];

            for (int i = 0; i < bufferCount; i++)
            {
                
                int size = fs.Read(buffer, 0, Consts.BUFFER_SIZE);

                conn.Write(buffer, 0, size);
            }

            conn.Flush();
            fs.Close();
        }

        public bool SaveFileFromNetworkStream(string filePath, long fileLength, IFileUtil fileUtil, INetworkConnection conn)
        {
            IMyFileStream fs = fileUtil.CreateFileStream(filePath, FileMode.OpenOrCreate);

            while (fileLength > 0)
            {
                byte[] buffer = new byte[Consts.BUFFER_SIZE];

                int size = conn.Read(buffer, 0, Consts.BUFFER_SIZE);

                if (size == 0)
                {
                    fs.Close();
                    fileUtil.Delete(filePath);
                    return false;
                }

                fs.Write(buffer, 0, size);

                fileLength -= size;
            }


            fs.Close();
            return true;
        }

        public void WriteFileNotFoundResponseIntoNetworkStream(string fileName, INetworkConnection conn)
        {
            IMessage response = new BadRequestResponse
            {
                ErrorCode = RequestErrorCode.FILE_NOT_FOUND,
                FileName = fileName
            };

            conn.WriteMessage(response);
        }

        public void WriteGetFileRequestIntoNetworkStream(string fileName, INetworkConnection conn)
        {
            IMessage request = new GetFileRequest
            {
                FileName = fileName
            };

            conn.WriteMessage(request);
        }

        public void WriteGotFileResponseIntoNetworkStream(string fileName, long fileSize, INetworkConnection conn)
        {
            IMessage response = new GotFileResponse
            {
                FileName = fileName,
                FileLength = fileSize
            };

            conn.WriteMessage(response);
        }

        public void WriteFileCannotReadResponseIntoNetworkStream(string fileName, INetworkConnection conn)
        {
            IMessage response = new BadRequestResponse
            {
                ErrorCode = RequestErrorCode.CANNOT_READ,
                FileName = fileName
            };

            conn.WriteMessage(response);
        }
    }
}
