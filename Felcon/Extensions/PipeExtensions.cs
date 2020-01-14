using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Felcon.Core
{
    public static class PipeExtensions
    {
        public static Int32 ReadI32(this PipeStream pipeStream)
        {
            var buffer = new byte[4];
            int len = pipeStream.Read(buffer, 0, 4);
            if (len != 4)
                throw new Exception("Int32 read exception ");
            return BitConverter.ToInt32(buffer, 0);
        }
        public static string ReadString(this PipeStream pipeStream, int count)
        {
            var buffer = new byte[count];
            int len = pipeStream.Read(buffer, 0, count);
            if (len != count)
                throw new Exception("string read exception");

            return Encoding.ASCII.GetString(buffer);
        }
        public static byte[] ReadBytes(this PipeStream pipeStream , int count)
        {
            var buffer = new byte[count];
            int len = pipeStream.Read(buffer, 0, count);
            if (len != count)
                throw new Exception("byte[] read exception");

            return buffer;
        }
    }
}
