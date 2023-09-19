using NetworkDLL.File;

namespace NetworkDLL.Network
{
    public interface IMyNetworkAdapter
    {

        public void WriteGotFileResponseIntoNetworkStream(string filePath, IFileUtil fileUtil, INetworkConnection conn);

        public bool SaveFileFromNetworkStream(string filePath, long fileLength, IFileUtil fileUtil, INetworkConnection conn);

        public void LoadFileIntoNetworkStream(string filePath, IFileUtil fileUtil, INetworkConnection conn);

        public void WriteFileNotFoundResponseIntoNetworkStream(string fileName, INetworkConnection conn);

        public void WriteFileCannotReadResponseIntoNetworkStream(string fileName, INetworkConnection conn);

        public void WriteGetFileRequestIntoNetworkStream(string fileName, INetworkConnection conn);

        public void WriteGotFileResponseIntoNetworkStream(string fileName, long fileSize, INetworkConnection conn);
    }
}
