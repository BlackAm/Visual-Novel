#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using System;

namespace BlackAm
{
    public class SystemFlag : EditorModeOnlyGameData<SystemFlag, SystemFlag.SystemFlagType, SystemFlag.SystemFlagRecord>
    {
        public enum SystemFlagType
        {
            /// <summary>
            /// 해당 프로젝트가 배포 플랫폼에서 동작해야 하는 경우 참.
            /// 실제 플랫폼 상에서는 사용하지 않고, 빌드 전에 실제 플랫폼의 환경처럼 프로젝트가 동작하는지
            /// 체크하고, 빌드에 필요없는 리소스 파일을 제거하는 용도로 활용된다.
            /// </summary>
            ReleaseMode,
            
            /// <summary>
            /// 해당 클라이언트가 배포됬을 때의 버전
            /// </summary>
            ReleasedVersion,
            
            /// <summary>
            /// 해당 프로젝트의 개발 모드에서, 자동으로 패치를 수행할지 여부를 결정하는 플래그
            /// 수동으로 패치하는 경우, PatchPackageBuilder를 통해 버전을 선택할 수 있으며,
            /// 자동으로 패치하는 경우, 최근에 배포한 버전으로 버전이 게임 실행시 패치가 진행된다.
            /// </summary>
            AutoPatchMode,
            
            /// <summary>
            /// 배포모드가 아닐 때, 리소스 리스트 테이블이 활성화되면서
            /// 리소스 폴더를 전부 검색하여 리스트를 갱신하도록 하는 플래그
            /// </summary>
            ResourceListAutoUpdate,
            
            /// <summary>
            /// 바이트 코드로부터 테이블을 읽는다. 대응하는 바이트 코드가 없다면, 기존 방식대로 텍스트 파일을 읽는다.
            /// </summary>
            UsingByteByteCode,
        }

        public class SystemFlagRecord : EditorModeOnlyTableRecord
        {
            public bool Flag { get; private set; }
            public int IntValue { get; private set; }
            
            public override async UniTask SetRecord(SystemFlagType p_Key, object[] p_RecordField)
            {
                await base.SetRecord(p_Key, p_RecordField);
                
                Flag = (bool) p_RecordField[0];
                IntValue = (int) p_RecordField[1];
            }
        }

        protected override async UniTask GenerateDefaultRecordSet()
        {
            await base.GenerateDefaultRecordSet();
            
            if (SystemTool.TryGetEnumEnumerator<SystemFlagType>(SystemTool.GetEnumeratorType.GetAll, out var o_Enumerator))
            {
                foreach (var systemFlagType in o_Enumerator)
                {
                    switch (systemFlagType)
                    {
                        case SystemFlagType.ReleaseMode :
                            await AddRecord(systemFlagType, false, 0);
                            break;
                        case SystemFlagType.ReleasedVersion :
                            await AddRecord(systemFlagType, false, 0);
                            break;
                        case SystemFlagType.AutoPatchMode :
                            await AddRecord(systemFlagType, true, 0);
                            break;          
                        case SystemFlagType.ResourceListAutoUpdate :
                            await AddRecord(systemFlagType, false, 0);
                            break;   
                        case SystemFlagType.UsingByteByteCode :
                            await AddRecord(systemFlagType, false, 0);
                            break;   
                    }
                }
            }
        }
        
        protected override async UniTask CheckMissedRecordSet()
        {
            await base.CheckMissedRecordSet();
            
            if (SystemTool.TryGetEnumEnumerator<SystemFlagType>(SystemTool.GetEnumeratorType.GetAll, out var o_Enumerator))
            {
                foreach (var systemFlagType in o_Enumerator)
                {
                    if (!HasKey(systemFlagType))
                    {
                        switch (systemFlagType)
                        {
                            case SystemFlagType.ReleaseMode :
                                await AddRecord(systemFlagType, false, 0);
                                break;
                            case SystemFlagType.ReleasedVersion :
                                await AddRecord(systemFlagType, false, 0);
                                break;
                            case SystemFlagType.AutoPatchMode :
                                await AddRecord(systemFlagType, true, 0);
                                break;          
                            case SystemFlagType.ResourceListAutoUpdate :
                                await AddRecord(systemFlagType, false, 0);
                                break;    
                            case SystemFlagType.UsingByteByteCode :
                                await AddRecord(systemFlagType, false, 0);
                                break;   
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// 시스템 배포모드 플래그 숏컷
        /// </summary>
        public static bool IsSystemReleaseMode()
        {
            return GetInstanceUnSafe?.GetTableData(SystemFlagType.ReleaseMode).Flag ?? false;
        }

        /// <summary>
        /// 시스템 배포버전 숏컷
        /// </summary>
        public static int GetReleasedVersion()
        {
            return GetInstanceUnSafe.GetTableData(SystemFlagType.ReleasedVersion).IntValue;
        }
        
        /// <summary>
        /// 시스템 자동패치 플래그 숏컷
        /// </summary>
        public static bool IsAutoPatchMode()
        {
            return GetInstanceUnSafe.GetTableData(SystemFlagType.AutoPatchMode).Flag;
        }

        /// <summary>
        /// 시스템 테이블 바이트코드 로드 플래그 숏컷
        /// </summary>
        public static bool IsTableByteImageMode()
        {
            return GetInstanceUnSafe?.GetTableData(SystemFlagType.UsingByteByteCode).Flag ?? false;
        }
        
        /// <summary>
        /// 리소스 리스트 자동 갱신 플래그
        /// </summary>
        public static bool GetAutoUpdateResourceListFlag()
        {
            return GetInstanceUnSafe.GetTableData(SystemFlagType.ResourceListAutoUpdate).Flag;
        }

        /// <summary>
        /// 배포모드 시스템 플래그를 지정한 값으로 테이블에 업데이트 시키는 메서드
        /// </summary>
        public async UniTask UpdateSystemFlagReleaseMode(SystemFlagType p_TargetType, bool p_Flag)
        {
            if (!GetTable().ContainsKey(p_TargetType)
                || p_Flag != GetTableData(p_TargetType).Flag)
            {
                await ReplaceRecord(p_TargetType, p_Flag, 0);
                await UpdateTableFile(ExportDataTool.WriteType.Overlap);
            }
        }
        
        /// <summary>
        /// 최신버전 시스템 플래그를 테이블에 업데이트 시키는 메서드
        /// </summary>
        /*public async UniTask UpdateSystemFlagReleasedVersion()
        {
            var _LatestVersion = SystemMaintenance.CalculateLatestBundleVersion();
            
            /* SE Cond #1#
            // 1. 현재 배포모드가 아닌 경우에
            // 2. 테이블의 배포버전과 현재 최신버전이 다른 경우에
            if (!GetTable().ContainsKey(SystemFlagType.ReleasedVersion) 
                || _LatestVersion != GetTableData(SystemFlagType.ReleasedVersion).IntValue)
            {
                await ReplaceRecord(SystemFlagType.ReleasedVersion, false, Math.Max(-1, _LatestVersion));
                await UpdateTableFile(ExportDataTool.WriteType.Overlap);
            }
        }*/

        protected override string GetDefaultTableFileName()
        {
            return "SystemFlag";

        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}
#endif