using System;
using System.IO;
using UnityEngine;

namespace k514
{
    public static partial class SystemTool
    {
        public static (bool t_Valid, string[] t_Lines) TryReadLines(string p_Path)
        {
            try
            {
                return (true, File.ReadAllLines(p_Path));
            }
#if UNITY_EDITOR
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return default;
            }
#else
            catch
            {
                return default;
            }
#endif
        }

        /// <summary>
        /// 지정한 경로로부터 문서 파일을 읽는 메서드
        /// </summary>
        public static bool LoadFile(string p_FilePath, out string o_ReadOut)
        {
            if (File.Exists(p_FilePath))
            {
                using (StreamReader sr = File.OpenText(p_FilePath))
                {
                    o_ReadOut = sr.ReadToEnd();
                    return true;
                }
            }

            o_ReadOut = String.Empty;
            return false;
        }
        
        /// <summary>
        /// 특정 디렉터리에서 지정한 디렉터리로 디렉터리 복사를 하는 메서드
        /// 옮길 디렉터리에 동명의 파일이 있다면 덮어쓴다.
        /// </summary>
        public static void CopyDirectoryTo(string p_FromPath, string p_ToPath, SearchOption p_SearchOption = SearchOption.AllDirectories, string p_SearchPattern = "*")
        {
            var allFilePath = Directory.GetFiles(p_FromPath, p_SearchPattern, p_SearchOption);
            foreach (var targetFilePath in allFilePath)
            {
                CopyFileTo(p_FromPath, targetFilePath, p_ToPath, true);
            }
        }

        /// <summary>
        /// 특정 파일을 지정한 디렉터리로 복사하는 메서드
        /// </summary>
        public static void CopyFileTo(string p_Root, string p_FileFullPath, string p_ToPath, bool p_OverlapFile)
        {
            var tryFileName = p_FileFullPath.CutString(p_Root, false, true).TurnToSlash();
            var targetFileFullPath = p_ToPath + tryFileName;
            var targetFileUpperPath = targetFileFullPath.GetUpperPath();
            
            // 생성할 디렉터리와 동명의 파일이 존재하는 경우
            if (File.Exists(targetFileUpperPath))
            {
                if (p_OverlapFile)
                {
                    File.Delete(targetFileUpperPath);
                }
                else
                {
                    return;
                }
            }
            
            // 옮길 디렉터리가 존재하지 않는 경우에 생성한다.
            if (!Directory.Exists(targetFileUpperPath))
            {
                Directory.CreateDirectory(targetFileUpperPath);
            }

            // 옮길 디렉터리에 동명의 파일이 존재하는 경우
            if (File.Exists(targetFileFullPath))
            {
                if (p_OverlapFile)
                {
                    File.Delete(targetFileFullPath);
                }
                else
                {
                    return;
                }
            }

            File.Copy(p_FileFullPath, targetFileFullPath);
        }

        /// <summary>
        /// 지정한 디렉터리 내부에 존재하는 파일을 보유하지 않은 텅빈 디렉터리를 일괄삭제하는 메서드
        /// </summary>
        public static void DeleteEmptyDirectoryAt(string p_TargetPath)
        {
            var allDirectory = Directory.GetDirectories(p_TargetPath);
            foreach (var bundleDirectoryName in allDirectory)
            {
                if (Directory.Exists(bundleDirectoryName))
                {
                    var allFileName = Directory.GetFiles(bundleDirectoryName, "*.*", SearchOption.AllDirectories);
                    if (allFileName.Length < 1)
                    {
                        Directory.Delete(bundleDirectoryName);
                    }
                }
            }
        }

        /// <summary>
        /// 지정한 디렉토리의 읽기전용 속성을 지우는 메서드
        /// </summary>
        public static void MakeFolderWritable(this DirectoryInfo p_DirectoryInfo)
        {
            if (p_DirectoryInfo.IsFolderReadOnly())
            {
                p_DirectoryInfo.Attributes = p_DirectoryInfo.Attributes & ~FileAttributes.ReadOnly;
            }
        }
        
        /// <summary>
        /// 지정한 디렉터리의 속성에 읽기전용이 있는지 검증하는 메서드
        /// </summary>
        private static bool IsFolderReadOnly(this DirectoryInfo p_DirectoryInfo)
        {
            return (p_DirectoryInfo.Attributes & FileAttributes.ReadOnly) > 0;
        }
        
        /// <summary>
        /// 두 파일의 바이너리 이미지를 비교해서, 같은 파일인지 검증하는 논리 메서드
        /// </summary>
        public static bool IsSameFile(this byte[] p_Left, byte[] p_Right)
        {
            if (p_Left.Length != p_Right.Length) return false;
            for (int i = 0; i < p_Left.Length; i++)
            {
                if (p_Left[i] != p_Right[i]) return false;
            }

            return true;
        }
    }
}