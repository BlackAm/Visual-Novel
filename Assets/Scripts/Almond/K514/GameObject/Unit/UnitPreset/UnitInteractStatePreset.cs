namespace k514
{
    public class UnitInteractStatePreset
    {
        #region <Fields>

        public UnitCombatStatePreset _CombatInfo;
        
        #endregion

        #region <Methods>

        public void UpdateCombatState(UnitCombatStatePreset p_CombatInfo)
        {
            _CombatInfo = p_CombatInfo;
        }

        public void Reset()
        {
            _CombatInfo = default;
        }

        #endregion
    }
}