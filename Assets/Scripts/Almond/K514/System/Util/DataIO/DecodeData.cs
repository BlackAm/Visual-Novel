using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Cysharp.Threading.Tasks;
using ICSharpCode.SharpZipLib.BZip2;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 지정한 파일이나 디렉터리의 압축을 해제하는 클래스
    /// </summary>   
    public static class DecodeData
    {
        #region <Decode>

        public static T DeserializeObject<T>(this byte[] p_ByteArray)
        {
            if (ReferenceEquals(null, p_ByteArray))
            {
                return default;
            }
            else
            {
                var bf = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    ms.Write(p_ByteArray, 0, p_ByteArray.Length);
                    ms.Seek(0, SeekOrigin.Begin);
                    return (T) bf.Deserialize(ms);
                }
            }
        }
        
        public static T DeserializeObject<T>(string p_TargetFileFullPath)
        {
            var tryByte = File.ReadAllBytes(p_TargetFileFullPath);
            return tryByte.DeserializeObject<T>();
        }
        
        public static T DeserializeObject<T>(TextAsset p_DefaultAsset)
        {
            return p_DefaultAsset.bytes.DeserializeObject<T>();
        }

        #endregion

        #region <Decompress>

        public static void DecompressDirectory(ExportDataTool.CompressType p_Type, string p_TargetCompressedFile, string p_TargetDirectory)
        {
            switch (p_Type)
            {
                case ExportDataTool.CompressType.BZip2:
                    DecompressDirectory_BZ2(p_TargetCompressedFile, p_TargetDirectory);
                    break;
                case ExportDataTool.CompressType.Gzip:
                    DecompressDirectory_GZ(p_TargetCompressedFile, p_TargetDirectory);
                    break;
            }
        }

        public static async UniTask DecompressDirectoryAsync(ExportDataTool.CompressType p_Type, string p_TargetCompressedFile, string p_TargetDirectory)
        {
            switch (p_Type)
            {
                case ExportDataTool.CompressType.BZip2:
                    await DecompressDirectory_BZ2Async(p_TargetCompressedFile, p_TargetDirectory);
                    break;
                case ExportDataTool.CompressType.Gzip:
                    await DecompressDirectory_GZAsync(p_TargetCompressedFile, p_TargetDirectory);
                    break;
            }
        }
        
        /// <summary>
        /// 지정한 압축파일의 위치에 압축을 해제시키는 숏컷 메서드
        /// </summary>
        public static void DecompressDirectory(ExportDataTool.CompressType p_Type, string p_TargetCompressedFullFileName)
        {
            DecompressDirectory(p_Type, p_TargetCompressedFullFileName, $"{Path.GetDirectoryName(p_TargetCompressedFullFileName)}/{Path.GetFileNameWithoutExtension(p_TargetCompressedFullFileName)}");
        }
        
        /// <summary>
        /// 지정한 압축파일의 위치에 압축을 해제시키는 숏컷 메서드, 비동기
        /// </summary>
        public static async UniTask DecompressDirectoryAsync(ExportDataTool.CompressType p_Type, string p_TargetCompressedFullFileName)
        {
            await DecompressDirectoryAsync(p_Type, p_TargetCompressedFullFileName, $"{Path.GetDirectoryName(p_TargetCompressedFullFileName)}/{Path.GetFileNameWithoutExtension(p_TargetCompressedFullFileName)}");
        }

        #endregion
        
        #region <Decompress/BZip2>

        private static bool DecompressFile_BZ2(string p_TargetFilePath, BZip2InputStream p_ZipStream)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintDataIO)
            {
                Debug.Log($"  * Try Decompress Directory Name [{p_TargetFilePath}]");
            }
#endif
            var windowBuffer = new byte[sizeof(int)];
            // Encode 클래스의 압축에서 가장 첫번째 스트림에 저장되었던 것은, '파일 이름의 길이' 정수 값
            var readedByteCount = p_ZipStream.Read(windowBuffer, 0, sizeof(int));
            if (readedByteCount < sizeof(int))
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintDataIO)
                {
                    if (readedByteCount == 0)
                    {
                        Debug.Log($"  * Decompress Over Successfully");
                    }
                    else
                    {
                        Debug.LogError($"  * File Name Length Data Breaked : [{readedByteCount}] Byte(s), Decompress Failed");
                    }
                }
