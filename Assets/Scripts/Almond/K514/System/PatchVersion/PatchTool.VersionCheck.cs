using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    /*public partial class PatchTool
    {
        #region <Fields>

        /// <summary>
        /// PlayerPreference로 관리중인 클라이언트 버전 접근 키
        /// </summary>
        private const string __PLAYER_PREFERENCE_CLIENT_VERSION = "__PLAYER_PREFERENCE_CLIENT_VERSION";

        /// <summary>
        /// 클라이언트 버전 Null값, 버전이 없는 경우 초기값
        /// </summary>
        public const int __Version_Null = -1;

        /// <summary>
        /// 클라이언트 버전 하한값
        /// </summary>
        public const int __Version_Lower_Bound = 514;
        
        #endregion

        #region <Enums>

        public enum PatchMode
        {
            None,
            Partial,
            Full,
            Same,
        }

        #endregion
        
        #region <Methods>

        /// <summary>
        /// 클라이언트 버전을 초기화시킨다.
        /// </summary>
        public static void InitClientVersion()
        {
            UpdateClientVersion(__Version_Null);
        }

        /// <summary>
        /// 현재 클라이언트 버전을 세트하는 메서드
        /// </summary>
        public static void UpdateClientVersion(int p_TargetVersion)
        {
            PlayerPrefs.SetInt(__PLAYER_PREFERENCE_CLIENT_VERSION, p_TargetVersion == __Version_Null ? __Version_Null : Mathf.Max(__Version_Lower_Bound, p_TargetVersion));
        }
        
        /// <summary>
        /// 클라이언트 버전이 존재하는지 체크하고, 초기화되지 않았다면 초기화시키는 메서드
        /// </summary>
        public static void CheckClientVersionValid()
        {
            if (!PlayerPrefs.HasKey(__PLAYER_PREFERENCE_CLIENT_VERSION))
            {
                InitClientVersion();
            }   
        }
        
        /// <summary>
        /// 현재 클라이언트 버전을 리턴하는 메서드
        /// </summary>
        public static int GetClientVersion()
        {
            CheckClientVersionValid();
            return PlayerPrefs.GetInt(__PLAYER_PREFERENCE_CLIENT_VERSION);
        }

        
        public static string GetBundleHash(string p_AssetBundlePath)
        {
            var hashSeed = new List<string>();
            var fileNameSet = Directory.GetFiles(p_AssetBundlePath, "*", SearchOption.AllDirectories);
            var fileNameSetLength = fileNameSet.Length;
            for (int i = 0; i < fileNameSet.Length; i++)
            {
                var tryFilePath = fileNameSet[i];
                var tryFileName = Path.GetFileName(tryFilePath);
                
                // 리소스 리스트는 시드에서 제외한다.
                if (tryFileName != ResourceTool.ResourceListFileNameWithExt)
                {
                    var tryExtension = Path.GetExtension(tryFileName);
                    switch (tryExtension)
                    {
                        // Case 1 : manifest 파일의 경우
                        case var _ when tryExtension == SystemMaintenance.BundleManifestFileExt:
                            // 매니피스트 파일의 경우에는 전체 매니피스트 파일을 관리하는 [디렉터리명].manifest 파일과
                            // 에셋번들을 관리하는 [에셋번들명].manifest 파일 2종류가 있고
                            // 
                            // 동일한 환경에서 에셋번들을 생성하더라도 에셋번들 및 매니피스트 파일은 용량이 달라지기 때문에
                            // 에셋번들의 무결성을 검증하기 위해서는 파일의 용량이 아니라
                            // 파일의 갯수, 각 매니피스트 파일 내의 Hash 값을 가지고 검증해야만 한다.
                            //
                            // 이 해쉬값은 에셋번들 매니피스트 파일에만 포함되어 있다.
                            var (valid, readLines) = SystemTool.TryReadLines(tryFilePath);
                            if (valid)
                            {
                                foreach (var line in readLines)
                                {
                                    if (line.ContainSymbol(ManifestHashSeedSymbolSet))
                                    {
                                        hashSeed.Add(line);
                                    }
                                }
                            }

                            break;
                    }
                }
                else
                {
                    fileNameSetLength--;
                }
            }

            // 파일 갯수도 시드 리스트에 넣어준다.
            hashSeed.Add(fileNameSetLength.ToString());
            hashSeed.Sort();
            return SystemTool.GetMD5HashCode(hashSeed);
        }

        #endregion
    }*/
}