using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// State-Constant한 ObjectDeployEventPreset 클래스의 상태를 추적하기 위한 종속 클래스
    /// </summary>
    public partial class ObjectDeployEventRecord : PoolingObject<ObjectDeployEventRecord>, IUniTaskDelayPredicate
    {
        #region <Consts>

        private const int MaxConcurrentEventSize = 16;

        #endregion
        
        #region <Fields>

        /// <summary>
        /// 타겟 유닛
        /// </summary>
        public Unit _MasterNode;

        /// <summary>
        /// 배치 기준이 되는 아핀 프리셋
        /// </summary>
        public TransformTool.AffineCachePreset _ZeroPivotAffine;

        /// <summary>
        /// 수동으로 아핀 프리셋이 지정되었는지 표시하는 플래그
        /// </summary>
        private bool _ManualZeroPivotSetted;
        
        /// <summary>
        /// 연산된 배치 프리셋 리스트
        /// </summary>
        public List<ObjectDeployEventExtraPreset> _DeployPresetStorage;

        /// <summary>
        /// 하나의 타임 스탬프에 다수의 이벤트가 처리되는데, 각 이벤트의 진행 상태를 기술하는 배열
        /// </summary>
        private DeployEventState[] DeployStateStorage;

        /// <summary>
        /// 음원 미디어 트래커
        /// </summary>
        private MediaTool.MediaPreset<AudioClip> _AudioClipMediaTracker;
        
        /* 현재 진행중인 이벤트 정보를 레코드의 ObjectDeployEventPreset로부터 복사해온 필드 목록 */
        private Dictionary<int, ObjectDeployEventProgressControl> _TimeControlUnitCollection;
        private Dictionary<int, List<(int t_DeployDataIndex, Dictionary<ObjectDeployTool.DeployableType, List<int>> t_EventCollection)>> _EventDeployCollection;
        private int _ConcurrentTracingEventCount;
        public ObjectDeployParams _ObjectDeployParams;
        public bool _DiverBackFlag;
        public bool _BlockBusterFlag;
        public bool _UsingPivotFlag;
        public bool _UsingSharingHitFlag;
        public bool _UpdateRecursiveStartPositionFlag;
        public bool _BlockEventWheneNoTargetFlag;
        public bool _BlockFallBackCurrentFocus;
        
        #endregion

        #region <Enums>

        public enum DeploySequenceState
        {
            Valid,
            ReserveLastDeployEvent,
            Blocked
        }

        #endregion

        #region <Callbacks>
        
        public override void OnSpawning()
        {
            _DeployPresetStorage = new List<ObjectDeployEventExtraPreset>();
            DeployStateStorage = 
                Enumerable.Range(0, MaxConcurrentEventSize)
                    .Select(index => DeployEventState.GetDefaultEventState()).ToArray();
        }

        public override void OnPooling()
        {
            _MasterNode = default;
            _ZeroPivotAffine = TransformTool.AffineCachePreset.GetDefaultAffineCachePreset();
            _ManualZeroPivotSetted = false;

            for (int i = 0; i < MaxConcurrentEventSize; i++)
            {
                DeployStateStorage[i] = default;
            }

            _TimeControlUnitCollection = default;
            _EventDeployCollection = default;
            _ConcurrentTracingEventCount = default;
            _ObjectDeployParams = default;
            
            _DiverBackFlag = default;
            _BlockBusterFlag = default;
            _UsingPivotFlag = default;
            _UsingSharingHitFlag = default;
            _UpdateRecursiveStartPositionFlag = default;
            _BlockEventWheneNoTargetFlag = default;
            _BlockFallBackCurrentFocus = default;
        }

        public override void OnRetrieved()
        {
            foreach (var deployEventExtraPreset in _DeployPresetStorage)
            {
                deployEventExtraPreset.OnRetrievePreset();
            }
            _DeployPresetStorage.Clear();
            
            SetUnit(default);
            ResetZeroAffine();
            SetAudioClipPreset(default);
        }

        public void InitializeRecord(ObjectDeployEventPreset p_DeployPreset)
        {
            _DeployPresetStorage.Clear();
            for (int i = 0; i < MaxConcurrentEventSize; i++)
            {
                DeployStateStorage[i] = DeployEventState.GetDefaultEventState();
            }

            _ObjectDeployParams = p_DeployPreset._ObjectDeployParams;

            _EventDeployCollection = p_DeployPreset._EventDeployCollection;
            _ConcurrentTracingEventCount = p_DeployPreset._EventSequenceCount;
            _TimeControlUnitCollection = p_DeployPreset._TimeControlUnitCollection;
            
            _BlockBusterFlag = _ObjectDeployParams.EventDeployFlag.HasAnyFlagExceptNone(ObjectDeployEventPreset.EventDeployFlag.BlockBuster);
            _DiverBackFlag = _ObjectDeployParams.EventDeployFlag.HasAnyFlagExceptNone(ObjectDeployEventPreset.EventDeployFlag.DiverBack);
            _UsingPivotFlag = _ObjectDeployParams.EventDeployFlag.HasAnyFlagExceptNone(ObjectDeployEventPreset.EventDeployFlag.UsingZeroTimePivot);
            _UsingSharingHitFlag = _ObjectDeployParams.EventDeployFlag.HasAnyFlagExceptNone(ObjectDeployEventPreset.EventDeployFlag.SharedHitState);
            _UpdateRecursiveStartPositionFlag = _ObjectDeployParams.EventDeployFlag.HasAnyFlagExceptNone(ObjectDeployEventPreset.EventDeployFlag.UpdateRecursiveStartPosition);
            _BlockEventWheneNoTargetFlag = _ObjectDeployParams.EventDeployFlag.HasAnyFlagExceptNone(ObjectDeployEventPreset.EventDeployFlag.BlockEventWhenNoTarget);
            _BlockFallBackCurrentFocus = _ObjectDeployParams.EventDeployFlag.HasAnyFlagExceptNone(ObjectDeployEventPreset.EventDeployFlag.BlockFallBackCurrentFocus);

            SetAudioClipPreset(default);
        }

        public void InitializeStartPivot()
        {
            if (_MasterNode.IsValid())
            {
                if (!_ManualZeroPivotSetted)
                {
                    _ZeroPivotAffine = _MasterNode.GetAttachPoint(_ObjectDeployParams.SpawnAttachPoint);
                }
                _ZeroPivotAffine.SetScaleFactor(_MasterNode.ObjectScale.CurrentValue); 
            }
        }
        
        public void InitializeStartPivot(ObjectDeployEventExtraPreset p_ObjectDeployEventExtraPreset)
        {
            if (p_ObjectDeployEventExtraPreset.IsRecursiveEntry())
            {
                _ZeroPivotAffine = p_ObjectDeployEventExtraPreset.StartPivot;
            }
            else
            {
                InitializeStartPivot();
            }
        }

        #endregion
        
        #region <Methods>

        public void SetUnit(Unit p_Master)
        {
            _MasterNode = p_Master;
        }
        
        public void SetZeroAffine(TransformTool.AffineCachePreset p_Affine)
        {
            _ManualZeroPivotSetted = true;
            _ZeroPivotAffine = p_Affine;
        }
        
        public void ResetZeroAffine()
        {
            _ManualZeroPivotSetted = false;
            _ZeroPivotAffine = default;
        }
        
        private void SetAudioClipPreset(MediaTool.MediaPreset<AudioClip> p_MediaPreset)
        {
//            _AudioClipMediaTracker.Dispose();
//            _AudioClipMediaTracker = p_MediaPreset;
        }

        private float GetProgressRate(int p_DeployIndex, int p_CurrentTimeStamp)
        {
            return _TimeControlUnitCollection[p_DeployIndex].GetProgressRate(p_CurrentTimeStamp);
        }

        public void DeployUnitActionEvent(int p_DeployMapIndex, ObjectDeployEventExtraPreset p_ObjectDeployEventExtraPreset)
        {
            DeployUnitActionEvent(ObjectDeployPresetMapData.GetInstanceUnSafe[p_DeployMapIndex].DeployEventPresetMap, p_ObjectDeployEventExtraPreset);
        }

        public async void DeployUnitActionEvent(ObjectDeployEventPreset p_DeployPreset, ObjectDeployEventExtraPreset p_ObjectDeployEventExtraPreset)
        {
            if (p_DeployPreset._ObjectDeployParams.EventDeployFlag.HasAnyFlagExceptNone(ObjectDeployEventPreset.EventDeployFlag.UsingZeroTimePivot))
            {
                CalculateAllFrameDeployAffine(p_DeployPreset, p_ObjectDeployEventExtraPreset);
            }
            else
            {
                InitializeRecord(p_DeployPreset);
                InitializeStartPivot(p_ObjectDeployEventExtraPreset);
            
                if (!p_ObjectDeployEventExtraPreset.OnCheckEventEnterable(this))
                {
                    return;
                }
                
                for (int i = 0; i < _ConcurrentTracingEventCount; i++)
                {
                    _DeployPresetStorage.Add(p_ObjectDeployEventExtraPreset);
                }
            }

            // 초기화된 필드를 기반으로 배치를 수행한다.
            await CastDeployEvent();
            
            // 배치 작업이 끝났다면 해당 인스턴스를 릴리스한다.
            RetrieveObject();
        }

        public async UniTask CastDeployEvent()
        {
            if (_EventDeployCollection == null) return;
            
            var preDelay = 0;
            var deployCount = 0;
            
            foreach (var deployCollectionKV in _EventDeployCollection)
            {
                // 지연된 타임 유닛 = 현재 이벤트 타임 스탬프 - 이전 이벤트 타임 스탬프
                // 해당 시간만큼 지연된 시간 이후에 현재 루프 분의 이벤트를 동작시킨다.
                var tryDelay = deployCollectionKV.Key;
                if (tryDelay > 0)
                {
                    await UniTask.Delay(tryDelay - preDelay);
                    if (CheckDelayActionValid())
                    {
                        preDelay = tryDelay;
                    }
                    else
                    {
                        return;
                    }
                }
                
                // 지정한 이벤트 컬렉션으로부터 이벤트를 배치한다.
                var currentDeployTimeStamp = deployCollectionKV.Key;
                var eventCollectionTupleList = deployCollectionKV.Value;

                for (int j = 0; j < _ConcurrentTracingEventCount; j++)
                {
                    switch (DeployStateStorage[j].DeploySequenceState)
                    {
                        case DeploySequenceState.Valid:
                        {
                            var eventCollectionTuple = eventCollectionTupleList[j];
                            DeployEvent(deployCount, j, eventCollectionTuple.t_DeployDataIndex, currentDeployTimeStamp, eventCollectionTuple.t_EventCollection);
                        }
                            break;
                        case DeploySequenceState.ReserveLastDeployEvent:
                        {
                            var lastEventCollectionTuple = _EventDeployCollection.Last().Value[j];
                            DeployEvent(deployCount - 1, j, lastEventCollectionTuple.t_DeployDataIndex, currentDeployTimeStamp, lastEventCollectionTuple.t_EventCollection);
                            DeployStateStorage[j].DeploySequenceState = DeploySequenceState.Blocked;
                        }
                            break;
                        case DeploySequenceState.Blocked:
                            break;
                    }
                }
                
                deployCount++;
            }
        }

        private ObjectDeployEventExtraPreset DeployEvent(int p_DeploySequence, int p_EventSequence, int p_DeployIndex, int p_DeployTimeStamp, Dictionary<ObjectDeployTool.DeployableType, List<int>> p_EventCollection)
        {
            var isFirstLoop = p_DeploySequence < 1;
            var deployRecord = ObjectDeployDataRoot.GetInstanceUnSafe[p_DeployIndex];
            var progressRate = GetProgressRate(p_DeployIndex, p_DeployTimeStamp);
            ObjectDeployEventExtraPreset result;
            ObjectDeployEventExtraPreset prev;
            TransformTool.AffineCachePreset resultAffinePreset;
            TransformTool.AffineCachePreset prevAffinePreset;

            // 아핀 연산을 수행한다.
            if (_UsingPivotFlag)
            {
                prev = isFirstLoop
                    ? _DeployPresetStorage[p_EventSequence]
                    : _DeployPresetStorage[(p_DeploySequence - 1) * _ConcurrentTracingEventCount + p_EventSequence];
                prevAffinePreset = prev.CurrentPivot;
                prev.ProgressRate01 = progressRate;
                result = _DeployPresetStorage[p_DeploySequence * _ConcurrentTracingEventCount + p_EventSequence];
            }
            else
            {
                prev = _DeployPresetStorage[p_EventSequence];
                prevAffinePreset = prev.CurrentPivot;
                prev.ProgressRate01 = progressRate;
                result = deployRecord.CalculateDeployAffine(prev);
            }

            resultAffinePreset = result.CurrentPivot;
            // 이벤트 진입 시점 ~ 이벤트 배치 시점 사이에 기준 유닛이 움직여서 실제 좌표나 회전값에 변화량이 생긴 경우
            // 해당 변화값을 처리하는 세그먼트
            switch (_ObjectDeployParams.AffineDeltaHandleType)
            {
                // 딱히 연산 결과 값을 보정하지 않는다.
                case ObjectDeployEventPreset.AffineDeltaHandleType.None:
                    break;
                // 아핀 변화량은 첫 연산시에만 적용된다.
                case ObjectDeployEventPreset.AffineDeltaHandleType.ApplyAffineOnlyFirst:
                {
                    if (result._IsLerpAffine)
                    {
                        goto case ObjectDeployEventPreset.AffineDeltaHandleType.ApplyAffineToLerp;
                    }
                    else
                    {
                        // 해당 연산이 배치 이벤트 First 연산이었던 경우에만 동작한다.
                        if (result._IsEnterFirstCalculate)
                        {
                            // deployAffine에 OriginPivot을 현재 프레임의 값으로 갱신한 값을 세트하고 아핀 변화량을 적용한다.
                            // 매 프레임 아핀 값이 갱신되므로 항상 회전도 및 좌표가 동기화된다.
                            var deployDelta = resultAffinePreset.GetAffineDelta(result.OriginPivot);
                            resultAffinePreset = result.OriginPivot;
                            resultAffinePreset.SyncPosition();
                            resultAffinePreset.ApplyAffineDelta(deployDelta);
                            
                            // 배치 아핀 값을 결과 아핀 값에 반영하여, 다음 프레임에서 배치 아핀값을 기준으로 연산을 하도록
                            // 해준다.
                            result.CurrentPivot = resultAffinePreset;
                        }
                    }
                }
                    break;
                // 매 프레임, 아핀 변화량의 회전값만 적용시켜준다.
                case ObjectDeployEventPreset.AffineDeltaHandleType.ApplyAffineWithFirstRotation:
                {
                    if (result._IsLerpAffine)
                    {
                        goto case ObjectDeployEventPreset.AffineDeltaHandleType.ApplyAffineToLerp;
                    }
                    else
                    {
                        var deployDelta = resultAffinePreset.GetAffineDelta(result.OriginPivot);
                        resultAffinePreset = result.OriginPivot;
                        resultAffinePreset.SyncPosition();
                        resultAffinePreset.ApplyAffineDelta(deployDelta);
                    }
                }
                    break;
                // 매 프레임, 아핀 변화량을 적용시켜준다.
                case ObjectDeployEventPreset.AffineDeltaHandleType.ApplyAffine:
                {
                    if (result._IsLerpAffine)
                    {
                        goto case ObjectDeployEventPreset.AffineDeltaHandleType.ApplyAffineToLerp;
                    }
                    else
                    {
                        var deployDelta = resultAffinePreset.GetAffineDelta(result.OriginPivot);
                        resultAffinePreset = result.OriginPivot;
                        resultAffinePreset.SyncAffine();
                        resultAffinePreset.ApplyAffineDelta(deployDelta);
                    }
                }
                    break;
                // 해당 이벤트의 배치 타입이 러프 계열이었던 경우, 해당 블럭으로 넘어옴
                case ObjectDeployEventPreset.AffineDeltaHandleType.ApplyAffineToLerp:
                {
                    if (result._IsEnterFirstCalculate)
                    {
                        var deployDelta = resultAffinePreset.GetAffineDelta(result.OriginPivot);
                        resultAffinePreset = result.OriginPivot;
                        resultAffinePreset.SyncAffine();
                        resultAffinePreset.ApplyAffineDelta(deployDelta);

                        result.SetStartPivot(resultAffinePreset, true);
                        result.TryUpdateBezierFirstPivot();
                    }
                }
                    break;
            }

            // 연산된 아핀값을 보정한다.
            var objectDeploySurfaceType = deployRecord.ObjectDeploySurfaceDeployType;
            var (valid, affine) = ObjectDeployTool.CorrectAffinePreset(GameManager.Obstacle_Terrain_LayerMask, resultAffinePreset, objectDeploySurfaceType);
            var checkDiverDown = _DiverBackFlag || DeployStateStorage[p_EventSequence].CheckDeployTraceUp(resultAffinePreset.Position.y - affine.Position.y);

            // 보정된 아핀값을 기준으로 각 이벤트를 처리한다.
            if (valid && checkDiverDown)
            {
                foreach (var deployableType in ObjectDeployTool._DeployableTypeEnumerator)
                {
                    if (p_EventCollection.ContainsKey(deployableType))
                    {
                        switch (deployableType)
                        {
#if !SERVER_DRIVE
                            case ObjectDeployTool.DeployableType.VFX:
                            {
                                var vfxList = p_EventCollection[ObjectDeployTool.DeployableType.VFX];
                                for (var i = 0; i < vfxList.Count; i++)
                                {
                                    DeployVfx(_MasterNode, vfxList[i], affine, result);
                                }
                            }
                                break;
#endif
                            case ObjectDeployTool.DeployableType.ProjectileVfx:
                            {
                                var projectileVfxList = p_EventCollection[ObjectDeployTool.DeployableType.ProjectileVfx];
                                var targetTuple = result.RangeTarget;
                                var targetDistance = targetTuple.t_Valid ? (affine.Position - targetTuple.t_Filtered.GetCenterPosition()).sqrMagnitude : 0f;
                                for (var i = 0; i < projectileVfxList.Count; i++)
                                {
                                    DeployVfxProjectile(_MasterNode, projectileVfxList[i], targetDistance, affine, result);
                                }
                            }
                                break;
#if !SERVER_DRIVE
                            case ObjectDeployTool.DeployableType.SFX:
                            {
                                var sfxList = p_EventCollection[ObjectDeployTool.DeployableType.SFX];
                                for (var i = 0; i < sfxList.Count; i++)
                                {
                                    var sfxIndex = sfxList[i];
                                    DeploySfx(_MasterNode, sfxIndex, affine, result);
                                }
                            }
                                break;
                            case ObjectDeployTool.DeployableType.Projector:
                            {
                                var _TryAffine = default(TransformTool.AffineCachePreset);
                                if (!objectDeploySurfaceType.HasAnyFlagExceptNone(ObjectDeployTool.ObjectDeploySurfaceDeployType.HasCollisionCheck))
                                {
                                    var (_valid, _affine) = ObjectDeployTool.CorrectAffinePreset(
                                        GameManager.Obstacle_Terrain_LayerMask,
                                        affine, ObjectDeployTool.ObjectDeploySurfaceDeployType.BreakDeployWhenNoCollision 
                                                | ObjectDeployTool.ObjectDeploySurfaceDeployType.ForceSurfaceUsingParameterVector);

                                    if (_valid)
                                    {
                                        _TryAffine = _affine;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    _TryAffine = affine;
                                }

                                var projectorList = p_EventCollection[ObjectDeployTool.DeployableType.Projector];
                                for (var i = 0; i < projectorList.Count; i++)
                                {
                                    var projectorIndex = projectorList[i];
                                    DeployProjector(_MasterNode, projectorIndex, _TryAffine, result);
                                }
                            }
                                break;
                            case ObjectDeployTool.DeployableType.TargetProjector:
                            {
                                var _TryAffine = default(TransformTool.AffineCachePreset);
                                var (_valid, _affine) = ObjectDeployTool.CorrectAffinePreset(
                                    GameManager.Obstacle_Terrain_LayerMask,
                                    result.TargetPivot, ObjectDeployTool.ObjectDeploySurfaceDeployType.BreakDeployWhenNoCollision 
                                                        | ObjectDeployTool.ObjectDeploySurfaceDeployType.ForceSurfaceUsingParameterVector);

                                if (_valid)
                                {
                                    _TryAffine = _affine;
                                }
                                else
                                {
                                    continue;
                                }

                                var projectorList = p_EventCollection[ObjectDeployTool.DeployableType.TargetProjector];
                                for (var i = 0; i < projectorList.Count; i++)
                                {
                                    var projectorIndex = projectorList[i];
                                    DeployProjector(_MasterNode, projectorIndex, _TryAffine, result);
                                }
                            }break;
#endif
                            case ObjectDeployTool.DeployableType.HitFilter:
                            {
                                var hitFilterList = p_EventCollection[ObjectDeployTool.DeployableType.HitFilter];
                                for (var i = 0; i < hitFilterList.Count; i++)
                                {
                                    var hitFilterIndex = hitFilterList[i];
                                    // DeployHitFilter(_MasterNode, p_EventSequence, hitFilterIndex, prevAffinePreset, affine, result);
                                }
                                
                            }
                                break;
                            case ObjectDeployTool.DeployableType.AutoMutton:
                            {
                                var autoMuttonList = p_EventCollection[ObjectDeployTool.DeployableType.AutoMutton];
                                for (var i = 0; i < autoMuttonList.Count; i++)
                                {
                                    var autoMuttonIndex = autoMuttonList[i];
                                    DeployAutoMutton(_MasterNode, autoMuttonIndex, affine, result);
                                }
                            }
                                break;
                            case ObjectDeployTool.DeployableType.ObjectDeployMap:
                            {
                                var deployMapList = p_EventCollection[ObjectDeployTool.DeployableType.ObjectDeployMap];
                                for (var i = 0; i < deployMapList.Count; i++)
                                {
                                    var deployMapIndex = deployMapList[i];
                                    DeployEventMap(_MasterNode, deployMapIndex, affine, result);
                                }
                            }
                                break;
                            case ObjectDeployTool.DeployableType.RecursiveDeploy:
                            {
                                var recursiveDeployList = p_EventCollection[ObjectDeployTool.DeployableType.RecursiveDeploy];
                                for (var i = 0; i < recursiveDeployList.Count; i++)
                                {
                                    var recursiveDeployIndex = recursiveDeployList[i];
                                    var targetRecord = ObjectDeployPresetMapData.GetInstanceUnSafe[recursiveDeployIndex];
                                    var copiedExtraPreset = result;
                                    var hasRecursiveOver = copiedExtraPreset.CheckRecursiveOver(targetRecord.MaxRecursiveCount);
                                    if (hasRecursiveOver)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        copiedExtraPreset.OnEntryRecursive(this, affine);

                                        if (_MasterNode.IsValid())
                                        {
                                            // _MasterNode._ActableObject.GetObjectDeployEventRecord().DeployUnitActionEvent(targetRecord.DeployEventPresetMap, copiedExtraPreset);
                                        }
                                        else
                                        {
                                            ObjectDeployLoader.GetInstance.GetObjectDeployEventRecord(_MasterNode).DeployUnitActionEvent(targetRecord.DeployEventPresetMap, copiedExtraPreset);
                                        }
                                    }
                                }
                                break;
                            }
                            case ObjectDeployTool.DeployableType.Buff:
                            {
                                var buffList = p_EventCollection[ObjectDeployTool.DeployableType.Buff];
                                for (var i = 0; i < buffList.Count; i++)
                                {
                                    var buffIndex = buffList[i];
                                    // DeployBuff(_MasterNode, buffIndex);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                DeployStateStorage[p_EventSequence].DeploySequenceState = DeploySequenceState.ReserveLastDeployEvent;
            }
            
            // 해당 프레임의 아핀값을 캐싱해준다.
            if (_UsingPivotFlag)
            {
            }
            else
            {
                _DeployPresetStorage[p_EventSequence] = result;
            }
            
            return result;
        }


        public bool CheckDelayActionValid()
        {
            return PoolState == PoolState.Actived;
        }
        
        #endregion

        #region <Structs>

        /// <summary>
        /// 현재 배치 이벤트의 상태를 기술하는 프리셋
        /// </summary>
        public struct DeployEventState
        {
            #region <Consts>

            public static DeployEventState GetDefaultEventState()
            {
                var result = new DeployEventState
                { 
                    HeightOffset = default, 
                    DeploySequenceState = default,
                };

                return result;
            }

            #endregion
            
            #region <Fields>

            /// <summary>
            /// 배치 이벤트의 높이 변화 값
            /// </summary>
            public float HeightOffset;
            
            /// <summary>
            /// 배치 이벤트의 유효상태
            /// </summary>
            public DeploySequenceState DeploySequenceState;

            #endregion

            #region <Methods>

            /// <summary>
            /// 배치 이벤트의 좌표가 연산됬을 때, 해당 좌표의 높이차이를 비교하는 메서드
            /// 차이가 더 커졌다면 더 저점으로 배치한다는 의미이므로 참을 리턴한다.
            /// </summary>
            public bool CheckDeployTraceUp(float p_NextHeightOffset)
            {
                var result = HeightOffset <= p_NextHeightOffset;
                if (result)
                {
                    HeightOffset = p_NextHeightOffset;
                }
                
                return result;
            }

            #endregion
        }

        #endregion
    }
}