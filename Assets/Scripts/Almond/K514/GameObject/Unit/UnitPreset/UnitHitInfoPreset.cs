namespace k514
{
    public struct UnitHitInfoPreset
    {
        #region <Fields>

        /// <summary>
        /// 타격된 횟수
        /// </summary>
        public int HitCount;

        /// <summary>
        /// 타격된 시점
        /// </summary>
        public float HitTimeStamp;

        #endregion

        #region <Constructors>

        public UnitHitInfoPreset(int p_HitCount, float p_HitTimeStamp)
        {
            HitCount = p_HitCount;
            HitTimeStamp = p_HitTimeStamp;
        }

        #endregion

        #region <Methods>

        public void UpdatePreset(float p_TimeStamp)
        {
            HitCount++;
            HitTimeStamp = p_TimeStamp;
        }

        #endregion
    }
}