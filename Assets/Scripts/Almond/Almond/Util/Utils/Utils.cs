using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using UnityEngine;
using System.Security.Cryptography;
using System.Net;
using System.Runtime.InteropServices;
using ICSharpCode.SharpZipLib.Zip;
using Object = UnityEngine.Object;

namespace Almond.Util
{
    public static class Utils
    {
        private const char KEY_VALUE_SPRITER = ':';
        private const char MAP_SPRITER = ',';
        private const char LIST_SPRITER = '/';
        private const string privateKey = "";

        #region string parse
        public static object GetValue(string value, Type type)
        {
            if (type == null)
                return null;
            else if (type == typeof(string))
                return value;
            else if (type == typeof(int))
                return Convert.ToInt32(Convert.ToDouble(value));
            else if (type == typeof(float))
                return float.Parse(value);
            else if (type == typeof(byte))
                return Convert.ToByte(Convert.ToDouble(value));
            else if (type == typeof(sbyte))
                return Convert.ToSByte(Convert.ToDouble(value));
            else if (type == typeof(uint))
                return Convert.ToUInt32(Convert.ToDouble(value));
            else if (type == typeof(short))
                return Convert.ToInt16(Convert.ToDouble(value));
            else if (type == typeof(long))
                return Convert.ToInt64(Convert.ToDouble(value));
            else if (type == typeof(ushort))
                return Convert.ToUInt16(Convert.ToDouble(value));
            else if (type == typeof(ulong))
                return Convert.ToUInt64(Convert.ToDouble(value));
            else if (type == typeof(double))
                return double.Parse(value);
            else if (type == typeof(bool))
            {
                if (value == "0")
                    return false;
                else if (value == "1")
                    return true;
                else
                    return bool.Parse(value);
            }
            else if (type.BaseType == typeof(Enum))
            {
                // BaseType이 System.Enum이면 열거형 상수
                var enumOriginType = Enum.GetUnderlyingType(type);
                // enum을 숫자로 기술한 경우
                if(IsDigitsOnly(value))
                {
                    return Enum.Parse(type, GetValue(value, enumOriginType).ToString());
                }
                // enum을 문자로 기술한 경우                    
                else
                {
                    return Enum.Parse(type, value, true);
                }
            }
            else if (type == typeof(Vector3))
            {
                ParseVector3(value, out Vector3 result);
                return result;
            }
            else if(type == typeof(Vector3Int))
            {
                ParseVector3Int(value, out Vector3Int result);
                return result;
            }
            else if (type == typeof(Quaternion))
            {
                ParseQuaternion(value, out Quaternion result);
                return result;
            }
            else if (type == typeof(Color))
            {
                ParseColor(value, out Color result);
                return result;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                Type[] types = type.GetGenericArguments();
                var map = ParseMap(value);
                var result = type.GetConstructor(Type.EmptyTypes).Invoke(null);
                foreach (var item in map)
                {
                    var key = GetValue(item.Key, types[0]);
                    var v = GetValue(item.Value, types[1]);
                    type.GetMethod("Add").Invoke(result, new object[] { key, v });
                }
                return result;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type t = type.GetGenericArguments()[0];
                var list = ParseList(value);
                var result = type.GetConstructor(Type.EmptyTypes).Invoke(null);
                for (int i = 0; i < list.Count; i++)
                {
                    var v = GetValue(list[i], t);
                    type.GetMethod("Add").Invoke(result, new object[] { v });
                }
                return result;
            }
            else
                return null;
        }

        /// <summary>
        /// 특정 문자열이 반각 숫자 0 ~ 9 로만 구성되어 있는지 검증하는 메서드
        /// </summary>
        public static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }
        
