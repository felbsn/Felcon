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
        public static byte[] MergeBytes(params byte[][] arrays)
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
        /// <param name="value"></param> this will be value of the registry key
        public static void RegisterRegistry( string regPath ,string regKey,string value)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(regPath, true);
            if (key == null)
                key = Registry.CurrentUser.CreateSubKey(regPath, true);
            if (key.GetValue(regKey, "").ToString() != value)
            {
                key.SetValue(regKey, value);
            }
        }

        public static string GetRegistryValue(string regPath, string regKey)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(regPath, true);
            if (key != null)
            {
                return (string)key.GetValue(regKey, null);
            }
            return null;
        }

        public static string GetExecutablePath()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
        }

    }
}
