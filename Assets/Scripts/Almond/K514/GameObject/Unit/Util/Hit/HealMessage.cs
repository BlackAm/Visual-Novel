using k514;

public struct HpRecovery
{
    /*#region <Fields>

    /// <summary>
    /// 타격 유닛
    /// </summary>
    public UnitPropertyChangePreset Trigger;
    
    /// <summary>
    /// 피격 유닛
    /// </summary>
    public Unit Target;

    /// <summary>
    /// 연산 회복량
    /// </summary>
    public float ResultHeal;

    #endregion

    public HpRecovery(UnitPropertyChangePreset p_Trigger, Unit p_Target, float p_ResultHeal){
        Trigger = p_Trigger;
        Target = p_Target;
        ResultHeal = p_ResultHeal;
    }

    public void ApplyHeal()
    {
//#if SERVER_DRIVE
//        Target.AddValue(BattleStatusPreset.BattleStatusType.HP_Base, new UnitPropertyChangePreset(Trigger), ResultHeal);
//#else
        Target.AddValue(BattleStatusPreset.BattleStatusType.HP_Base, Trigger, ResultHeal);
//#endif
    }
}

public struct MPRecovery
{
    #region <Fields>

    /// <summary>
    /// 타격 유닛
    /// </summary>
    public UnitPropertyChangePreset Trigger;
    
    /// <summary>
    /// 피격 유닛
    /// </summary>
    public Unit Target;

    /// <summary>
    /// 연산 회복량
    /// </summary>
    public float ResultMana;

    #endregion

    public MPRecovery(UnitPropertyChangePreset p_Trigger, Unit p_Target, float p_ResultMana){
        Trigger = p_Trigger;
        Target = p_Target;
        ResultMana = p_ResultMana;
    }

    public void ApplyMana()
    {
//#if SERVER_DRIVE
//        Target.AddValue(BattleStatusPreset.BattleStatusType.MP_Base, new UnitPropertyChangePreset(Trigger), ResultMana);
//#else
        Target.AddValue(BattleStatusPreset.BattleStatusType.MP_Base, Trigger, ResultMana);
//#endif
    }*/
}