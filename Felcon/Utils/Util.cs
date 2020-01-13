using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Felcon.Utils
{
    public static class Util
    {
        // merging arrays has never been easier  ( -_-)
        public static byte[] Merge(params byte[][] arrays)
        {
            if(arrays.Length == 0)
            {
                return null;
            }

            var totalLength = 0;
            for (int i = 0; i < arrays.Length; i++)
            {
                totalLength += arrays[i].Length;
            }

            var arr = new byte[totalLength];
            var arrIndex = 0;
            for (int i = 0; i < arrays.Length; i++)
            {
                Array.Copy(arrays[i], 0, arr, arrIndex, arrays[i].Length);
                arrIndex += arrays[i].Length;
            }
            return arr;
        }


        /// <summary>
        ///  Register Registry (ofcourse minimal pain)
        /// </summary>
        /// <param name="regPath"></param> something like @"Software/{SomeProgram}
        /// <param name="regKey"></param>  ... just a key, 'executable_path' is the best imo
        /// <param name="executablePath"></param> this is the real executable file location
        public static void RegisterRegistry( string regPath ,string regKey,string executablePath)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(regPath, true);
            if (key == null)
                key = Registry.CurrentUser.CreateSubKey(regPath, true);
            if (key.GetValue(regKey, "null").ToString() != executablePath)
            {
                key.SetValue(regKey, executablePath);
            }
        }



        public static string GetExecutablePath()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
        }

    }
}
