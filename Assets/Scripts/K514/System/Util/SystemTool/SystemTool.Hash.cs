using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BlackAm
{
    public partial class SystemTool
    {
        public static string GetMD5HashCode(string[] p_StringSet)
        {
            using (var md5 = MD5.Create())
            {
                foreach (var str in p_StringSet)
                {
                    var contentBytes = Encoding.UTF8.GetBytes(str);
                    md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
                }

                //Handles empty filePaths case
                md5.TransformFinalBlock(new byte[0], 0, 0);

                return BitConverter.ToString(md5.Hash).Replace("-", "").ToLower();
            }
        }
        
        public static string GetMD5HashCode(List<string> p_StringSet)
        {
            using (var md5 = MD5.Create())
            {
                foreach (var str in p_StringSet)
                {
                    var contentBytes = Encoding.UTF8.GetBytes(str);
                    md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
                }

                //Handles empty filePaths case
                md5.TransformFinalBlock(new byte[0], 0, 0);

                return BitConverter.ToString(md5.Hash).Replace("-", "").ToLower();
            }
        }
    }
}