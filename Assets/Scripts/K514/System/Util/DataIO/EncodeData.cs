#if UNITY_EDITOR
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.BZip2;
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 지정한 파일이나 디렉터리를 압축하는 클래스
    /// </summary>   
    public static class EncodeData
    {
        #region <Encode>

        public static byte[] SerializeObject(this object p_Object)
        {
            if (ReferenceEquals(null, p_Object))
            {
                return null;
            }
            else
            {
                var bf = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, p_Object);
                    return ms.ToArray();
                }
            }
        }
        
        public static void SerializeObject(this object p_Object, string p_TargetFileFullPath)
        {
            var trySerialize = p_Object.SerializeObject();
            if (!ReferenceEquals(null, trySerialize))
            {
                var rootPath = p_TargetFileFullPath.GetUpperPath();
                if (!Directory.Exists(rootPath))
                {
                    Directory.CreateDirectory(rootPath);
                }
                using (var outFile = new FileStream(p_TargetFileFullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    outFile.Write(trySerialize, 0, trySerialize.Length);
                }
            }
        }
        
        #endregion

        #region <Compress>

        /// <summary>
        /// 디렉터리 압축 메서드
        /// </summary>
        public static void CompressDirectory(ExportDataTool.CompressType p_Type, string p_TargetDirectoryPath, string p_SpawnFileName, List<string> p_ExceptionFileNameSet)
        {
            switch (p_Type)
            {
                case ExportDataTool.CompressType.BZip2:
                    CompressDirectory_BZ2(p_TargetDirectoryPath, p_SpawnFileName, p_ExceptionFileNameSet);
                    break;
                case ExportDataTool.CompressType.Gzip:
                    CompressDirectory_GZ(p_TargetDirectoryPath, p_SpawnFileName, p_ExceptionFileNameSet);
                    break;
            }
        }

        #endregion
        
        #region <Compress/BZip2>

        private static void CompressFile_BZ2(string p_TargetFilePath, string p_TargetFileName, BZip2OutputStream p_ZipStream)
        {
            var fileFullPath = Path.Combine(p_TargetFilePath, p_TargetFileName);
#if UNITY_EDITOR
            if (CustomDebug.PrintDataIO)
            {
                Debug.Log($"  * Try Compress File Name [{fileFullPath}]");
            }
#endif

            // 스트림에 해당 파일의 이름 길이 및 이름을 Write한다.
            var fileNameCharSet = p_TargetFileName.ToCharArray();
            p_ZipStream.Write(BitConverter.GetBytes(fileNameCharSet.Length), 0, sizeof(int));
            foreach (char character in fileNameCharSet)
            {
                p_ZipStream.Write(BitConverter.GetBytes(character), 0, sizeof(char));
            }

            // 스트림에 해당 파일의 용량 및 바이트 코드를 Write한다.
            var fileByteCode = File.ReadAllBytes(fileFullPath);
            var fileByteLength = fileByteCode.Length;
            p_ZipStream.Write(BitConverter.GetBytes(fileByteLength), 0, sizeof(int));
            p_ZipStream.Write(fileByteCode, 0, fileByteLength);

#if UNITY_EDITOR
            if (CustomDebug.PrintDataIO)
            {
                Debug.Log($"  * File [{p_TargetFileName}] compressed [{fileByteLength} Byte(s)] at Stream");
            }
#endif
        }

        private static void CompressDirectory_BZ2(string p_TargetDirectoryPath, string p_SpawnFileName, List<string> p_ExceptionFileNameSet)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintDataIO)
            {
                Debug.Log($" - Try Compress Directory Name [{p_TargetDirectoryPath}]");
            }
#endif
            
            var SpawnDirectory = $"{Path.GetDirectoryName(p_SpawnFileName)}/";
            if (!Directory.Exists(SpawnDirectory))
            {
                Directory.CreateDirectory(SpawnDirectory);
#if UNITY_EDITOR
                if (CustomDebug.PrintDataIO)
                {
                    Debug.Log($" - Create CompressDirectory [{SpawnDirectory}]");
                }
#endif
            }

#if UNITY_EDITOR
            if (CustomDebug.PrintDataIO)
            {
                Debug.Log($" - Create CompressFile [{p_SpawnFileName}] relative with ProjectRootPath");
            }
