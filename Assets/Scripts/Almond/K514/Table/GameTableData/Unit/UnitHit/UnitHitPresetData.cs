using System;
using Cysharp.Threading.Tasks;

namespace k514
{
    /// <summary>
    /// 유닛 타격 판정을 기술하는 테이블 클래스
    /// </summary>
    public class UnitHitPresetData : GameTable<UnitHitPresetData, int, UnitHitPresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            /// <summary>
            /// 유닛 타격 메시지
            /// </summary>
            private HitMessage HitMessage;

            public AnimatorParamStorage.MotionType MotionType;
            
            /// <summary>
            /// 타격할 유닛을 검색할 필터 플래그 마스크
            /// </summary>
            public UnitFilterTool.UnitFilterFlagType FilterFlag { get; private set; }
            
            /// <summary>
            /// 타격할 유닛을 검색할 범위 등을 가지는 프리셋
            /// </summary>
            public FilterParams FilterParams { get; private set; }

            public int HitParameterIndex { get; private set; }

            public HitMessage GetHitMessage() => HitMessage;
            
            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();
                
                HitMessage.SetHitPresetInfo(KEY);
            }
        }

        protected override async UniTask OnCreated()
        {
            await base.OnCreated();

            // 0 키가 없는 경우, 데미지 및 타격 판정이 없는 기본 레코드를 등록시켜준다.
            if (!HasKey(0))
            {
                await AddRecord(0, new TableRecord());
            }
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitHitPresetTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}