        public static bool ParseVector3(string _inputString, out Vector3 result)
        {
            string trimString = _inputString.Trim();
            result = new Vector3();
            if (trimString.Length < 7)
            {
                return false;
            }
            try
            {
                string[] _detail = trimString.Split(MAP_SPRITER);
                if (_detail.Length != 3)
                {
                    return false;
                }
                result.x = float.Parse(_detail[0]);
                result.y = float.Parse(_detail[1]);
                result.z = float.Parse(_detail[2]);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("Parse Vector3 error: " + trimString + e.ToString());
                return false;
            }
        }
        public static bool ParseVector3Int(string _inputString, out Vector3Int result)
        {
            string trimString = _inputString.Trim();
            result = new Vector3Int();
            try
            {
                string[] _detail = _inputString.Split(MAP_SPRITER);
                if (_detail.Length != 3)
                {
                    return false;
                }
                result.x = int.Parse(_detail[0]);
                result.y = int.Parse(_detail[1]);
                result.z = int.Parse(_detail[2]);

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("Parse Vector3Int error: " + trimString + e.ToString());
                return false;
            }
        }
        public static bool ParseQuaternion(string _inputString, out Quaternion result)
        {
            string trimString = _inputString.Trim();
            result = new Quaternion();
            if (trimString.Length < 9)
            {
                return false;
            }
            try
            {
                string[] _detail = trimString.Split(MAP_SPRITER);
                if (_detail.Length != 4)
                {
                    return false;
                }
                result.x = float.Parse(_detail[0]);
                result.y = float.Parse(_detail[1]);
                result.z = float.Parse(_detail[2]);
                result.w = float.Parse(_detail[3]);
                return true;
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.LogError("Parse Quaternion error: " + trimString + e.ToString());
#endif
                return false;
            }
        }

        public static bool ParseColor(string _inputString, out Color result)
        {
            string trimString = _inputString.Trim();
            result = Color.clear;
            if (trimString.Length < 9)
            {
                return false;
            }
            try
            {
                string[] _detail = trimString.Split(LIST_SPRITER);
                if (_detail.Length != 4)
                {
                    return false;
                }
                result = new Color(float.Parse(_detail[0]) / 255, float.Parse(_detail[1]) / 255, float.Parse(_detail[2]) / 255, float.Parse(_detail[3]) / 255);
                return true;
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.LogError("Parse Color error: " + trimString + e.ToString());
#endif
                return false;
            }
        }

