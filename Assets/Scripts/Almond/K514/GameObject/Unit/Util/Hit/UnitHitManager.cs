using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public class UnitHitManager : Singleton<UnitHitManager>
    {
        #region <Consts>

        private const int HitProcessObserverPreloadCount = 256;
        
        #endregion

        #region <Fields>

        private ObjectPooler<HitProcessObserver> _HitProcessObserverPooler;

        #endregion

        #region <Callbacks>

        protected override void OnCreated()
        {
            _HitProcessObserverPooler = new ObjectPooler<HitProcessObserver>();
            _HitProcessObserverPooler.PreloadPool(HitProcessObserverPreloadCount, HitProcessObserverPreloadCount);
        }

        public override void OnInitiate()
        {
        }

        #endregion

        #region <Methods>

        public HitProcessObserver GetHitProcessObserver(int p_HitPresetIndex)
        {
            var hitPreset = UnitHitPresetData.GetInstanceUnSafe.GetTableData(p_HitPresetIndex);
            var spawned = _HitProcessObserverPooler.GetObject();
            var hitParameter = hitPreset.GetHitMessage().GetHitExtraRecord();
            if (hitParameter.Item1)
            {
                var hitExtraRecord = hitParameter.Item2;
                spawned.SetMaxHitCount(hitExtraRecord.MaxHitCount);
                spawned.SetSameUnitHitPreset(hitExtraRecord.MaxSameUnitHitPreset);
            }
            
            return spawned;
        }
        
        public HitProcessObserver GetHitProcessObserver(int p_MaxHitCount, (int t_Count, float t_Interval) p_SameUnitHitPreset)
        {
            var spawned = _HitProcessObserverPooler.GetObject();
            spawned.SetMaxHitCount(p_MaxHitCount);
            spawned.SetSameUnitHitPreset(p_SameUnitHitPreset);
            
            return spawned;
        }

        public (UnitFilterTool.FilterResultType, UnitHitTool.HitResultType) TossDamageWithNullableObserver(Unit p_FromUnit, int p_HitPresetIndex, UnitTool.UnitAddForcePreset p_HitVariablePreset, TransformTool.AffineCachePreset p_Affine, bool p_FilterObstacle, HitProcessObserver p_HitMessageObserver)
        {
            if (ReferenceEquals(null, p_HitMessageObserver))
            {
                return TossDamage(p_FromUnit, p_HitPresetIndex, p_HitVariablePreset, p_Affine, p_FilterObstacle);
            }
            else
            {
                return p_HitMessageObserver.TossDamage(p_FromUnit, p_HitPresetIndex, p_HitVariablePreset, p_Affine, p_FilterObstacle);
            }
        }

        public (UnitFilterTool.FilterResultType, UnitHitTool.HitResultType) TossDamage(Unit p_FromUnit, int p_HitPresetIndex, UnitTool.UnitAddForcePreset p_HitVariablePreset, TransformTool.AffineCachePreset p_Affine, bool p_FilterObstacle)
        {
            var instantHitObserver = GetHitProcessObserver(p_HitPresetIndex);
            var result = instantHitObserver.TossDamage(p_FromUnit, p_HitPresetIndex, p_HitVariablePreset, p_Affine, p_FilterObstacle);
            instantHitObserver.TryRetrieveHitObserver();

            return result;
        }

        #endregion
    }

    /// <summary>
    /// 타격 판정을 직접 제어하는 풀링 인스턴스
    /// 하나의 타격 판정의 수명은 해당 인스턴스의 수명과 같다.
    /// </summary>
    public class HitProcessObserver : PoolingObject<HitProcessObserver>
    {
        #region <Consts>

        /// <summary>
        /// 후폭풍 간격 및 멀티 타격 간격 제한 등에 사용하는 타격 시간 단위
        /// </summary>
        public const int HitBasisTimeUnit = 50;

        #endregion
        
        #region <Fields>

        /// <summary>
        /// 유닛 필터에 사용되는 NonAlloc 리스트
        /// 필터 함수에 의해 내부 멤버가 결정된다.
        /// </summary>
        private List<Unit> _FilterList;
        
        /// <summary>
        /// 타격정보 컬렉션
        /// </summary>
        public Dictionary<Unit, UnitHitInfoPreset> UnitHitInfoPresetTable { get; private set; }
        
        /// <summary>
        /// 최대 타격횟수
        /// </summary>
        public int MaxHitCount { get; private set; }
        
        /// <summary>
        /// 한 유닛을 대상으로 최대한 타격할 수 있는 횟수 및 간격
        /// </summary>
        public (int t_Count, float t_Interval) SameUnitHitPreset { get; private set; }

        /// <summary>
        /// 타격이 후폭풍을 가지는 경우, 후폭풍 판정이 끝난후 해당 인스턴스를 릴리스하게 하는 플래그
        /// </summary>
        private bool _ProgressHBBFlag;
        
        /* TossDamage 로부터 초기화되는 필드 */
        /// <summary>
        /// 타격 주체
        /// </summary>
        public Unit _FromUnit;
        
        /// <summary>
        /// 타격 프리셋 테이블
        /// </summary>
        public UnitHitPresetData.TableRecord _HitPreset;
        
        /// <summary>
        /// 타격 메시지
        /// </summary>
        public HitMessage _HitMessage;
        
        /// <summary>
        /// 외력 프리셋
        /// </summary>
        public UnitTool.UnitAddForcePreset _HitVariablePreset;
        
        /// <summary>
        /// 필터 파라미터
        /// </summary>
        public FilterParams _FilterParams;
        
        /// <summary>
        /// 타격 추가 데이터 튜플
        /// </summary>
        public (bool, UnitHitExtraData.TableRecord) _HitExtraRecordPreset;
        
        #endregion

        #region <Callbacks>

        public override void OnSpawning()
        {
            _FilterList = new List<Unit>();
            UnitHitInfoPresetTable = new Dictionary<Unit, UnitHitInfoPreset>();
        }

        public override void OnPooling()
        {
            _FilterList.Clear();
            UnitHitInfoPresetTable.Clear();
            
            SetSameUnitHitPreset((1, 0f));
            SetMaxHitCount(1);
        }

        public override void OnRetrieved()
        {
            _ProgressHBBFlag = false;
            _FromUnit = default;
            _HitMessage = default;
            _HitPreset = default;
            _HitVariablePreset = default;
            _FilterParams = default;
        }

        #endregion
        
        #region <Methods>

        /// <summary>
        /// Entry Point
        /// </summary>
        public (UnitFilterTool.FilterResultType, UnitHitTool.HitResultType) TossDamage(Unit p_FromUnit, int p_HitPresetIndex, UnitTool.UnitAddForcePreset p_HitVariablePreset, TransformTool.AffineCachePreset p_Affine, bool p_FilterObstacle)
        {
            _FromUnit = p_FromUnit;
            _HitPreset = UnitHitPresetData.GetInstanceUnSafe.GetTableData(p_HitPresetIndex);
            _HitVariablePreset = p_HitVariablePreset;
            _HitMessage = _HitPreset.GetHitMessage();

            _FilterParams = _HitPreset.FilterParams;
            _FilterParams.SetFilterAffine(p_Affine, true);
            _FilterParams.SetFilterFlag(UnitFilterTool.FilterParamsFlag.FilterObstacle, p_FilterObstacle);

            _HitExtraRecordPreset = _HitMessage.GetHitExtraRecord();

            // 타격 후폭풍 판정이 있다면 가져온다.
            var hbbCount = _HitExtraRecordPreset.Item1 ? _HitExtraRecordPreset.Item2.HitBackBlastCount : 0;
            
            // 해당 타격이 후폭풍 판정이 없는 경우
            if (hbbCount < 1)
            {
                return EstimateDamage();
            }
            // 해당 타격이 후폭풍을 가지는 경우
            else
            {
                var result = EstimateDamage();
                _ProgressHBBFlag = true;
                
                SystemBoot
                    .GameEventTimer
                    .RunTimer(
                        null,
                        (HitBasisTimeUnit, HitBasisTimeUnit),
                        (handler) =>
                        {
                            var hitObserver = handler.Arg1;
                            hitObserver.EstimateDamage();
                            handler.Arg2--;
                            
                            // 후폭풍 카운터가 전소됬거나, 타격횟수가 남지 않은 경우
                            if (handler.Arg2 < 1 || hitObserver.MaxHitCount < 1)
                            {
                                hitObserver.RetrieveObject();
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        },
                        (handler) => handler.Arg2 < 1,
                        this, hbbCount
                    );

                return result;
            }
        }
        
        private (UnitFilterTool.FilterResultType, UnitHitTool.HitResultType) EstimateDamage()
        {
            var timeStamp = Time.time;
            var hitResult = default(HitResult);
            var filterResult = UnitInteractManager.GetInstance.FilterUnit_And(_FromUnit, _FilterParams, _HitPreset.FilterFlag, _FilterList);
            var hitMessage = _HitMessage;
            
            if (filterResult.HasAnyFlagExceptNone(UnitFilterTool.FilterResultType.Unit))
            {
                var hitCount = _FilterList.Count;
                var extraHitCount = _HitExtraRecordPreset.Item1 ? _HitExtraRecordPreset.Item2.ExtraHitCount : 0;
                var maxHitCount = MaxHitCount;
                for (int i = 0; i < hitCount && 0 < maxHitCount; i++)
                {
                    var targetUnit = _FilterList[i];
                    if (CheckHitValidation(targetUnit, timeStamp))
                    {
                        hitResult = targetUnit.HitUnit(_FromUnit, hitMessage, _HitVariablePreset, false);
                        if (hitResult.HitResultType != UnitHitTool.HitResultType.HitFail)
                        {
                            maxHitCount--;
                        }
                        
                        if (extraHitCount > 0)
                        {
                            var hitInterval = _HitExtraRecordPreset.Item1 ? _HitExtraRecordPreset.Item2.ExtraHitInterval : 0;
                            if (hitInterval > HitBasisTimeUnit)
                            {
                                SystemBoot
                                    .GameEventTimer
                                    .RunTimer(
                                        null,
                                        (hitInterval, hitInterval),
                                        (handler) =>
                                        {
                                            var _hitResult = handler.Arg2.HitUnit(handler.Arg1, handler.Arg3, handler.Arg4, false).HitResultType;
                                            handler.Arg5--;
                                            return _hitResult != UnitHitTool.HitResultType.HitFail;
                                        },
                                        (handler) => handler.Arg5 < 1 || !handler.Arg1.IsInteractValid(Unit.UnitStateType.DEAD) || !handler.Arg2.IsInteractValid(Unit.UnitStateType.DEAD),
                                        _FromUnit, targetUnit, hitMessage, _HitVariablePreset, extraHitCount
                                    );
                            }
                            // 멀티 히트 간격이 HitBasisTimeUnit 이하면 동시 타격으로 판정한다.
                            else
                            {
                                for (int j = 0; j < extraHitCount; j++)
                                {
                                    if (targetUnit.HitUnit(_FromUnit, hitMessage, _HitVariablePreset, false).HitResultType == UnitHitTool.HitResultType.HitFail)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                hitResult = UnitHitTool.HIT_NO_ONE(hitMessage.GetHitParameter());
                UnitHitTool.PlayHitFX(hitResult.HitResultType, _FromUnit, _FromUnit, hitResult);
            }
            
            return (filterResult, hitResult.HitResultType);
        }
        
        public void TryRetrieveHitObserver()
        {
            if (!_ProgressHBBFlag)
            {
                RetrieveObject();
            }
        }

        /// <summary>
        /// 최대 타격 횟수를 지정하는 메서드
        /// </summary>
        public void SetMaxHitCount(int p_Count)
        {
            MaxHitCount = p_Count;
        }

        /// <summary>
        /// 한 유닛을 대상으로 최대한 타격할 수 있는 횟수 및 간격을 지정하는 메서드
        /// </summary>
        public void SetSameUnitHitPreset((int t_Count, float t_Interval) p_SameUnitHitPreset)
        {
            SameUnitHitPreset = p_SameUnitHitPreset;
        }

        /// <summary>
        /// 지정한 타임스탬프 및 유닛에게 타격이 허용되는지 체크하는 논리 메서드
        /// </summary>
        public bool CheckHitValidation(Unit p_Unit, float p_TimeStamp)
        {
            // 최대 포착횟수를 검증한다.
            if (MaxHitCount > 0)
            {
                var sameUnitHitCount = SameUnitHitPreset.t_Count;
                if (sameUnitHitCount > 0)
                {
                    // 동일 유닛 최대 타격수를 검증한다.
                    if (UnitHitInfoPresetTable.TryGetValue(p_Unit, out var o_HitInfo))
                    {
                        if (o_HitInfo.HitCount < sameUnitHitCount)
                        {
                            var sameUnitHitInterval = SameUnitHitPreset.t_Interval;
                            if (p_TimeStamp - o_HitInfo.HitTimeStamp > sameUnitHitInterval)
                            {
                                o_HitInfo.UpdatePreset(p_TimeStamp);
                                UnitHitInfoPresetTable[p_Unit] = o_HitInfo;
                                MaxHitCount--;
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        UnitHitInfoPresetTable.Add(p_Unit, new UnitHitInfoPreset(1, p_TimeStamp));
                        MaxHitCount--;
                        return true;
                    }
                }
                else
                {
                    MaxHitCount--;
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}