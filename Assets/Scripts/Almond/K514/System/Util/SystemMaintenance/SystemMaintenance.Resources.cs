using System;
using Cysharp.Threading.Tasks;

namespace k514
{
    public partial class SystemMaintenance
    {
        #region <Methods>

        /// <summary>
        /// 지정한 에셋명을 보유한 에셋번들명을 리턴하는 메서드
        /// </summary>
        public static async UniTask<string> FindAssetBundle(this string p_AssetName)
        {
            var tryTable = (await ResourceListData.GetInstance()).GetTable();
            foreach (var tryRecord in tryTable)
            {
                if (p_AssetName == tryRecord.Key)
                {
                    return tryRecord.Value.GetAssetBundleName();
                }
            }
            return String.Empty;
        }

         /// <summary>
        /// 지정한 리소스 타입이 시스템 적으로 수정 불가능한 리소스 로드 타입을 가지는지 검증하는 논리메서드
        /// </summary>
        public static bool IsFixedLoadTypeResource(this ResourceType p_ResourceType)
        {
            switch (p_ResourceType)
            {
                case ResourceType.None:
                case ResourceType.AssetBundle:
                case ResourceType.Dependencies:
                    return true;
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// 지정한 패스가 리소스 타입에 대응하는 디렉터리를 포함하는지 검증하는 논리메서드
        /// </summary>
        public static bool IsResourcePath(ResourceType p_ResourceType, string p_Path)
        {
            return p_Path.Contains(GetSystemResourcePath(AssetLoadType.FromUnityResource, p_ResourceType, PathType.SystemGenerate_AbsolutePath));
        }
        
        /// <summary>
        /// 지정한 패스가 특정 리소스 타입에 대응하는 Dependency 리소스 디렉터리를 포함하는지 검증하여, 해당하는 리소스 타입을 리턴하는 메서드
        /// </summary>
        public static DependencyResourceSubType GetDependencyResourceSubType(string p_Path)
        {
            var dependencyRootPath = GetSystemResourcePath(AssetLoadType.FromUnityResource, ResourceType.Dependencies, PathType.SystemGenerate_AbsolutePath);
            if (p_Path.Contains(dependencyRootPath))
            {
                var rearPath = p_Path.CutString(dependencyRootPath, false, true).CutString("/", true, true);
                var enumerator = SystemTool.GetEnumEnumerator<DependencyResourceSubType>(SystemTool.GetEnumeratorType.GetAll);
                foreach (var subResourceType in enumerator)
                {
                    if (rearPath == subResourceType.ToString())
                    {
                        return subResourceType;
                    }
                }
                return DependencyResourceSubType.Misc;
            }
            else
            {
                return DependencyResourceSubType.None;
            }
        }
        
        /// <summary>
        /// 지정한 패스가 특정 리소스 타입에 대응하는 Dependency 리소스 디렉터리를 포함하는지 검증하여, 해당하는 리소스 타입을 리턴하는 메서드
        /// </summary>
        public static string GetDependencyResourceSubTypeString(string p_Path)
        {
            var dependencyRootPath = GetSystemResourcePath(AssetLoadType.FromUnityResource, ResourceType.Dependencies, PathType.SystemGenerate_AbsolutePath);
            if (p_Path.Contains(dependencyRootPath))
            {
                var rearPath = p_Path.CutString(dependencyRootPath, false, true).CutString("/", true, true);
                var enumerator = SystemTool.GetEnumStringEnumerator<DependencyResourceSubType>(SystemTool.GetEnumeratorType.GetAll);
                foreach (var subResourceType in enumerator)
                {
                    if (rearPath == subResourceType.ToString())
                    {
                        return subResourceType;
                    }
                }
                return string.Empty;
            }
            else
            {
                return null;
            }
        }
        
        #endregion
    }
}