using k514.xTrial;

namespace k514
{
    public partial class Unit
    {
        #region <Callbacks>
                
        private void TryHandle_Evolve_KillingEvent()
        {
            /*if (UnitExtraFlagMask.HasAnyFlagExceptNone(UnitTool.UnitExtraFlag.Evolve) && _PrefabModelKey.Item1)
            {
                var evolveTable = UnitEvolveTreeData.GetInstanceUnSafe.GetTable();
                if (evolveTable.TryGetValue(_PrefabModelKey.Item2, out var o_Record))
                {
                    if (KillPoint >= o_Record.NeedLevelUp)
                    {
                        var targetModelIndex = o_Record.EvolveTo;
                        UnitEvolveManager.GetInstance.EvolveUnit(this, targetModelIndex);
                        return;
                    }
                }
                else
                {
                    UnitExtraFlagMask.RemoveFlag(UnitTool.UnitExtraFlag.Evolve);
                }
            }*/
        }

        #endregion
    }
}