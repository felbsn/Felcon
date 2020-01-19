using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Felcon.Extensions
{
    public static class StringExtensions
    {
        public static byte[]  ToLengthValuePair(this string str)
        {
            var bValue = Encoding.ASCII.GetBytes(str);
            var bValueLen = BitConverter.GetBytes(bValue.Length);
            return bValueLen.Concat(bValue).ToArray();
        }
    }
}
