using System.Collections.Generic;

namespace k514
{
    public interface IUnitHitEventTrace
    {
        /// <summary>
        /// 타격정보 컬렉션
        /// </summary>
        Dictionary<Unit, UnitHitInfoPreset> UnitHitInfoPresetTable { get; }
        
        /// <summary>
        /// 최대 타격횟수
        /// </summary>
        int MaxHitCount { get; }
        
        /// <summary>
        /// 한 유닛을 대상으로 최대한 타격할 수 있는 횟수 및 간격
        /// </summary>
        (int t_Count, float t_Interval) SameUnitHitPreset { get; }

        /// <summary>
        /// 최대 타격 횟수를 지정하는 메서드
        /// </summary>
        void SetMaxHitCount(int p_Count);

        /// <summary>
        /// 한 유닛을 대상으로 최대한 타격할 수 있는 횟수 및 간격을 지정하는 메서드
        /// </summary>
        void SetSameUnitHitPreset((int t_Count, float t_Interval) p_SameUnitHitPreset);
        
        /// <summary>
        /// 지정한 타임스탬프 및 유닛에게 타격이 허용되는지 체크하는 논리 메서드
        /// </summary>
        bool CheckHitValidation(Unit p_Unit, float p_TimeStamp);
    }
    
    
#if UNITY_EDITOR
    public class IUnitHitEventTraceImp : IUnitHitEventTrace
    {
        #region <Fields>

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

        #endregion

        #region <Methods>

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
#endif
}