#endif
                return false;
            }

            // 읽은 바이트 값을 정수(파일 이름 길이)로 변환한다.
            var fileNameLength = BitConverter.ToInt32(windowBuffer, 0);
        
            // Encode 클래스의 압축에서 '파일 이름의 길이' 이후에 직렬화된 것은 '파일 이름' 문자열 값
            windowBuffer = new byte[sizeof(char)];
        
            // 읽은 바이트 값을 문자열(파일이름)로 변환한다.
            var sb = new StringBuilder();
            for (int i = 0; i < fileNameLength; i++)
            {
                p_ZipStream.Read(windowBuffer, 0, sizeof(char));
                var c = BitConverter.ToChar(windowBuffer, 0);
                sb.Append(c);
            }
            var fileName = sb.ToString();
#if UNITY_EDITOR
            if (CustomDebug.PrintDataIO)
            {
                Debug.Log($"  * file Name : [{fileName}]");
            }
#endif
            // Encode 클래스의 압축에서 '파일 이름' 이후에 직렬화된 것은 '파일 바이트 용량' 정수 값
            windowBuffer = new byte[sizeof(int)];
            p_ZipStream.Read(windowBuffer, 0, sizeof(int));
            var fileByteLength = BitConverter.ToInt32(windowBuffer, 0);

            // Encode 클래스의 압축에서 '파일 바이트 용량' 이후에 직렬화된 것은 '파일 바이트 코드' 정수 배열 값
            windowBuffer = new byte[fileByteLength];
            p_ZipStream.Read(windowBuffer, 0, fileByteLength);

            // 안드로이드는 역슬래시 경로를 읽지 못하므로 슬래시로 바꿔준다.
            string fileFullPath = Path.Combine(p_TargetFilePath, fileName).TurnToSlash();
            string terminalDirectoryPath = Path.GetDirectoryName(fileFullPath).TurnToSlash();
            if (!Directory.Exists(terminalDirectoryPath))
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintDataIO)
                {
                    Debug.Log($"  * Directory Spawned : [{terminalDirectoryPath}]");
                }
#endif
                Directory.CreateDirectory(terminalDirectoryPath);
            }

            // 파일 생성후, 파일에 스트림으로부터 바이트 코드를 기술해준다.
            using (FileStream outFile = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                outFile.Write(windowBuffer, 0, fileByteLength);
            }

            return true;
        }
      
        public static void DecompressDirectory_BZ2(string p_TargetCompressedFile, string p_TargetDirectory)
        {
            using (FileStream inFile = new FileStream(p_TargetCompressedFile, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using (BZip2InputStream zipStream = new BZip2InputStream(inFile))
                {
                    while (DecompressFile_BZ2(p_TargetDirectory, zipStream));
                }
            }
        }

        private static async UniTask<bool> DecompressFile_BZ2Async(string p_TargetFilePath, BZip2InputStream p_ZipStream)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintDataIO)
            {
                Debug.Log($"  * Try Decompress Directory Name [{p_TargetFilePath}]");
            }
#endif
            var windowBuffer = new byte[sizeof(int)];
            // Encode 클래스의 압축에서 가장 첫번째 스트림에 저장되었던 것은, '파일 이름의 길이' 정수 값
            var readedByteCount = await p_ZipStream.ReadAsync(windowBuffer, 0, sizeof(int));
            if (readedByteCount < sizeof(int))
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintDataIO)
                {
                    if (readedByteCount == 0)
                    {
                        Debug.Log($"  * Decompress Over Successfully");
                    }
                    else
                    {
                        Debug.LogError($"  * File Name Length Data Breaked : [{readedByteCount}] Byte(s), Decompress Failed");
                    }
                }
#endif
                return false;
            }

            // 읽은 바이트 값을 정수(파일 이름 길이)로 변환한다.
            var fileNameLength = BitConverter.ToInt32(windowBuffer, 0);
        
            // Encode 클래스의 압축에서 '파일 이름의 길이' 이후에 직렬화된 것은 '파일 이름' 문자열 값
            windowBuffer = new byte[sizeof(char)];
        
            // 읽은 바이트 값을 문자열(파일이름)로 변환한다.
            var sb = new StringBuilder();
            for (int i = 0; i < fileNameLength; i++)
            {
                await p_ZipStream.ReadAsync(windowBuffer, 0, sizeof(char));
                var c = BitConverter.ToChar(windowBuffer, 0);
                sb.Append(c);
            }
            var fileName = sb.ToString();
#if UNITY_EDITOR
            if (CustomDebug.PrintDataIO)
            {
                Debug.Log($"  * file Name : [{fileName}]");
            }
