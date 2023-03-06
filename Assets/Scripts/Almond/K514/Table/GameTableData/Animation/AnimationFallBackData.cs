using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace k514
{
    public class AnimationFallBackData : GameTable<AnimationFallBackData, AnimatorParamStorage.MotionType, AnimationFallBackData.TableRecord>
    {
        /// <summary>
        /// 지정한 모션이 없어서 FallBack 모션을 재생해야 할 때, 유닛의 상태에 따라 처리해야할 이벤트를
        /// 기술하는 열거형 상수
        /// </summary>
        public enum FallBackFailHandleType
        {
            /// <summary>
            /// 그냥 FallBackMotion을 재생한다.
            /// </summary>
            JustPlay,
                
            /// <summary>
            /// 만약 유닛이 이동중이라면, FallBack으로 전이하지 않고 이전 모션을 계속 재생한다.
            /// </summary>
            KeepPrevMotionWhenMoving,
        }
        
        public class TableRecord : GameTableRecordBase
        {
            public List<MotionFallBackPreset> FallBackSequence { get; private set; }
        }

        protected override async UniTask OnCreated()
        {
            await base.OnCreated();

            var tryTable = GetTable();
            var enumerator = SystemTool.GetEnumEnumerator<AnimatorParamStorage.MotionType>(SystemTool.GetEnumeratorType.GetAll);

            foreach (var motionType in enumerator)
            {
                if (!tryTable.ContainsKey(motionType))
                {
                    await AddRecord(motionType, null);
                }
            }
        }

        protected override string GetDefaultTableFileName()
        {
            return "AnimationFallBackTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public struct MotionFallBackPreset
        {
            public AnimatorParamStorage.MotionType FallBackMotionType;
            public FallBackFailHandleType FallBackFailHandleType;

            public MotionFallBackPreset(AnimatorParamStorage.MotionType p_FallBackMotionType) : this(p_FallBackMotionType, FallBackFailHandleType.JustPlay)
            {
            }
            
            public MotionFallBackPreset(AnimatorParamStorage.MotionType p_FallBackMotionType,
                FallBackFailHandleType p_FallBackFailHandleType)
            {
                FallBackMotionType = p_FallBackMotionType;
                FallBackFailHandleType = p_FallBackFailHandleType;
            }
        }
    }
}