using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public class UnitHitExtraData : GameTable<UnitHitExtraData, int, UnitHitExtraData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public UnitHitTool.HitExtraAttributeType HitExtraAttributeType;
            
            /// <summary>
            /// 해당 데미지의 속성
            /// </summary>
            public UnitTool.UnitAttributeType StrikeAttributeType;
                    
            /// <summary>
            /// 해당 타격이 포착할 수 있는 최대 숫자
            /// </summary>
            public int MaxHitCount { get; private set; }
                        
            /// <summary>
            /// 해당 타격이 같은 유닛을 타격할 수 있는 최대 횟수 및 타격 간격
            /// </summary>
            public (int t_Count, float t_Interval) MaxSameUnitHitPreset { get; private set; }
            
            /// <summary>
            /// 해당 타격이 몇 번 반복되는지
            /// </summary>
            public int ExtraHitCount { get; private set; }
            
            /// <summary>
            /// 추가타격의 간격
            /// </summary>
            public uint ExtraHitInterval { get; private set; }

            /// <summary>
            /// HitManager의 HitBasisTimeUnit 단위로 해당 타격판정이 몇 번 지속될지 기술하는 필드
            /// </summary>
            public int HitBackBlastCount;

            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();
                        
                MaxHitCount = Mathf.Max(1, MaxHitCount);
                MaxSameUnitHitPreset = (Mathf.Max(1, MaxSameUnitHitPreset.t_Count), Mathf.Max(0f, MaxSameUnitHitPreset.t_Interval));
                
                ExtraHitCount = Mathf.Max(0, ExtraHitCount);
                if (ExtraHitCount > 0)
                {
                    HitExtraAttributeType.AddFlag(UnitHitTool.HitExtraAttributeType.ExtraHit);
                }

                if (ExtraHitInterval > 0)
                {
                    HitExtraAttributeType.AddFlag(UnitHitTool.HitExtraAttributeType.HitStorm);
                }
            }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitHitExtraTable";
        }
    }
}