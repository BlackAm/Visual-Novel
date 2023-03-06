using UnityEngine;

namespace k514
{
    /// <summary>
    /// 애니메이터 유닛 프리팹을 초기화 하는 레코드 테이블
    /// 지정 레코드 정수 인덱스 0 ~ 99999
    /// </summary>
    public class PrefabExtraData_LamiereUnit : PrefabExtraData_UnitBase<PrefabExtraData_LamiereUnit, PrefabExtraData_LamiereUnit.PrefabExtraDataLamiereUnitRecord>, PrefabExtraDataTableBridge
    {
        public class PrefabExtraDataLamiereUnitRecord : PrefabExtraDataUnitBaseRecord
        {
            public int PortraitImage { get; private set; }
            public Vocation Vocation { get; private set; }
            public bool HasMeshChanger { get; private set; }
            public float TargetingEffectScale { get; private set; }
            public Vector3 TargetingEffectPosition { get; private set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "LamiereUnitExtraDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 0;
            EndIndex = 100000;
        }
                
        public override PrefabExtraDataRoot.PrefabExtraDataType GetThisLabelType()
        {
            return PrefabExtraDataRoot.PrefabExtraDataType.Lamiere;
        }
    }
}