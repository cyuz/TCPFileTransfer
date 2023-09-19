using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkDLL.File
{
    public interface IFileUtil
    {
        public bool Exists(string filePath);

        public bool CanRead(string filePath);

        public bool CanWrite(string filePath);

        public IMyFileStream CreateFileStream(string path, FileMode mode);

        public void Delete(string filePath);
    }
}
