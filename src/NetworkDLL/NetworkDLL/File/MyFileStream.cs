using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkDLL.File
{
    class MyFileStream : IMyFileStream
    {
        private FileStream stream;

        public long Length => stream.Length;

        public MyFileStream(FileStream fs)
        {
            if (fs == null) throw new ArgumentNullException("fs");
            this.stream = fs;
        }

        public void Write(byte[] array, int offset, int count)
        {
            this.stream.Write(array, offset, count);
        }

        public void Close()
        {
            this.stream.Close();
        }

        public int Read(byte[] array, int offset, int count)
        {
            return stream.Read(array, offset, count);
        }
    }
}
