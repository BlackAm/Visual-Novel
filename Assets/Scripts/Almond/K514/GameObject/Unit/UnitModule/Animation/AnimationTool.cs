using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace k514
{
    public static class AnimationTool
    {
        #region <Fields>

        public const string __AnimationStartEventName = "OnAnimationStart";
        public const string __AnimationEndEventName = "OnAnimationEnd";

        public static readonly Dictionary<AnimatorParamStorage.MotionClipEventType, string> __MotionClipEventNameTable
            = new Dictionary<AnimatorParamStorage.MotionClipEventType, string>
            {
                {AnimatorParamStorage.MotionClipEventType.Cue, "OnAnimationCue"},
                {AnimatorParamStorage.MotionClipEventType.MotionStop, "OnAnimationMotionStop"}
            };

        public static AnimatorParamStorage.MotionTransitionType[] _MotionTransitionType_Enumerator = SystemTool.GetEnumEnumerator<AnimatorParamStorage.MotionTransitionType>(SystemTool.GetEnumeratorType.ExceptNone);
        public static AnimatorParamStorage.MotionType[] _MotionTypeEnumerator = SystemTool.GetEnumEnumerator<AnimatorParamStorage.MotionType>(SystemTool.GetEnumeratorType.ExceptNone);

        public const float AttackSpeedStatusAdaptFactor = 1f;
        public const float MoveSpeedStatusAdaptFactor = 0.05f;
        public const float AnimationSpeedUpperBound = 4f;
        public const float AnimationSpeedLowerBound = 0f;
        
        #endregion

        #region <Methods>

        public static void AddEventToClip(this AnimationClip p_AnimationClip, string p_FuncName, float p_NormalTime01, float p_CueDurationRate01)
        {
            var targetEvent = new AnimationEvent();
            targetEvent.time = p_AnimationClip.length * p_NormalTime01;
            targetEvent.functionName = p_FuncName;
            targetEvent.floatParameter = p_AnimationClip.length * p_CueDurationRate01;
            p_AnimationClip.AddEvent(targetEvent);
        }

        #endregion
    }

    public class AnimatorParamStorage : Singleton<AnimatorParamStorage>
    {
        #region <Fields>

        private Dictionary<string, AnimationParamsRecord> AnimatorParamsCollection;
        
        #endregion

        #region <Enums>

        [Flags]
        public enum MotionTransitionType
        {
            None = 0,
            
            /// <summary>
            /// 각 모션의 상관 관계에 따라 전이
            /// 예를 들어, 점프 모션 중에 달리기 모션은 할 수 없다던가
            /// </summary>
            Bypass_StateMachine = 1 << 0,
            
            /// <summary>
            /// 각 모션의 상관 관계에 따라 전이의 역
            /// </summary>
            Bypass_InverseStateMachine = 1 << 1,
            
            /// <summary>
            /// 지정한 모션으로 무조건 전이
            /// </summary>
            Restrict = 1 << 2,
            
            /// <summary>
            /// 지정한 모션이 현재 모션과 같은 타입인 경우에만 전이
            /// </summary>
            Restrict_ToSameMotion = 1 << 3,
            
            /// <summary>
            /// 지정한 모션이 현재 모션과 같은 타입인 다른 경우에만 전이
            /// </summary>
            Restrict_ToDiffentMotion = 1 << 4,
            
            
            /// <summary>
            /// 마스크를 논리곱으로 변환시키는 플래그
            /// </summary>
            AndMask = 1 << 10,
            
            /// <summary>
            /// 해당 플래그를 포함시키는 전이의 경우, 현재 애니메이션 정보를 지우고 모션 진입을 시도한다.
            /// </summary>
            Restrict_ErasePrevMotion = 1 << 11,
        }

        public enum MotionType
        {
            None,
            
            RelaxIdle, GroggyIdle, CombatIdle,
            MoveWalk, MoveRun,
            SpecialMove,
            Punch, Kick,
            Hit,
            JumpUp, JumpDown,
            UnWeapon,
            Dead,
        }
        
        public enum MotionClipEventType
        {
            Cue,
            MotionStop,
        }
        
        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
            AnimatorParamsCollection = new Dictionary<string, AnimationParamsRecord>();
        }

        public override void OnInitiate()
        {
        }

        #endregion

        #region <Methods>

        public AnimationParamsRecord GetAnimationMotionParamsRecord(string p_AnimationControllerName)
        {
            if (AnimatorParamsCollection.TryGetValue(p_AnimationControllerName, out var AnimationParamsRecord))
            {
                // 참조 카운트를 갱신시키기 위해, 일단 로드해준다.
                LoadAssetManager.GetInstanceUnSafe.LoadAsset<RuntimeAnimatorController>(
                    ResourceType.Misc, ResourceLifeCycleType.Free_Condition, p_AnimationControllerName);
                return AnimationParamsRecord;
            }
            else
            {
                var resultTuple = 
                    LoadAssetManager.GetInstanceUnSafe
                        .LoadAsset<RuntimeAnimatorController>
                    (
                        ResourceType.Misc, ResourceLifeCycleType.Free_Condition, p_AnimationControllerName
                    );
                var assetPreset = resultTuple.Item1;
                var tryController = resultTuple.Item2;
                
                var _MotionCollection = new Dictionary<MotionType, (AnimationParamsRecord.ClipListType, List<AnimationClipPreset>)>();
                var targetClipGroup = tryController.animationClips;
                var indexPermutaionList = new List<int>();
                AnimationParamsRecord.ClipListType tryClipType = AnimationParamsRecord.ClipListType.Random;
                
                foreach (var motionType in AnimationTool._MotionTypeEnumerator)
                {
                    indexPermutaionList.Clear();
                    var targetAnimationClipList = new List<AnimationClipPreset>();
                    targetAnimationClipList.Clear();
                    foreach (var animationClip in targetClipGroup)
                    {
                        var targetClip = animationClip;
                        // TODO<K514> : 아래의 name 프로퍼티 및 enum ToString 비교에서 8kb 할당이 일어난다.
                        var targetName = targetClip.name;
                        var motionName = motionType.ToString();
                        if (targetName.Contains(motionName))
                        {
                            var parsedIndex = int.Parse(targetName.CutString(motionName, false, false));
                            indexPermutaionList.Add(parsedIndex);
                            targetAnimationClipList.Add(new AnimationClipPreset(motionType, targetClip));

                            if (tryClipType != AnimationParamsRecord.ClipListType.Sequence &&
                                targetName.Contains(AnimationParamsRecord.SequenceClipSymbol))
                            {
                                tryClipType = AnimationParamsRecord.ClipListType.Sequence;
                            }
                        }
                    }
                    
                    // 애니메이터에 추가된 순으로 clip들이 정렬되기 때문에, index 서순으로 재배치해줘야 한다.
                    var assembledClipCount = targetAnimationClipList.Count;
                    for (int i = 0; i < assembledClipCount; i++)
                    {
                        var targetPermulateIndex = indexPermutaionList[i];
                        if (targetPermulateIndex != i)
                        {
                            indexPermutaionList[targetPermulateIndex] = targetPermulateIndex;
                            indexPermutaionList[i] = i;
                            
                            var tmpElement = targetAnimationClipList[targetPermulateIndex];
                            targetAnimationClipList[targetPermulateIndex] = targetAnimationClipList[i];
                            targetAnimationClipList[i] = tmpElement;
                        }
                    }
                    
                    _MotionCollection.Add(
                        motionType, 
                        (tryClipType, targetAnimationClipList)
                    );
                }
                
                var animationParamsRecord = new AnimationParamsRecord(tryController, assetPreset, _MotionCollection);
                AnimatorParamsCollection.Add(p_AnimationControllerName, animationParamsRecord);
                return animationParamsRecord;
            }
        }

        public void DisposeAnimationMotionParamsRecord(AnimationParamsRecord p_Dispose)
        {
            var assetPreset = p_Dispose._AssetPreset;
            AnimatorParamsCollection.Remove(assetPreset.AssetName);
            LoadAssetManager.GetInstanceUnSafe.UnloadAsset(assetPreset);
        }

        #endregion

        #region <Struct>

        public struct AnimationParamsRecord
        {
            #region <Consts>

            public static string SequenceClipSymbol = "Seq";

            #endregion
            
            #region <Fields>

            public RuntimeAnimatorController _AnimationController { get; private set; }
            public AssetPreset _AssetPreset { get; private set; }
            public Dictionary<MotionType, (ClipListType, List<AnimationClipPreset>)> _MotionParams { get; private set; }

            #endregion
            
            #region <Enums>

            /// <summary>
            /// 특정타입의 모션이 여러개 있는 경우, 각 모션의 관계를 기술하는 타입
            /// </summary>
            public enum ClipListType
            {
                /// <summary>
                /// 각 모션 중에 하나가 랜덤으로 발동
                /// </summary>
                Random,
                
                /// <summary>
                /// 각 모션이 순차적으로 발동
                /// </summary>
                Sequence
            }

            #endregion

            #region <Constructors>

            public AnimationParamsRecord(RuntimeAnimatorController p_AnimationController, AssetPreset p_AssetPreset,
                Dictionary<MotionType, (ClipListType, List<AnimationClipPreset>)> p_MotionParams)
            {
                _AnimationController = p_AnimationController;
                _AssetPreset = p_AssetPreset;
                _MotionParams = p_MotionParams;
            }

            #endregion

            #region <Methods>

            public bool HasMotion(MotionType p_TargetMotion)
            {
                return HasMotion(p_TargetMotion, 0);
            }

            public bool HasMotion(MotionType p_TargetMotion, int p_Index)
            {
                var targetMotionCollection = _MotionParams;
                return p_Index > -1 && targetMotionCollection.ContainsKey(p_TargetMotion) &&
                       targetMotionCollection[p_TargetMotion].Item2.Count > p_Index;
            }

            public int GetRandomMotionIndex(MotionType p_TargetMotion)
            {
                var targetMotionCollection = _MotionParams;
                var clipType = targetMotionCollection[p_TargetMotion].Item1;
                switch (clipType)
                {
                    case ClipListType.Random:
                        var motionSeq = targetMotionCollection[p_TargetMotion].Item2;
                        return Random.Range(0, motionSeq.Count);
                    case ClipListType.Sequence:
                    default:
                        return 0;
                }
            }

            public int GetRelaxMotionIndex(ControlAnimator controlAnimator, int p_Index)
            {
                if (p_Index < 15)
                {
                    controlAnimator._RelaxMotionCount++;
                    return 0;
                }
                else
                {
                    controlAnimator._RelaxMotionCount = 0;
                    return 1;
                }
            }

            public MotionType GetFallBackMotionType(MotionType p_Key, Unit p_Unit)
            {
                var fallBackList = AnimationFallBackData.GetInstanceUnSafe.GetTableData(p_Key).FallBackSequence;
                if (fallBackList == null)
                {
                    return MotionType.None;
                }
                else
                {
                    foreach (var fallback in fallBackList)
                    {
                        var fallbackMotion = fallback.FallBackMotionType;
                        if (HasMotion(fallbackMotion))
                        {
                            switch (fallback.FallBackFailHandleType)
                            {
                                // 해당 fallback 모션을 가지고 있다면, 바로 전이
                                case AnimationFallBackData.FallBackFailHandleType.JustPlay:
                                    return fallbackMotion;
                                // 현재 움직이는 상태라면 이전 모션을 유지. 정지 상태라면 fallback으로 전이
                                case AnimationFallBackData.FallBackFailHandleType.KeepPrevMotionWhenMoving:
                                    if (p_Unit._ActableObject.IsIdleState())
                                    {
                                        return fallbackMotion;
                                    }
                                    break;
                            }
                        }
                    }
                    return MotionType.None;
                }
            }
            
            public bool HasFallBackMotionType(MotionType p_Key, MotionType p_Fallback)
            {
                var fallBackList = AnimationFallBackData.GetInstanceUnSafe.GetTableData(p_Key).FallBackSequence;
                if (fallBackList == null)
                {
                    return false;
                }
                else
                {
                    foreach (var fallback in fallBackList)
                    {
                        if (fallback.FallBackMotionType == p_Fallback)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            
            #endregion
        }

        public struct AnimationClipPreset
        {
            #region <Fields>
            
            public MotionType MotionType { get; private set; }

            public MotionPresetData.TableRecord MotionPresetRecord;
            
            public AnimationClip AnimationClip { get; private set; }
            
            #endregion

            #region <Constructors>

            public AnimationClipPreset(MotionType p_MotionType, AnimationClip p_AnimationClip)
            {
                MotionType = p_MotionType;
                AnimationClip = p_AnimationClip;
                AnimationClip.events = null;
                
                var motionClipPresetTable = MotionPresetData.GetInstanceUnSafe.GetTable();
                var targetAnimationClip = AnimationClip;
                var targetAnimationClipName = AnimationClip.name;
                
                if (motionClipPresetTable.TryGetValue(targetAnimationClipName, out MotionPresetRecord))
                {
                    var targetMotionClipEventList = MotionPresetRecord.MotionPresetList;
                    if (targetMotionClipEventList.CheckGenericCollectionSafe())
                    {
                        string functionName = null;
                        float functionTimeRate = 0f;
                        var onceFlag = false;
                        foreach (var targetMotionClipEvent in targetMotionClipEventList)
                        {
                            var _functionName =
                                AnimationTool.__MotionClipEventNameTable[targetMotionClipEvent._MotionClipEventType];
                            var _functionTimeRate = targetMotionClipEvent.AnimationClipEventTimeRate01;
                            if (!onceFlag)
                            {
                                onceFlag = true;
                                targetAnimationClip.AddEventToClip(AnimationTool.__AnimationStartEventName, 0f, _functionTimeRate);
                            }
                            else
                            {
                                targetAnimationClip.AddEventToClip(functionName, functionTimeRate, _functionTimeRate - functionTimeRate);
                            }

                            functionName = _functionName;
                            functionTimeRate = _functionTimeRate;
                        }
                        
                        targetAnimationClip.AddEventToClip(functionName, functionTimeRate, 1f - functionTimeRate);
                    }
                    else
                    {
                        targetAnimationClip.AddEventToClip(AnimationTool.__AnimationStartEventName, 0f, 1f);
                    }
                }
                else
                {
                    targetAnimationClip.AddEventToClip(AnimationTool.__AnimationStartEventName, 0f, 1f);
                }
                
                targetAnimationClip.AddEventToClip(AnimationTool.__AnimationEndEventName, 1f, 0f);
            }

            #endregion

            #region <Methods>

#if UNITY_EDITOR
            public override string ToString()
            {
                return AnimationClip == null
                    ? " - Null"
                    : $" - Clip Name : [{AnimationClip.name}]\n - Clip Length : [{AnimationClip.length}]\n - Loop : [{AnimationClip.isLooping}]";
            }
#endif

            #endregion
        }

        #endregion
    }
}