#endif
            Debug.Log($"  * file Name : [{fileName}]");
            
            // Encode 클래스의 압축에서 '파일 이름' 이후에 직렬화된 것은 '파일 바이트 용량' 정수 값
            windowBuffer = new byte[sizeof(int)];
            await p_ZipStream.ReadAsync(windowBuffer, 0, sizeof(int));
            var fileByteLength = BitConverter.ToInt32(windowBuffer, 0);

            // Encode 클래스의 압축에서 '파일 바이트 용량' 이후에 직렬화된 것은 '파일 바이트 코드' 정수 배열 값
            windowBuffer = new byte[fileByteLength];
            await p_ZipStream.ReadAsync(windowBuffer, 0, fileByteLength);

            // 안드로이드는 역슬래시 경로를 읽지 못하므로 슬래시로 바꿔준다.
            string fileFullPath = Path.Combine(p_TargetFilePath, fileName).TurnToSlash();
            string terminalDirectoryPath = Path.GetDirectoryName(fileFullPath).TurnToSlash();
            if (!Directory.Exists(terminalDirectoryPath))
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintDataIO)
                {
                    Debug.Log($"  * Directory Spawned : [{terminalDirectoryPath}]");
                }
#endif
                Directory.CreateDirectory(terminalDirectoryPath);
            }

            // 파일 생성후, 파일에 스트림으로부터 바이트 코드를 기술해준다.
            using (FileStream outFile = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                outFile.Write(windowBuffer, 0, fileByteLength);
            }

            return true;
        }
      
        public static async UniTask DecompressDirectory_BZ2Async(string p_TargetCompressedFile, string p_TargetDirectory)
        {
            using (FileStream inFile = new FileStream(p_TargetCompressedFile, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using (BZip2InputStream zipStream = new BZip2InputStream(inFile))
                {
                    while (await DecompressFile_BZ2Async(p_TargetDirectory, zipStream));
                }
            }
        }
        
        #endregion
        
        #region <Decompress/GZ>

        private static bool DecompressFile_GZ(string p_TargetFilePath, GZipStream p_ZipStream)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintDataIO)
            {
                Debug.Log($"  * Try Decompress Directory Name [{p_TargetFilePath}]");
            }
#endif
            var windowBuffer = new byte[sizeof(int)];
            // Encode 클래스의 압축에서 가장 첫번째 스트림에 저장되었던 것은, '파일 이름의 길이' 정수 값
            var readedByteCount = p_ZipStream.Read(windowBuffer, 0, sizeof(int));
            if (readedByteCount < sizeof(int))
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintDataIO)
                {
                    if (readedByteCount == 0)
                    {
                        Debug.Log($"  * Decompress Over Successfully");
                    }
                    else
                    {
                        Debug.LogError($"  * File Name Length Data Breaked : [{readedByteCount}] Byte(s), Decompress Failed");
                    }
                }
#endif
                return false;
            }

            // 읽은 바이트 값을 정수(파일 이름 길이)로 변환한다.
            var fileNameLength = BitConverter.ToInt32(windowBuffer, 0);
        
            // Encode 클래스의 압축에서 '파일 이름의 길이' 이후에 직렬화된 것은 '파일 이름' 문자열 값
            windowBuffer = new byte[sizeof(char)];
        
            // 읽은 바이트 값을 문자열(파일이름)로 변환한다.
            var sb = new StringBuilder();
            for (int i = 0; i < fileNameLength; i++)
            {
                p_ZipStream.Read(windowBuffer, 0, sizeof(char));
                var c = BitConverter.ToChar(windowBuffer, 0);
                sb.Append(c);
            }
            var fileName = sb.ToString();

            // Encode 클래스의 압축에서 '파일 이름' 이후에 직렬화된 것은 '파일 바이트 용량' 정수 값
            windowBuffer = new byte[sizeof(int)];
            p_ZipStream.Read(windowBuffer, 0, sizeof(int));
            var fileByteLength = BitConverter.ToInt32(windowBuffer, 0);

            // Encode 클래스의 압축에서 '파일 바이트 용량' 이후에 직렬화된 것은 '파일 바이트 코드' 정수 배열 값
            windowBuffer = new byte[fileByteLength];
            p_ZipStream.Read(windowBuffer, 0, fileByteLength);

            // 안드로이드는 역슬래시 경로를 읽지 못하므로 슬래시로 바꿔준다.
            string fileFullPath = Path.Combine(p_TargetFilePath, fileName).TurnToSlash();
            string terminalDirectoryPath = Path.GetDirectoryName(fileFullPath).TurnToSlash();
            if (!Directory.Exists(terminalDirectoryPath))
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintDataIO)
                {
                    Debug.Log($"  * Directory Spawned : [{terminalDirectoryPath}]");
                }
