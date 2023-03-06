using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;

namespace k514
{
    public partial class TableBase<M, K, T>
    {
        #region <Method/Path>

        /// <summary>
        /// 지정한 절대 경로에 해당 테이블 클래스의 xml 파일이 존재하는지 검증하는 메서드
        /// </summary>
        public bool HasTableCollectionFromAbsolutePath(string p_TableAbsolutePath)
        {
            return Directory.Exists(p_TableAbsolutePath) && File.Exists(p_TableAbsolutePath + GetTableFileName(TableTool.TableNameType.Alter, true));
        }

#if UNITY_EDITOR
        /// <summary>
        /// 현재 해당 테이블 파일의 풀패스를 리턴하는 메서드
        ///
        /// ResourceList를 먼저 참조하여 해당 테이블 파일명을 검색하여, 해당하는 경로를 리턴한다.
        /// 
        /// 만약 해당 테이블 파일명이 ResourceList에 포함되어 있지 않는 경우
        /// 파라미터로 받은 에셋로드타입을 기준으로 경로를 생성하여 리턴한다.
        /// </summary>
        public string GetTableFileFullPath_RefResourceList(TableTool.TableNameType p_NameType, AssetLoadType p_FallbackAssetLoadType)
        {
            var tableName = GetTableFileName(p_NameType, true);
            var targetTable = ResourceListData.GetInstanceUnSafe?.GetTable();
            if (targetTable?.ContainsKey(tableName) ?? false)
            {
                return targetTable[tableName].GetResourceFullPath();
            }
            else
            {
                return GetTableFileFullPath(p_FallbackAssetLoadType, PathType.SystemGenerate_AbsolutePath, p_NameType, true);
            }
        }
        
        /// <summary>
        /// 현재 해당 테이블 파일을 포함하는 디렉터리를 리턴하는 메서드
        /// 
        /// ResourceList를 먼저 참조하여 해당 테이블 파일명을 검색하여, 해당하는 경로를 리턴한다.
        /// 
        /// 만약 해당 테이블 파일명이 ResourceList에 포함되어 있지 않는 경우
        /// 파라미터로 받은 에셋로드타입을 기준으로 경로를 생성하여 리턴한다.
        /// </summary>
        public string GetTableFileRootPath_RefResourceList(TableTool.TableNameType p_NameType, AssetLoadType p_FallbackAssetLoadType)
        {
            var tableName = GetTableFileName(p_NameType, true);
            var targetTable = ResourceListData.GetInstanceUnSafe?.GetTable();
            if (targetTable?.ContainsKey(tableName) ?? false)
            {
                return targetTable[tableName].GetResourceFullPath().CutString(tableName, true, false);
            }
            else
            {
                return GetTableFileRootPath(p_FallbackAssetLoadType, PathType.SystemGenerate_AbsolutePath);
            }
        }
#endif

        /// <summary>
        /// 해당 테이블의 루트 패스에 xml파일명을 더한 에셋 타입별 기본 풀패스를 리턴하는 메서드
        /// </summary>
        public string GetTableFileFullPath(AssetLoadType p_AssetLoadType, PathType p_PathType, TableTool.TableNameType p_NameType, bool p_AttachExt)
        {
            return GetTableFileRootPath(p_AssetLoadType, p_PathType) + GetTableFileName(p_NameType, p_AttachExt);
        }
        
        /// <summary>
        /// 해당 테이블이 참조하는 xml파일의 에셋 타입별 기본 루트 패스를 리턴하는 메서드
        /// </summary>
        public string GetTableFileRootPath(AssetLoadType p_AssetLoadType, PathType p_PathType)
        {
            return GetTableFileRootPath(p_AssetLoadType, p_PathType, GetTableFileType());
        }
        
        /// <summary>
        /// 해당 테이블이 참조하는 xml파일의 에셋 타입별 기본 루트 패스를 리턴하는 메서드
        /// </summary>
        public string GetTableFileRootPath(AssetLoadType p_AssetLoadType, PathType p_PathType, TableTool.TableFileType p_TableFileType)
        {
            if (_BranchNameTuple.Item1)
            {
                return TableTool.GetTablePath(p_AssetLoadType, p_PathType, TableType, p_TableFileType) + _BranchNameTuple.Item2;
            }
            else
            {
                return TableTool.GetTablePath(p_AssetLoadType, p_PathType, TableType, p_TableFileType);
            }
        }
        
        #endregion
    }
}