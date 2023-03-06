using System.Collections.Generic;

namespace k514
{
    /// <summary>
    /// 모션 클립의 지정한 타임 레이트(TimeRate01)에 특정 타입의 콜백 타임스탬프를 추가하는 테이블
    /// </summary>
    public class MotionPresetData : GameTable<MotionPresetData, string, MotionPresetData.TableRecord>
    {
        public enum MotionPlaceType
        {
            /// <summary>
            /// 루트모션의 영향을 받지 않음
            /// </summary>
            None,
            
            /// <summary>
            /// 루트모션 노드(애니메이터 컴포넌트)가 모션에 의해 아핀변환을 적용받는 모션
            /// </summary>
            InPlaceRootMotion,
            
            /// <summary>
            /// 모션의 Y 평행이동 아핀변환을 제외한 나머지 변환을 적용받는 모션 
            /// </summary>
            InPlaceRootMotionExceptY,
            
            /// <summary>
            /// 루트모션 노드(애니메이터 컴포넌트)가 모션에 의해 평행이동 변환만을 적용받는 모션
            /// </summary>
            InPlaceRootMotionPositionOnly,
                        
            /// <summary>
            /// 루트모션 노드(애니메이터 컴포넌트)가 모션에 의해 회전 변환만을 적용받는 모션
            /// </summary>
            InPlaceRootMotionRotationOnly,
        }

        public class TableRecord : GameTableRecordBase
        {
            public List<UnitActionTool.MotionClipEventPreset> MotionPresetList { get; private set; }
            public MotionPlaceType MotionPlaceType { get; private set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "MotionPresetTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}