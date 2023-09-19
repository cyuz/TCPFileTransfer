using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkDLL.File
{
    public class FileUtil : IFileUtil
    {
        public IMyFileStream CreateFileStream(string path, FileMode mode)
        {
            return new MyFileStream(new FileStream(path, mode));
        }

        public bool Exists(string filePath)
        {
            return System.IO.File.Exists(filePath);
        }

        public bool CanRead(string filePath)
        {
            try
            {
                using (FileStream fs = System.IO.File.Open(filePath, FileMode.Open))
                {
                    return fs.CanRead;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }

        public bool CanWrite(string filePath)
        {
            try
            {
                using (FileStream fs = System.IO.File.Open(filePath, FileMode.Open))
                {
                    return fs.CanWrite;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }

        public void Delete(string filePath)
        {
            System.IO.File.Delete(filePath);
        }
    }
}
