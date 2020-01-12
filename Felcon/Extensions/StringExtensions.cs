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
            var bAction = Encoding.ASCII.GetBytes(str);
            var bActionLen = BitConverter.GetBytes(bAction.Length);
            return bActionLen.Concat(bAction).ToArray();
        }
    }
}
