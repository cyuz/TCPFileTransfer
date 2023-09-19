using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkDLL.File
{
    public interface IMyFileStream
    {
        public void Write(byte[] array, int offset, int count);

        public int Read(byte[] array, int offset, int count);

        public void Close();

        public long Length { get; }
    }
}
