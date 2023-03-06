namespace k514
{
    public struct UnitCombatStatePreset
    {
        #region <Fields>

        public UnitTool.UnitCombatInfoType InfoType;
        public Unit Striker;
        public Unit Victim;
        public float TimeStamp;
        
        #endregion

        #region <Constructor>

        public UnitCombatStatePreset(UnitTool.UnitCombatInfoType p_InfoType, Unit p_Striker, Unit p_Victim, float p_TimeStamp)
        {
            InfoType = p_InfoType;
            Striker = p_Striker;
            Victim = p_Victim;
            TimeStamp = p_TimeStamp;
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// 해당 전투 정보가 유효한지 검증한다.
        /// 전투 정보는 전투 발생으로부터 일정시간 유효하다.
        /// </summary>
        public bool IsTimeStampValid(float p_TimeStamp)
        {
            return p_TimeStamp - TimeStamp < UnitTool.__CombatInfo_ValidTime_UpperBound;
        }
        
        /// <summary>
        /// 지정한 유닛으로부터 전투가 시작됬는지 검증한다.
        /// </summary>
        public bool IsStartCombatFrom(Unit p_Unit)
        {
            return ReferenceEquals(Striker, p_Unit);
        }

        #endregion
    }
}