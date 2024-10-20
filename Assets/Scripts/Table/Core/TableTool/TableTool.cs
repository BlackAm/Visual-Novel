using System;
using System.Linq;
using static System.String;

namespace BlackAm
{
    public static class TableTool
    {
        #region <Consts>
        
        /// <summary>
        /// 테이블 데이터 클래스의 키 필드 명
        /// </summary>
        public const string TableKeyFieldName = "KEY";

        /// <summary>
        /// 프로퍼티에 의해 가려진 내부 필드 이름 규칙 명
        /// </summary>
        public const string BackingFieldRear = "BackingField";
        
        /// <summary>
        /// xml 테이블 디렉터리 중간 브랜치명
        /// </summary>
        public static readonly string XML_PATH = $"{TableFileType.Xml}/";

        /// <summary>
        /// xml 테이블 파일 확장자
        /// </summary>
        public const string XML_EXT = ".xml";

        /// <summary>
        /// json 테이블 디렉터리 중간 브랜치명
        /// </summary>
        public static readonly string JSON_PATH = $"{TableFileType.JSON}/";

        /// <summary>
        /// json 테이블 파일 확장자
        /// </summary>
        public const string JSON_EXT = ".json";
        
        /// <summary>
        /// 바이트 테이블 디렉터리 중간 브랜치명
        /// </summary>
        public static readonly string BYTES_PATH = $"{TableFileType.Bytes}/";

        /// <summary>
        /// 바이트 파일 확장자
        /// </summary>
        public const string BYTES_EXT = ".bytes";

        public static readonly string[] TableSystemBranchGroup = 
            SystemTool.GetEnumEnumerator<TableFileType>(SystemTool.GetEnumeratorType.GetAll)
            .Select(type => $"{ResourceType.Table}/{type}/{TableType.SystemTable}/").ToArray();
        
        #endregion
        
        #region <Enums>

        public enum TableType
        {
            WholeGameTable,
            SceneGameTable,
            EditorOnlyTable,
            SystemTable,
        }

        public enum TableFileType
        {
            Xml,
            JSON,
            Bytes,
        }

        public enum TableNameType
        {
            Default,
            Alter,
        }

        public enum TableSerializeType
        {
            SerializeObjects,
            SerializeString,
            NoneSerialize,
        }

        #endregion

        #region <Methods>

        public static bool IsSystemTablePath(this string p_TargetPath)
        {
            foreach (var branch in TableSystemBranchGroup)
            {
                if (p_TargetPath.StartsWith(branch))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsUnityResourceTable(this ITableBase p_Table)
        {
            switch (p_Table.TableType)
            {
                case TableType.WholeGameTable:
                case TableType.SceneGameTable:
                    return ResourceType.Table.GetResourceLoadType() == AssetLoadType.FromUnityResource;;
                default:
                case TableType.EditorOnlyTable:
                case TableType.SystemTable:
                    return true;
            }
        }
        
        public static AssetLoadType GetAssetLoadType(this ITableBase p_Table)
        {
            return p_Table.IsUnityResourceTable() ? AssetLoadType.FromUnityResource : AssetLoadType.FromAssetBundle;
        }

        /// <summary>
        /// 테이블 타입에 따른 파일 확장자를 리턴하는 메서드
        /// </summary>
        public static string GetTableExtension(this TableFileType p_Type)
        {
            switch (p_Type)
            {
                case TableFileType.Xml :
                    return XML_EXT;
                case TableFileType.JSON :
                    return JSON_EXT;
                case TableFileType.Bytes :
                    return BYTES_EXT;
            }
            return Empty;
        }

        public static string GetTablePath(AssetLoadType p_AssetLoadType, PathType p_PathType, TableType p_TableType, TableFileType p_TableFileType)
        {
            var headBranch = Empty;
            switch (p_TableType)
            {
                case TableType.WholeGameTable:
                case TableType.SceneGameTable:
                    headBranch = SystemMaintenance.GetSystemResourcePath(p_AssetLoadType, ResourceType.Table, p_PathType);
                    break;
                case TableType.EditorOnlyTable:
                    switch (p_PathType)
                    {
                        case PathType.SystemGenerate_AbsolutePath:
                            headBranch = SystemMaintenance.EditorOnlyAssetAbsolutePath + $"{ResourceType.Table}/";
                            break;
                        case PathType.SystemGenerate_RelativePath:
                            headBranch = SystemMaintenance.EditorOnlyResourceDirectory + $"{ResourceType.Table}/";
                            break;
                    }
                    break;
                case TableType.SystemTable:
                    switch (p_PathType)
                    {
                        case PathType.SystemGenerate_AbsolutePath:
                            headBranch = SystemMaintenance.UnityResourceAbsolutePath + SystemMaintenance.GetDependencyResourcePathBranch(DependencyResourceSubType.SystemTable);
                            break;
                        case PathType.SystemGenerate_RelativePath:
                            headBranch = SystemMaintenance.GetDependencyResourcePathBranch(DependencyResourceSubType.SystemTable);
                            break;
                    }
                    break;
            }
            
            switch (p_TableFileType)
            {
                case TableFileType.Xml :
                    return headBranch + XML_PATH;
                case TableFileType.JSON :
                    return headBranch + JSON_PATH;
                case TableFileType.Bytes :
                    return headBranch + BYTES_PATH;
            }

            return Empty;
        }

        #endregion
    }
}