#endif
                Directory.CreateDirectory(terminalDirectoryPath);
            }

            // 파일 생성후, 파일에 스트림으로부터 바이트 코드를 기술해준다.
            using (FileStream outFile = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                outFile.Write(windowBuffer, 0, fileByteLength);
            }

            return true;
        }
      
        private static void DecompressDirectory_GZ(string p_TargetCompressedFile, string p_TargetDirectory)
        {
            using (FileStream inFile = new FileStream(p_TargetCompressedFile, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using (GZipStream zipStream = new GZipStream(inFile, CompressionMode.Decompress, true))
                {
                    while (DecompressFile_GZ(p_TargetDirectory, zipStream));
                }
            }
        }

        private static async UniTask<bool> DecompressFile_GZAsync(string p_TargetFilePath, GZipStream p_ZipStream)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintDataIO)
            {
                Debug.Log($"  * Try Decompress Directory Name [{p_TargetFilePath}]");
            }
#endif
            var windowBuffer = new byte[sizeof(int)];
            // Encode 클래스의 압축에서 가장 첫번째 스트림에 저장되었던 것은, '파일 이름의 길이' 정수 값
            var readedByteCount = await p_ZipStream.ReadAsync(windowBuffer, 0, sizeof(int));
            if (readedByteCount < sizeof(int))
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintDataIO)
                {
                    if (readedByteCount == 0)
                    {
                        Debug.Log($"  * Decompress Over Successfully");
                    }
                    else
                    {
                        Debug.LogError($"  * File Name Length Data Breaked : [{readedByteCount}] Byte(s), Decompress Failed");
                    }
                }
#endif
                return false;
            }

            // 읽은 바이트 값을 정수(파일 이름 길이)로 변환한다.
            var fileNameLength = BitConverter.ToInt32(windowBuffer, 0);
        
            // Encode 클래스의 압축에서 '파일 이름의 길이' 이후에 직렬화된 것은 '파일 이름' 문자열 값
            windowBuffer = new byte[sizeof(char)];
        
            // 읽은 바이트 값을 문자열(파일이름)로 변환한다.
            var sb = new StringBuilder();
            for (int i = 0; i < fileNameLength; i++)
            {
                await p_ZipStream.ReadAsync(windowBuffer, 0, sizeof(char));
                var c = BitConverter.ToChar(windowBuffer, 0);
                sb.Append(c);
            }
            var fileName = sb.ToString();

            // Encode 클래스의 압축에서 '파일 이름' 이후에 직렬화된 것은 '파일 바이트 용량' 정수 값
            windowBuffer = new byte[sizeof(int)];
            await p_ZipStream.ReadAsync(windowBuffer, 0, sizeof(int));
            var fileByteLength = BitConverter.ToInt32(windowBuffer, 0);

            // Encode 클래스의 압축에서 '파일 바이트 용량' 이후에 직렬화된 것은 '파일 바이트 코드' 정수 배열 값
            windowBuffer = new byte[fileByteLength];
            await p_ZipStream.ReadAsync(windowBuffer, 0, fileByteLength);

            // 안드로이드는 역슬래시 경로를 읽지 못하므로 슬래시로 바꿔준다.
            string fileFullPath = Path.Combine(p_TargetFilePath, fileName).TurnToSlash();
            string terminalDirectoryPath = Path.GetDirectoryName(fileFullPath).TurnToSlash();
            if (!Directory.Exists(terminalDirectoryPath))
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintDataIO)
                {
                    Debug.Log($"  * Directory Spawned : [{terminalDirectoryPath}]");
                }
#endif
                Directory.CreateDirectory(terminalDirectoryPath);
            }

            // 파일 생성후, 파일에 스트림으로부터 바이트 코드를 기술해준다.
            using (FileStream outFile = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                outFile.Write(windowBuffer, 0, fileByteLength);
            }

            return true;
        }
      
        private static async UniTask DecompressDirectory_GZAsync(string p_TargetCompressedFile, string p_TargetDirectory)
        {
            using (FileStream inFile = new FileStream(p_TargetCompressedFile, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using (GZipStream zipStream = new GZipStream(inFile, CompressionMode.Decompress, true))
                {
                    while (await DecompressFile_GZAsync(p_TargetDirectory, zipStream));
                }
            }
        }
        
        #endregion
    }
}