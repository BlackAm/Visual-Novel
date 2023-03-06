using Cysharp.Threading.Tasks;
using UnityEngine;
using System.IO;

namespace k514
{
    public static class ExportDataTool
    {
        #region <Consts>
        
#if UNITY_EDITOR
        public const string BackUpFormat = "_Backup";
#endif
        
        #endregion
        
        #region <Enums>

#if UNITY_EDITOR
        public enum WriteType
        {
            Overlap, CopyWithNumbering, BackUp
        }
#endif
        
        public enum CompressType
        {
            BZip2,
            Gzip,
        }

        #endregion

        #region <Method/Path>

#if UNITY_EDITOR
        public static string GetNumberingFileRegex(string p_FileName, string p_FileExt)
        {
            return $"{p_FileName}_[0-9]*\\{p_FileExt}";
        }

        /// <summary>
        /// 특정한 디렉터리에 특정한 이름의 파일이 존재하는지 검색하여 생성에 적합한 경로를 리턴하는 메서드
        /// </summary>
        public static async UniTask<string> TryCheckGetExportFilePath(string p_Path, string p_FileName, string p_ExtFormat, WriteType p_WriteType)
        {
            await UniTask.SwitchToMainThread();
            
            if (!Directory.Exists(p_Path))
            {
                Directory.CreateDirectory(p_Path);
                return p_Path + p_FileName + p_ExtFormat;
            }
            else
            {
                switch (p_WriteType)
                {
                    case WriteType.Overlap :
                        return p_Path + p_FileName + p_ExtFormat;
                    case WriteType.CopyWithNumbering :
                        var startIndex = -1;
                        string tryName = null;
                        while (true)
                        {
                            tryName = startIndex == -1 ? p_Path + p_FileName + p_ExtFormat : p_Path + p_FileName + $"_{startIndex}" + p_ExtFormat;
                            Debug.Log($"[{p_Path}] / [{p_FileName}] / [{startIndex}] / [{p_ExtFormat}]");
                            if (File.Exists(tryName))
                            {
                                startIndex++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        return tryName;
                    case WriteType.BackUp :
                        return p_Path + p_FileName + BackUpFormat + p_ExtFormat;
                }
            }
            
            return p_Path + p_FileName + p_ExtFormat;
        }

        /// <summary>
        /// 특정한 디렉터리에 특정한 이름의 디렉터리가 존재하는지 검색하여 생성에 적합한 디렉터리 경로를 리턴하는 메서드
        /// </summary>
        public static async UniTask<string> TryCheckExportDirectoryPath(string p_Path, string p_DirectoryName, WriteType p_Type)
        {
            await UniTask.SwitchToMainThread();

            if (!Directory.Exists(p_Path))
            {
                Directory.CreateDirectory(p_Path);
                return p_Path + p_DirectoryName;
            }
            else
            {
                switch (p_Type)
                {
                    case WriteType.Overlap :
                        return p_Path + p_DirectoryName;
                    case WriteType.CopyWithNumbering :
                        var startIndex = 0;
                        string tryName = null;
                        while (true)
                        {
                            tryName = p_Path + p_DirectoryName + startIndex;
                            if (Directory.Exists(tryName))
                            {
                                startIndex++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        return tryName;
                    case WriteType.BackUp :
                        return p_Path + p_DirectoryName + BackUpFormat;
                }
            }
            
            return p_Path + p_DirectoryName;
        }
#endif     
        
        #endregion
    }
}