#endif
            using (var outFile = new FileStream(p_SpawnFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var allFilePath = Directory.GetFiles(p_TargetDirectoryPath, "*.*", SearchOption.AllDirectories);
                var directoryLength = p_TargetDirectoryPath.Length;
                using (var zipStream = new BZip2OutputStream(outFile))
                {
                    foreach (var filePath in allFilePath)
                    {
                        string fileName = filePath.Substring(directoryLength);
                        if (p_ExceptionFileNameSet == null || !p_ExceptionFileNameSet.Contains(fileName))
                        {
                            CompressFile_BZ2(p_TargetDirectoryPath, fileName, zipStream);
                        }
                        else
                        {
#if UNITY_EDITOR
                            if (CustomDebug.PrintDataIO)
                            {
                                Debug.Log($" - the file [{fileName}] excepted from Comressing");
                            }
#endif 
                        }
                    }
                }
            }
        }

        #endregion
        
        #region <Compress/Gzip>
    
        private static void CompressFile_GZ(string p_TargetFilePath, string p_TargetFileName, GZipStream p_ZipStream)
        {
            var fileFullPath = Path.Combine(p_TargetFilePath, p_TargetFileName);
#if UNITY_EDITOR
            if (CustomDebug.PrintDataIO)
            {
                Debug.Log($"  * Try Compress File Name [{fileFullPath}]");
            }
#endif

            // 스트림에 해당 파일의 이름 길이 및 이름을 Write한다.
            var fileNameCharSet = p_TargetFileName.ToCharArray();
            p_ZipStream.Write(BitConverter.GetBytes(fileNameCharSet.Length), 0, sizeof(int));
            foreach (char character in fileNameCharSet)
            {
                p_ZipStream.Write(BitConverter.GetBytes(character), 0, sizeof(char));
            }

            // 스트림에 해당 파일의 용량 및 바이트 코드를 Write한다.
            var fileByteCode = File.ReadAllBytes(fileFullPath);
            var fileByteLength = fileByteCode.Length;
            p_ZipStream.Write(BitConverter.GetBytes(fileByteLength), 0, sizeof(int));
            p_ZipStream.Write(fileByteCode, 0, fileByteLength);

#if UNITY_EDITOR
            if (CustomDebug.PrintDataIO)
            {
                Debug.Log($"  * File [{p_TargetFileName}] compressed [{fileByteLength} Byte(s)] at Stream");
            }
#endif
        }

        private static void CompressDirectory_GZ(string p_TargetDirectoryPath, string p_SpawnFileName, List<string> p_ExceptionFileNameSet)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintDataIO)
            {
                Debug.Log($" - Try Compress Directory Name [{p_TargetDirectoryPath}]");
            }
#endif
            
            var SpawnDirectory = $"{Path.GetDirectoryName(p_SpawnFileName)}/";
            if (!Directory.Exists(SpawnDirectory))
            {
                Directory.CreateDirectory(SpawnDirectory);
#if UNITY_EDITOR
                if (CustomDebug.PrintDataIO)
                {
                    Debug.Log($" - Create CompressDirectory [{SpawnDirectory}]");
                }
#endif
            }

#if UNITY_EDITOR
            if (CustomDebug.PrintDataIO)
            {
                Debug.Log($" - Create CompressFile [{p_SpawnFileName}] relative with ProjectRootPath");
            }
#endif
            using (var outFile = new FileStream(p_SpawnFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var allFilePath = Directory.GetFiles(p_TargetDirectoryPath, "*.*", SearchOption.AllDirectories);
                var directoryLength = p_TargetDirectoryPath.Length;
                using (var zipStream = new GZipStream(outFile, CompressionMode.Compress))
                {
                    foreach (var filePath in allFilePath)
                    {
                        string fileName = filePath.Substring(directoryLength);
                        if (p_ExceptionFileNameSet == null || !p_ExceptionFileNameSet.Contains(fileName))
                        {
                            CompressFile_GZ(p_TargetDirectoryPath, fileName, zipStream);
                        }
                        else
                        {
#if UNITY_EDITOR
                            if (CustomDebug.PrintDataIO)
                            {
                                Debug.Log($" - the file [{fileName}] excepted from Comressing");
                            }
#endif 
                        }
                    }
                }
            }
        }
        
        #endregion
    }
}
#endif