        public static Dictionary<string, string> ParseMap(this string strMap, char keyValueSpriter = KEY_VALUE_SPRITER, char mapSpriter = MAP_SPRITER)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(strMap))
            {
                return result;
            }
            if (strMap.Equals("0"))

                if (strMap == "0")
                    strMap = "0:0";
            var map = strMap.Split(mapSpriter);
            for (int i = 0; i < map.Length; i++)
            {
                if (string.IsNullOrEmpty(map[i]))
                {
                    continue;
                }
                var keyValuePair = map[i].Split(keyValueSpriter);
                if (keyValuePair.Length == 2)
                {
                    if (!result.ContainsKey(keyValuePair[0]))
                        result.Add(keyValuePair[0], keyValuePair[1]);
                    else
                        Debug.LogError(string.Format("Key {0} already exist, index {1} of {2}.", keyValuePair[0], i, strMap));
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogError(string.Format("KeyValuePair are not match: {0}, index {1} of {2}.", map[i], i, strMap));
#endif
                }
            }
            return result;
        }

        public static List<string> ParseList(this string strList, char listSpriter = LIST_SPRITER)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(strList))
                return result;

            var trimString = strList.Trim();
            if (string.IsNullOrEmpty(strList))
            {
                return result;
            }
            var detials = trimString.Split(listSpriter);
            int count = detials.Length;

            for (int i = 0; i < count; i++)
            {
                if (!string.IsNullOrEmpty(detials[i]))
                    result.Add(detials[i].Trim());
            }

            return result;
        }
        public static string ReplaceFirst(this string input, string oldValue, string newValue, int startAt = 0)
        {
            int pos = input.IndexOf(oldValue, startAt);
            if (pos < 0)
            {
                return input;
            }
            return string.Concat(input.Substring(0, pos), newValue, input.Substring(pos + oldValue.Length));
        }
        public static string DeleteToString(object data, params string[] deleteText)
        {
            string str = data.ToString();
            foreach(var text in deleteText)
            {
                str = str.Replace(text, "");
            }
            return str;
        }
        #endregion
        #region ResourceLoad
        public static string LoadResource(string fileName)
        {
            var text = Resources.Load(fileName);
            if (text != null)
            {
                var result = text.ToString();
                Resources.UnloadAsset(text);
                return result;
            }
            else
                return string.Empty;
        }

        public static string LoadFile(string fileName)
        {
            Debug.Log($" ************ File Name{ fileName }  *************** ");
            if (File.Exists(fileName))
            {
                Debug.Log($" ************ File Name Exist  *************** ");
                using (StreamReader sr = File.OpenText(fileName))
                    return sr.ReadToEnd();
            }
            else
            {
                Debug.Log($" ************ File Name Not Exist  *************** ");
                return string.Empty;
            }
        }

        public static byte[] LoadByteResource(string fileName)
        {
            TextAsset binAsset = Resources.Load(fileName, typeof(TextAsset)) as TextAsset;
            if (binAsset == null) return null;

            var result = binAsset.bytes;
            Resources.UnloadAsset(binAsset);


            return result;
        }

        public static byte[] LoadByteFile(string fileName)
        {
            if (File.Exists(fileName))
                return File.ReadAllBytes(fileName);
            else
                return null;
        }
        #endregion

        #region 파일 경로 처리
        public static string GetFileNameWithoutExtention(string fileName, char separator = '/')
        {
            var name = GetFileName(fileName, separator);
            return GetFilePathWithoutExtention(name);
        }

        public static string GetFilePathWithoutExtention(string fileName)
        {
            return fileName.Substring(0, fileName.LastIndexOf('.'));
        }

        public static string GetDirectoryName(string fileName)
        {
            return fileName.Substring(0, fileName.LastIndexOf('/'));
        }

        public static string GetFileName(string path, char separator = '/')
        {
            return path.Substring(path.LastIndexOf(separator) + 1);
        }
        public static string GetFilePath(string path, string outterFolder = "")
        {
            path = path.Replace("\\", "/");
            return path.Replace(outterFolder, "");
        }
        public static string PathNormalize(this string str)
        {
            return str.Replace("\\", "/").ToLower();
        }
        public static string GetFileExtention(string fileName)
        {
            return fileName.Substring(fileName.LastIndexOf('.') + 1);
        }
        public static string CheckPrefabName(string path)
        {
            return path.EndsWith(".prefab") ? path : path + ".prefab";
        }
        public static string RemovePrefabName(string prefab)
        {
            return prefab.EndsWith(".prefab") ? prefab.Replace(".prefab", "") : prefab;
        }
        #endregion

        #region zip

        public static void CompressDirectory(string sourcePath, string outputFilePath, int zipLevel = 0)
        {
            FileStream compressed = new FileStream(outputFilePath, FileMode.OpenOrCreate);
            compressed.CompressDirectory(sourcePath, zipLevel);
        }

        public static void DecompressToDirectory(string targetPath, string zipFilePath)
        {
            Debug.Log($"******* Target Dir {targetPath} / Zip File at { zipFilePath }*************");
            if (File.Exists(zipFilePath))
            {
                Debug.Log("************** FileStrean read zip .. ..  ******************");
                var compressed = File.OpenRead(zipFilePath);
                compressed.DecompressToDirectory(targetPath);
            }
        }

        public static void CompressDirectory(this Stream target, string sourcePath, int zipLevel)
        {
            sourcePath = Path.GetFullPath(sourcePath);

            int trimOffset = (string.IsNullOrEmpty(sourcePath)
                                  ? Path.GetPathRoot(sourcePath).Length
                                  : sourcePath.Length);


            List<string> fileSystemEntries = new List<string>();

            fileSystemEntries
                .AddRange(Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories)
                              .Select(d => d + "\\"));

            fileSystemEntries
                .AddRange(Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories));


            using (ZipOutputStream compressor = new ZipOutputStream(target))
            {
                compressor.SetLevel(zipLevel);
                int count = fileSystemEntries.Count;

                for (int i = 0; i < count; i++)
                {
                    var filePath = fileSystemEntries[i];
                    var trimFile = filePath.Substring(trimOffset);
                    var file = trimFile.StartsWith(@"\") ? trimFile.ReplaceFirst(@"\", "") : trimFile;
                    file = file.Replace(@"\", "/");
                    compressor.PutNextEntry(new ZipEntry(file));

                    if (filePath.EndsWith(@"\"))
                    {
                        continue;
                    }

                    byte[] data = new byte[2048];

                    using (FileStream input = File.OpenRead(filePath))
                    {
                        int bytesRead;

                        while ((bytesRead = input.Read(data, 0, data.Length)) > 0)
                        {
                            compressor.Write(data, 0, bytesRead);
                        }
                    }
                }
                compressor.Finish();
            }
        }

        public static void DecompressToDirectory(this Stream source, string targetPath)
        {
            targetPath = Path.GetFullPath(targetPath);
            Debug.Log($"*************** Zip Target Full path : { targetPath } *********************");
            using (ZipInputStream decompressor = new ZipInputStream(source))
            {
                Debug.Log($"*************** ZipStream Generate *********************");
                ZipEntry entry;
                while ((entry = decompressor.GetNextEntry()) != null)
                {
                    string name = entry.Name;
                    Debug.Log($"*************** ZipStream Entry Name : {name} *********************");
                    if (entry.IsDirectory && entry.Name.StartsWith("\\"))
                        name = entry.Name.ReplaceFirst("\\", "");
                    string filePath = Path.Combine(targetPath, name);
                    string directoryPath = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                    {
                        Debug.Log($"*************** ZipStream Entry Create Directory *********************");
                        Directory.CreateDirectory(directoryPath);
                    }
                    if (entry.IsDirectory)
                        continue;
                    byte[] data = new byte[2048];
                    using (FileStream streamWriter = File.Create(filePath))
                    {
                        int bytesRead = 0;

                        while ((bytesRead = decompressor.Read(data, 0, data.Length)) > 0)
                        {
                            streamWriter.Write(data, 0, bytesRead);
                        }
                    }
                }
            }
        }
        #endregion

        #region enctypt
        public static string Encrypt(string textToEncrypt, string fileName = "")
        {
            string enctyptKey = "";
#if !UNITY_EDITOR
        //enctyptKey = SystemSetting.EncryptKey;
#else
            enctyptKey = privateKey;
#endif
            if (!string.IsNullOrEmpty(fileName)) fileName = GetFileName(fileName);
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            byte[] PlainText = Encoding.Unicode.GetBytes(textToEncrypt);
            byte[] salt = Encoding.ASCII.GetBytes((enctyptKey + fileName).Length.ToString());
            PasswordDeriveBytes secretKey = new PasswordDeriveBytes((enctyptKey + fileName), salt);
            ICryptoTransform Encryptor = rijndaelCipher.CreateEncryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));
            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(PlainText, 0, PlainText.Length);
            cryptoStream.FlushFinalBlock();

            byte[] CipherBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();

            return Convert.ToBase64String(CipherBytes);

        }
        public static string Decrypt(string textToDecrypt, string fileName = "")
        {
            try
            {
                string enctyptKey = "";
#if !UNITY_EDITOR
         //   enctyptKey = SystemSetting.EncryptKey;
#else
                enctyptKey = privateKey;
#endif
                if (!string.IsNullOrEmpty(fileName)) fileName = GetFileName(fileName);
                RijndaelManaged rijndaelCipher = new RijndaelManaged();

                byte[] EncryptData = Convert.FromBase64String(textToDecrypt);
                byte[] Salt = Encoding.ASCII.GetBytes((enctyptKey + fileName).Length.ToString());

                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes((enctyptKey + fileName), Salt);
                ICryptoTransform Decryptor = rijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
                MemoryStream memoryStream = new MemoryStream(EncryptData);
                CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);

                byte[] PlainText = new byte[EncryptData.Length];
                int DecrypedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);

                memoryStream.Close();
                cryptoStream.Close();

                return Encoding.Unicode.GetString(PlainText, 0, DecrypedCount);
            }
            catch (Exception e)
            {
                return null;
            }

            //return Convert.ToBase64String(PlainText);
        }

        public static Dictionary<string, string> GetMD5List(string path, HashSet<string> set, bool folder = false)
        {
            Dictionary<string, string> md5List = new Dictionary<string, string>();
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            DirectoryInfo dir = new DirectoryInfo(path);

            var fileList = dir.GetFiles("*.*", SearchOption.AllDirectories);
            for (int i = 0; i < fileList.Length; i++)
            {
                if (!set.Contains(fileList[i].Extension))
                {
                    md5List.Add(folder ? GetFilePath(fileList[i].FullName, path) : GetFileName(fileList[i].FullName), GetFileMD5(fileList[i].FullName));
                }
            }
            return md5List;
        }
        public static string GetFileMD5(string fileName)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                }
            }
        }
        #endregion
        #region byte변환
        //구조체를 byte배열로 변환
        public static byte[] StructureToByte(object obj)
        {
            int datasize = Marshal.SizeOf(obj);
            IntPtr buff = Marshal.AllocHGlobal(datasize);
            Marshal.StructureToPtr(obj, buff, false);
            byte[] data = new byte[datasize];
            Marshal.Copy(buff, data, 0, datasize);
            Marshal.FreeHGlobal(buff);
            return data;
        }

        //byte배열을 구조체로 변환
        public static object ByteToStructure(byte[] data, Type type)
        {
            IntPtr buff = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, buff, data.Length);
            object obj = Marshal.PtrToStructure(buff, type);

            Marshal.FreeHGlobal(buff);
            return obj;
        }
        #endregion
        #region Temp
        
        public static void MountToSomeObjWithoutPosChange(Transform child, Transform parent)
        {
            Vector3 scale = child.localScale;
            Vector3 position = child.localPosition;
            Vector3 angle = child.localEulerAngles;
            child.parent = parent;
            child.localScale = scale;
            child.localEulerAngles = angle;
            child.localPosition = position;
        }

        public static System.Random CreateRandom()
        {
            long tick = DateTime.Now.Ticks;
            return new System.Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
        }

        public static T Choice<T>(List<T> list)
        {
            if (list.Count == 0)
            {
                return default(T);
            }

            int index = UnityEngine.Random.Range(0, list.Count);
            return list[index];
        }

        public static Dictionary<Int32, Int32> ParseMapIntInt(this String strMap, Char keyValueSpriter = KEY_VALUE_SPRITER, Char mapSpriter = MAP_SPRITER)
        {
            Dictionary<Int32, Int32> result = new Dictionary<Int32, Int32>();
            var strResult = ParseMap(strMap, keyValueSpriter, mapSpriter);
            foreach (var item in strResult)
            {
                int key;
                int value;
                if (int.TryParse(item.Key, out key) && int.TryParse(item.Value, out value))
                    result.Add(key, value);
                else
                    LoggerHelper.Warning(String.Format("Parse failure: {0}, {1}", item.Key, item.Value));
            }
            return result;
        }


        public static Byte[] CreateMD5(Byte[] data)
        {

            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(data);
            }
        }

        public static string FormatMD5(Byte[] data)
        {
            return System.BitConverter.ToString(data).Replace("-", "").ToLower();
        }

        public static String BuildFileMd5(String filename)
        {
            String filemd5 = null;
            try
            {
                using (var fileStream = File.OpenRead(filename))
                {
                    //UnityEditor.AssetDatabase
                    var md5 = MD5.Create();
                    var fileMD5Bytes = md5.ComputeHash(fileStream);//计算指定Stream 对象的哈希值                            
                    //fileStream.Close();//流数据比较大，手动卸载 
                    //fileStream.Dispose();
                    //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”               
                    filemd5 = FormatMD5(fileMD5Bytes);
                }
            }
            catch (System.Exception ex)
            {
                LoggerHelper.Except(ex);
            }
            return filemd5;
        }

        public static bool IsInTimeRange(DateTime cur, DateTime before, DateTime after)
        {
            if (cur >= before && cur <= after) { return true; }
            return false;
        }
        public static bool IsInTimeRange(DateTime cur, DateTime before)
        {
            if (cur >= before) { return true; }
            return false;
        }
        private static bool IsBaseType(Type type)
        {
            if (type == typeof(byte) || type == typeof(sbyte)
                || type == typeof(short) || type == typeof(ushort)
                || type == typeof(int) || type == typeof(uint)
                || type == typeof(Int64) || type == typeof(UInt64)
                || type == typeof(float) || type == typeof(double)
                || type == typeof(string) || type == typeof(bool))
                return true;
            else
                return false;
        }

        public static List<T> ParseListAny<T>(this String strList, Char listSpriter = LIST_SPRITER)
        {
            var type = typeof(T);
            var list = strList.ParseList(listSpriter);
            var result = new List<T>();
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                result.Add((T)GetValue(list[i], type));
            }
            return result;
        }
        #endregion

        #region state
        public static ulong BitSet(ulong data, int nBit)
        {
            if (nBit >= 0 && nBit < (int)sizeof(ulong) * 8)
            {
                data |= (ulong)(1 << nBit);
            }

            return data;
        }

        public static ulong BitReset(ulong data, int nBit)
        {
            if (nBit >= 0 && nBit < (int)sizeof(ulong) * 8)
            {
                data &= (ulong)(~(1 << nBit));
            };

            return data;
        }

        public static int BitTest(ulong data, int nBit)
        {
            int nRet = 0;
            if (nBit >= 0 && nBit < (int)sizeof(ulong) * 8)
            {
                data &= (ulong)(1 << nBit);
                if (data != 0) nRet = 1;
            }
            return nRet;
        }
        #endregion
    }

}
