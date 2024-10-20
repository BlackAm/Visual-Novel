using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 배치 진행도를 기술하는 프리셋
    /// </summary>
    public class ObjectDeployEventProgressControl
    {
        #region <Fields>

        /// <summary>
        /// 배치 인덱스
        /// </summary>
        public int DeployIndex;
        
        /// <summary>
        /// 배치 이벤트를 진행하는 타임 스탬프 하한지점
        /// </summary>
        public int LowerTimeStamp;
        
        /// <summary>
        /// 배치 이벤트를 진행하는 타임 스탬프 상한지점
        /// </summary>
        public int UpperTimeStamp;
        
        /// <summary>
        /// 배치 이벤트 시구간 길이, 상한 - 하한
        /// </summary>
        public float DeployDuration;
        
        /// <summary>
        /// 배치 이벤트 시구간 길이의 역수
        /// </summary>
        public float InverseDeployDuration;
        
        /// <summary>
        /// 해당 배치 이벤트가 타임 유닛을 하나 가지는 이벤트인지 표시하는 플래그
        /// </summary>
        public bool IsMonoDeploy;

        #endregion

        #region <Constructors>

        public ObjectDeployEventProgressControl(int p_DeployIndex)
        {
            DeployIndex = p_DeployIndex;
            LowerTimeStamp = int.MaxValue;
            UpperTimeStamp = 0;
        }
        
        public ObjectDeployEventProgressControl(int p_DeployIndex, int p_LowerTimeStamp, int p_UpperTimeStamp) : this(p_DeployIndex)
        {
            LowerTimeStamp = p_LowerTimeStamp;
            UpperTimeStamp = p_UpperTimeStamp;
        }

        #endregion

        #region <Callbacks>

        /// <summary>
        /// 타임 스탬프를 비교하여, 상/하한 값을 갱신하는 콜백
        /// </summary>
        public void OnCheckTimeStamp(int p_TryTimeStamp)
        {
            LowerTimeStamp = Mathf.Min(p_TryTimeStamp, LowerTimeStamp);
            UpperTimeStamp = Mathf.Max(p_TryTimeStamp, UpperTimeStamp);
        }

        /// <summary>
        /// 상/하한 값 갱신 종료시, 해당 값들로 시간 관련 필드를 초기화시키는 콜백
        /// </summary>
        public void OnInitialize()
        {
            IsMonoDeploy = LowerTimeStamp == UpperTimeStamp;
            if (IsMonoDeploy)
            {
                DeployDuration = InverseDeployDuration = 0f;
            }
            else
            {
                DeployDuration = UpperTimeStamp - LowerTimeStamp;
                InverseDeployDuration = 1f / DeployDuration;
            }
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// 해당 프리셋의 시구간에 대해, 지정한 타임스탬프의 진행도를 리턴하는 메서드
        /// </summary>
        public float GetProgressRate(int p_TryStamp)
        {
            return IsMonoDeploy ? 1f : (p_TryStamp - LowerTimeStamp) * InverseDeployDuration;
        }

        #endregion
    }

    /// <summary>
    /// 배치 타임라인을 기술하는 프리셋
    /// </summary>
    public struct ObjectDeployTimePreset : IEnumerator<int>
    {
        #region <Fields>

        private List<int> _TimeStampSet;
        private int _CurrentIndex;
        
        #endregion

        #region <Constructors>

        public ObjectDeployTimePreset(int p_Predelay)
        {
            _TimeStampSet = new List<int>();
            _TimeStampSet.Add(p_Predelay);
            Current = default;
            _CurrentIndex = -1;
        }

        public ObjectDeployTimePreset(int p_Predelay, int p_Interval, int p_Count)
        {
            _TimeStampSet = new List<int>();
            for (int i = 0; i < p_Count; i++)
            {
                var tryDelay = p_Predelay + p_Interval * i;
                _TimeStampSet.Add(tryDelay);
            }
            Current = default;
            _CurrentIndex = -1;
        }
        
        public ObjectDeployTimePreset(List<int> p_DelayList)
        {
            _TimeStampSet = p_DelayList;
            Current = default;
            _CurrentIndex = -1;
        }
        
        #endregion

        #region <Methods>

        public int GetFirstStamp()
        {
            return ReferenceEquals(null, _TimeStampSet) ? 0 : _TimeStampSet.First();
        }
        
        public int GetLastStamp()
        {
            return ReferenceEquals(null, _TimeStampSet) ? 0 : _TimeStampSet.Last();
        }

        public void AddOffset(int p_PivotStamp)
        {
            var count = _TimeStampSet.Count;
            for (int i = 0; i < count; i++)
            {
                _TimeStampSet[i] = p_PivotStamp + _TimeStampSet[i];
            }
        }
        
        public void UpdateOffset(int p_PivotStamp)
        {
            var count = _TimeStampSet.Count;
            if (count > 0)
            {
                var tryFirst = _TimeStampSet[0];
                if (tryFirst < 1)
                {
                    for (int i = 0; i < count; i++)
                    {
                        _TimeStampSet[i] = p_PivotStamp - 2 * tryFirst + _TimeStampSet[i];
                    }
                }
            }
        }

        #endregion

        #region <Enumerator>

        public bool MoveNext()
        {
            if (_CurrentIndex + 1 < _TimeStampSet.Count)
            {
                Current = _TimeStampSet[++_CurrentIndex];
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Reset()
        {
            _CurrentIndex = -1;
        }

        public int Current { get; private set; }

        object IEnumerator.Current => Current;

        /// <summary>
        /// 인스턴스 파기 메서드
        /// </summary>
        public void Dispose()
        {
        }

        #endregion
    }
    
    /// <summary>
    /// 배치할 이벤트 인덱스를 기술하는 프리셋
    /// </summary>
    public struct ObjectDeployIndexPreset : IEnumerator<int>
    {
        #region <Fields>

        private List<int> _DeployIndexList;
        private int _CurrentIndex;
        
        #endregion

        #region <Constructors>

        public ObjectDeployIndexPreset(int p_Index)
        {
            _DeployIndexList = new List<int>();
            _DeployIndexList.Add(p_Index);
            Current = default;
            _CurrentIndex = -1;
        }
        
        public ObjectDeployIndexPreset(int p_Index, int p_Count)
        {
            _DeployIndexList = new List<int>();
            for (int i = 0; i < p_Count; i++)
            {
                _DeployIndexList.Add(p_Index);
            }
            Current = default;
            _CurrentIndex = -1;
        }
        
        public ObjectDeployIndexPreset(List<int> p_IndexList)
        {
            _DeployIndexList = p_IndexList;
            Current = default;
            _CurrentIndex = -1;
        }
        
        #endregion

        #region <Enumerator>

        public bool MoveNext()
        {
            if (_CurrentIndex + 1 < _DeployIndexList.Count)
            {
                Current = _DeployIndexList[++_CurrentIndex];
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Reset()
        {
            _CurrentIndex = -1;
        }

        public int Current { get; private set; }

        object IEnumerator.Current => Current;

        /// <summary>
        /// 인스턴스 파기 메서드
        /// </summary>
        public void Dispose()
        {
        }

        #endregion
    }

    /// <summary>
    /// 배치 이벤트의 파라미터를 기술하는 프리셋
    /// </summary>
    public struct ObjectDeployParams
    {
        #region <Fields>

        /// <summary>
        /// 이벤트 배치 플래그
        /// </summary>
        public ObjectDeployEventPreset.EventDeployFlag EventDeployFlag;
        
        /// <summary>
        /// 해당 이벤트의 최대 타격 횟수
        /// 해당 값이 0인 경우, 해당 이벤트의 타격횟수는 서로 독립적으로 동작한다.
        /// </summary>
        public int MaxHitCount { get; private set; }
        
        /// <summary>
        /// 해당 이벤트가 하나의 유닛을 타격할 수 있는 최대횟수
        /// </summary>
        public (int t_Count, float t_Interval) SameUnitHitPreset { get; private set; }
        
        /// <summary>
        /// 해당 이벤트가 근처의 타겟을 탐색해야하는 경우 범위
        /// </summary>
        public float Range { get; private set; }

        /// <summary>
        /// 아핀 변화량 처리 타입
        /// </summary>
        public ObjectDeployEventPreset.AffineDeltaHandleType AffineDeltaHandleType { get; private set; }
        
        #endregion

        #region <Constructors>

        #endregion

        #region <Methods>

        public void AddDeployEventFlag(ObjectDeployEventPreset.EventDeployFlag p_Flag)
        {
            EventDeployFlag.AddFlag(p_Flag);
        }

        #endregion
    }

    /// <summary>
    /// 배치 연산에 직접 관여하지 않는 추가 데이터 프리셋
    /// </summary>
    public struct ObjectDeployEventExtraPreset
    {
        #region <Fields>

        /// <summary>
        /// 배치 이벤트의 연산 기준을 기술하는 필드
        /// </summary>
        public TransformTool.AffineCachePreset StartPivot;
        public TransformTool.AffineCachePreset OriginPivot;
        public TransformTool.AffineCachePreset CurrentPivot;
        public Vector3 TargetPivot;
        public float ProgressRate01;
        
        /// <summary>
        /// 배치 이벤트가 러프 모드로 동작해야할 때 사용할 데이터 프리셋
        /// </summary>
        public CubicBezierPreset BezierPreset;

        /// <summary>
        /// 적 탐색 범위
        /// </summary>
        public float Range;
        
        /// <summary>
        /// 플래그
        /// </summary>
        public bool _UsingFallbackTargetPositionFlag;
        public bool _IsLerpAffine;
        public bool _IsEnterFirstCalculate;
        public bool _IsFoucsOnMainWeapon;
        
        /// <summary>
        /// 재귀 배치 카운트 프리셋
        /// </summary>
        public int MaxRecursiveCount;
        public int CurrentRecursiveCount;
        public int DeployEventIndex;
        
        #endregion
        
        #region <Callbacks>

        /// <summary>
        /// 이벤트가 종료되어 릴리스 되는 경우 호출되는 콜백
        /// </summary>
        public void OnRetrievePreset()
        {
            if (!ReferenceEquals(null, BezierPreset))
            {
                BezierPreset.RetrieveObject();
                BezierPreset = null;
            }
        }

        /// <summary>
        /// 최초 이벤트 실행 시에, 호출되는 콜백
        /// </summary>
        public bool OnCheckEventEnterable(ObjectDeployEventRecord p_DeployEventRecord)
        {
            var result = true;
            // 재귀 호출된 경우, Start 및 Target 값은 이미 세트되어 있음
            if (IsRecursiveEntry())
            {
                _UsingFallbackTargetPositionFlag = false;
            }
            // 재귀 호출 외의 방법으로 이벤트에 진입하는 경우,
            else
            {
                SetStartPivot(p_DeployEventRecord._ZeroPivotAffine, true);
                TargetPivot = StartPivot.Position;
                Range = p_DeployEventRecord._ObjectDeployParams.Range;
                DeployEventIndex = -1;
            }
            
            _UsingFallbackTargetPositionFlag = true;
            result = !p_DeployEventRecord._BlockEventWheneNoTargetFlag;

            if (result)
            {
                OnEntryFirstCalculation();
            }

            return result;
        }

        public void OnEntryFirstCalculation()
        {
            SetStartPivot(CurrentPivot, false);
            OriginPivot = StartPivot;
        }

        /// <summary>
        /// 해당 프리셋이 재귀 배치 이벤트로 넘어갈 때 호출되는 콜백
        /// </summary>
        public void OnEntryRecursive(ObjectDeployEventRecord p_DeployEventRecord, TransformTool.AffineCachePreset p_Affine)
        {
            if (p_DeployEventRecord._UpdateRecursiveStartPositionFlag)
            {
                SetStartPivot(p_Affine, true);
                TargetPivot = p_Affine.Position;
            }
            else
            {
                TargetPivot = p_Affine.Position;
            }
        }

        #endregion
        
        #region <Methods>

        /// <summary>
        /// 해당 프리셋이 재귀호출되어 있는지 여부를 리턴하는 논리함수
        /// </summary>
        public bool IsRecursiveEntry()
        {
            return CurrentRecursiveCount > 0;
        }

        /// <summary>
        /// 재귀 횟수를 비교하여, 현재 이벤트가 재귀 이벤트에 진입할 수 있는지 검증하는 메서드
        /// </summary>
        public bool CheckRecursiveOver(int p_MaxRecursiveCount)
        {
            if (MaxRecursiveCount < 1)
            {
                MaxRecursiveCount = p_MaxRecursiveCount;
            }
            CurrentRecursiveCount++;

            return CurrentRecursiveCount > MaxRecursiveCount;
        }

        /// <summary>
        /// 시점 pivot을 지정하는 메서드
        /// </summary>
        public void SetStartPivot(TransformTool.AffineCachePreset p_StartPivot, bool p_SyncCurrentPivot)
        {
            StartPivot = p_StartPivot;

            if (p_SyncCurrentPivot)
            {
                CurrentPivot = StartPivot;
            }
        }
        
        /// <summary>
        /// 현재 pivot을 지정하는 메서드
        /// </summary>
        public void SetCurrentPivot(TransformTool.AffineCachePreset p_Pivot)
        {
            CurrentPivot = p_Pivot;
        }
        
        /// <summary>
        /// 종점 pivot을 지정하는 메서드
        /// </summary>
        public void SetTargetPivot(Vector3 p_Pivot)
        {
            TargetPivot = p_Pivot;
        }

        /// <summary>
        /// 베지어 러프를 수행하는 경우, 베지어 연산자를 초기화시키는 메서드
        /// </summary>
        public void InitBezierPreset(Vector3 p_Pivot01, Vector3 p_Pivot02)
        {
            BezierPreset = CubicBezierManager.GetInstance.GetBezierPreset();
            BezierPreset.SetPivot(StartPivot.Position, p_Pivot01, p_Pivot02, TargetPivot);
        }

        public void TryUpdateBezierFirstPivot()
        {
            if (!ReferenceEquals(null, BezierPreset))
            {
                BezierPreset.SetFirstPivot(StartPivot.Position);
            }
        }

        /// <summary>
        /// 베지어 연산자로부터 진행도에 따른 좌표값을 리턴하는 메서드
        /// </summary>
        public Vector3 GetBezierPosition(float p_ProgressRate)
        {
            return BezierPreset.GetBezierPosition(p_ProgressRate);
        }
        
        /// <summary>
        /// CurrentPivot에 베지어 곡선 좌표 값을 세트하는 메서드
        /// </summary>
        public void SetBezierPosition(float p_ProgressRate)
        {
            CurrentPivot.SetPosition(GetBezierPosition(p_ProgressRate));
        }
        
        #endregion
    }

    /// <summary>
    /// 지정한 유닛 모션 타이밍에 생성해야할 오브젝트에 대한 기술을 하는 프리셋
    /// </summary>
    public struct ObjectDeployEventPreset
    {
        #region <Fields>
            
        /// <summary>
        /// EventDeployFlag 타입 플래그 마스크
        /// </summary>
        public ObjectDeployParams _ObjectDeployParams;

        /// <summary>
        /// [이벤트 선딜레이, [배치 인덱스, [배치할 이벤트 타입, 배치할 이벤트 인덱스 리스트]] 리스트] 컬렉션
        /// </summary>
        public Dictionary<int, List<(int t_DeployDataIndex, Dictionary<ObjectDeployTool.DeployableType, List<int>> t_EventCollection)>> _EventDeployCollection;
        
        /// <summary>
        /// 해당 이벤트 프리셋이 보유한 Deployable 타입 마스크
        /// </summary>
        public ObjectDeployTool.DeployableType _RetainedDeployableTypeMask;
        
        /// <summary>
        /// 하나의 시점에서 동시에 배치되는 이벤트 숫자
        /// </summary>
        public int _EventSequenceCount;

        /// <summary>
        /// 해당 이벤트 배치 인덱스 별 시간 유닛
        /// </summary>
        public Dictionary<int, ObjectDeployEventProgressControl> _TimeControlUnitCollection;
        
        #endregion

        #region <Enums>

        public enum AffineDeltaHandleType
        {
            /// <summary>
            /// Affine 변화값을 처리하지 않음
            /// </summary>
            None,

            ApplyAffineOnlyFirst,

            ApplyAffineWithFirstRotation,
            
            ApplyAffine,
            
            ApplyAffineToLerp,
        }

        [Flags]
        public enum EventDeployFlag
        {
            None = 0,
                
            /// <summary>
            /// 지연된 이벤트 처리시에, 각 지연된 타이밍에 사용하는 아핀값을 최초 이벤트 타이밍의 것을 사용할지 정하는 플래그
            /// </summary>
            UsingZeroTimePivot = 1 << 0,

            /// <summary>
            /// 해당 이벤트를 통해 발생하는 타격 정보가 서로 타격 정보를 공유하도록 하는 플래그
            /// </summary>
            SharedHitState = 1 << 1,
            
            /// <summary>
            /// 연속 배치 이벤트가 지형의 차이로 인해 저점으로 배치를 하다 높은 지점과 충돌하면 이벤트를 종료하는데, 저점에서 고점으로의 배치를 허용시키는 플래그
            /// </summary>
            DiverBack = 1 << 2,
            
            /// <summary>
            /// 기본적으로 오브젝트 배치 레코드에서 배치할 위치에 배치할 수 없는 경우에 곧바로 마지막 배치 이벤트를 발동하도록 처리하고 있는데
            /// 그와는 별개로 HitFilter에 의해 유닛이나 장해물과 충돌이 검증된 경우 곧바로 마지막 이벤트를 발동하고 해당 배치 이벤트를 종료시키는 플래그
            /// </summary>
            BlockBuster = 1 << 3,
            
            /// <summary>
            /// 해당 플래그가 Set인 경우 Recursive Deploy가 발생할 때, Start 시점을 현재 배치 지점으로 지정한다.
            /// </summary>
            UpdateRecursiveStartPosition = 1 << 4,
            
            /// <summary>
            /// 대상이 없는 경우, 이벤트를 Block시킨다.
            /// </summary>
            BlockEventWhenNoTarget = 1 << 5,
            
            /// <summary>
            /// 대상이 없는 경우 대타로 현재 MasterNode가 포커싱 중인 유닛을 참조하지 못하게 한다.
            /// </summary>
            BlockFallBackCurrentFocus = 1 << 6,
        }

        #endregion
            
        #region <Constructors>
             
        /// <summary>
        /// 기본 생성자
        /// </summary>
        private ObjectDeployEventPreset(ObjectDeployParams p_ObjectDeployParams)
        {
            _ObjectDeployParams = p_ObjectDeployParams;
            _EventDeployCollection = null;
            _RetainedDeployableTypeMask = ObjectDeployTool.DeployableType.None;
            _EventSequenceCount = default;
            _TimeControlUnitCollection = new Dictionary<int, ObjectDeployEventProgressControl>();
        }
            
        /// <summary>
        /// 지정한 이벤트 배치 셋 하나만을 초기화하는 생성자
        /// </summary>
        public ObjectDeployEventPreset(Dictionary<int, List<(int t_DeployDataIndex, Dictionary<ObjectDeployTool.DeployableType, List<int>> t_EventCollection)>> p_EventDeployCollection, 
            ObjectDeployParams p_ObjectDeployParams = default) : this(p_ObjectDeployParams)
        {
            _EventDeployCollection = p_EventDeployCollection;
            InitializeDeployEvent();
        }
        
        /// <summary>
        /// 지정한 이벤트 배치 셋 하나만을 초기화하는 생성자에
        /// 추가로 동시에 배치할 수 있도록 DeployIndex를 리스트로 지정할 수 있는 생성자
        /// </summary>
        public ObjectDeployEventPreset(Dictionary<ObjectDeployTimePreset, List<(ObjectDeployIndexPreset t_DeployDataIndexPreset, Dictionary<ObjectDeployTool.DeployableType, List<int>> t_EventCollection)>> p_EventDeployCollection, 
            ObjectDeployParams p_ObjectDeployParams = default) : this(p_ObjectDeployParams)
        {
            _EventDeployCollection = new Dictionary<int, List<(int t_DeployDataIndex, Dictionary<ObjectDeployTool.DeployableType, List<int>> t_EventCollection)>>();
            MergeCollection(p_EventDeployCollection, ref _EventDeployCollection);
            InitializeDeployEvent();
        }

        private void InitializeDeployEvent()
        {
            var minDelay = int.MaxValue;
            var maxDelay = 0;

            foreach (var deployableCollectionKV in _EventDeployCollection)
            {
                // 전체 이벤트 진행에 관한 시간 유닛 초기화
                var delay = deployableCollectionKV.Key;
                minDelay = Mathf.Min(delay, minDelay);
                maxDelay = Mathf.Max(delay, maxDelay);

                var eventTypeCollectionList = deployableCollectionKV.Value;
                foreach (var eventCollectionKV in eventTypeCollectionList)
                {
                    // 각 배치 인덱스에 대한 시간 유닛 초기화
                    var eventDeployIndex = eventCollectionKV.t_DeployDataIndex;
                    if(!_TimeControlUnitCollection.TryGetValue(eventDeployIndex, out var o_TimeControl))
                    {
                        _TimeControlUnitCollection.Add(eventDeployIndex, o_TimeControl = new ObjectDeployEventProgressControl(eventDeployIndex));
                    }
                    o_TimeControl.OnCheckTimeStamp(delay);
                    
                    // 각 배치 인덱스에 포함된 이벤트 타입 초기화
                    var eventTypeCollection = eventCollectionKV.t_EventCollection.Keys;
                    foreach (var deployableType in eventTypeCollection)
                    {
                        _RetainedDeployableTypeMask.AddFlag(deployableType);
                    }
                }
            }

            // 각 시간 유닛을 기준으로 시간 제어 유닛을 초기화시켜준다.
            _TimeControlUnitCollection.Add(-1, new ObjectDeployEventProgressControl(-1, minDelay, maxDelay));
            foreach (var _TimeUnitKV in _TimeControlUnitCollection)
            {
                _TimeUnitKV.Value.OnInitialize();
            }
            
            // 포함된 이벤트 종류에 따라, 파라미터의 배치 플래그 마스크를 갱신시킨다.
            if (_RetainedDeployableTypeMask.HasAnyFlagExceptNone(ObjectDeployTool.DeployableType.HitFilter) 
                && _ObjectDeployParams.MaxHitCount > 0)
            {
                _ObjectDeployParams.AddDeployEventFlag(EventDeployFlag.SharedHitState);
            }
            
            // 동시에 진행되고 있는 이벤트 숫자를 갱신시켜준다.
            _EventSequenceCount = _EventDeployCollection.First().Value.Count;
        }
            
        #endregion

        #region <Constructor/Merge>

        public void MergeCollection(Dictionary<ObjectDeployTimePreset, List<(ObjectDeployIndexPreset t_DeployDataIndexPreset, Dictionary<ObjectDeployTool.DeployableType, List<int>> t_EventCollection)>> p_EventDeployCollection, ref Dictionary<int, List<(int t_DeployDataIndex, Dictionary<ObjectDeployTool.DeployableType, List<int>> t_EventCollection)>> r_MergeTarget)
        {
            var keySet = p_EventDeployCollection.Keys;
            foreach (var timeLine in keySet)
            {
                var record = p_EventDeployCollection[timeLine];
                var delaySet = _EventDeployCollection.Keys;
                timeLine.UpdateOffset(delaySet.Count > 0 ? delaySet.Max() : 0);
                
                while (timeLine.MoveNext())
                {
                    var tryDelay = timeLine.Current;
                    if (r_MergeTarget.TryGetValue(tryDelay, out var o_EventList))
                    {
                        var wrapper = new List<(int t_DeployDataIndex, Dictionary<ObjectDeployTool.DeployableType, List<int>> t_EventCollection)>();
                        foreach (var tryTuple in o_EventList)
                        {
                            foreach (var compareTupleList in record)
                            {
                                var deployIndexPreset = compareTupleList.t_DeployDataIndexPreset;
                                var compareEventCollection = compareTupleList.t_EventCollection;
                                while (deployIndexPreset.MoveNext())
                                {
                                    var currentDeployIndex = deployIndexPreset.Current;
                                    if (tryTuple.t_DeployDataIndex == currentDeployIndex)
                                    {
                                        var eventCollection = tryTuple.t_EventCollection;
                                        foreach (var compareEventPair in compareEventCollection)
                                        {
                                            var tryDeployEventType = compareEventPair.Key;
                                            if (eventCollection.TryGetValue(tryDeployEventType, out var o_DeployEventList))
                                            {
                                                o_DeployEventList.AddRange(compareEventPair.Value);
                                            }
                                            else
                                            {
                                                eventCollection.Add(tryDeployEventType, compareEventPair.Value);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var hasDeployIndex = false;

                                        foreach (var tuple in wrapper)
                                        {
                                            if (tuple.t_DeployDataIndex == currentDeployIndex)
                                            {
                                                hasDeployIndex = true;
                                                var eventCollection = tuple.t_EventCollection;
                                                foreach (var compareEventPair in compareEventCollection)
                                                {
                                                    var tryDeployEventType = compareEventPair.Key;
                                                    if (eventCollection.TryGetValue(tryDeployEventType, out var o_DeployEventList))
                                                    {
                                                        o_DeployEventList.AddRange(compareEventPair.Value);
                                                    }
                                                    else
                                                    {
                                                        eventCollection.Add(tryDeployEventType, compareEventPair.Value);
                                                    }
                                                }

                                                break;
                                            }
                                        }

                                        if (!hasDeployIndex)
                                        {
                                            wrapper.Add((currentDeployIndex, compareEventCollection));
                                        }
                                    }
                                }
                            }
                        }
                        
                        o_EventList.AddRange(wrapper);
                    }
                    else
                    {
                        var wrapper = new List<(int t_DeployDataIndex, Dictionary<ObjectDeployTool.DeployableType, List<int>> t_EventCollection)>();
                        foreach (var deployTupleListTuple in record)
                        {
                            var deployIndexPreset = deployTupleListTuple.t_DeployDataIndexPreset;
                            var deployEventCollection = deployTupleListTuple.t_EventCollection;
                            while (deployIndexPreset.MoveNext())
                            {
                                wrapper.Add((deployIndexPreset.Current, deployEventCollection));
                            }
                        }

                        r_MergeTarget.Add(tryDelay, wrapper);
                    }
                }
            }
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// 프리로드되는 리소스는 전부 씬 단위로 로드되며,
        /// 유닛이 초기화되어 스킬 액션을 로드할 때 해당 메서드를 통해 수행된다.
        ///
        /// 따라서, 유닛의 리소스 수명이 씬 이상인 유닛이 있는 경우 해당 유닛이 씬을 전이했을 때,
        /// 프리로드는 동작하지 않게 된다.
        ///
        /// 이를 처리하려면, UnitInteractManager 에서 씬 전이시 활성화된 유닛들을 Hide하는 이벤트에서 같이
        /// 프리로드를 하도록 개선하면 될 듯.
        /// 
        /// </summary>
        public void Preload()
        {
            foreach (var deployCollectionListKV in _EventDeployCollection)
            {
                var deployCollectionList = deployCollectionListKV.Value;
                foreach (var deployCollectionKV in deployCollectionList)
                {
                    var deployCollection = deployCollectionKV.t_EventCollection;
                    foreach (var deployableType in ObjectDeployTool._DeployableTypeEnumerator)
                    {
                        if (deployCollection.ContainsKey(deployableType))
                        {
                            switch (deployableType)
                            {
#if !SERVER_DRIVE
                                case ObjectDeployTool.DeployableType.VFX:
                                    var vfxList = deployCollection[ObjectDeployTool.DeployableType.VFX];
                                    foreach (var i in vfxList)
                                    {
                                        VfxSpawnManager.GetInstance.Preload(i, 8);
                                    }
                                    break;
#endif
                                case ObjectDeployTool.DeployableType.ProjectileVfx:
                                    var projectileVfxList = deployCollection[ObjectDeployTool.DeployableType.ProjectileVfx];
                                    foreach (var i in projectileVfxList)
                                    {
                                        VfxSpawnManager.GetInstance.Preload(i, 8);
                                    }
                                    break;
#if !SERVER_DRIVE
                                case ObjectDeployTool.DeployableType.SFX:
                                    var sfxList = deployCollection[ObjectDeployTool.DeployableType.SFX];
                                    foreach (var i in sfxList)
                                    {
                                        SoundDataRoot.GetInstanceUnSafe.PreloadMediaClip(i);
                                    }
                                    break;
#endif
                                case ObjectDeployTool.DeployableType.AddForce:
                                    break;
                                case ObjectDeployTool.DeployableType.RecursiveDeploy:
                                    var recursiveDeployList = deployCollection[ObjectDeployTool.DeployableType.RecursiveDeploy];
                                    foreach (var i in recursiveDeployList)
                                    {
                                        var targetRecord = ObjectDeployPresetMapData.GetInstanceUnSafe[i];
                                        targetRecord.DeployEventPresetMap.Preload();
                                    }
                                    break;
                                
                                case ObjectDeployTool.DeployableType.Buff:
                                    break;
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}