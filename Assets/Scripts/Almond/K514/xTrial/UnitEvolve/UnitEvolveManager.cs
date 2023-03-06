namespace k514.xTrial
{
    public class UnitEvolveManager : Singleton<UnitEvolveManager>
    {
        #region <Fields>

        protected override void OnCreated()
        {
        }

        public override void OnInitiate()
        {
        }

        #endregion

        #region <Callbacks>

        public async void EvolveUnit(Unit p_TargetUnit, int p_TargetSpawnIndex)
        {
            /*var targetUnitPosition = p_TargetUnit._Transform.position;
            var inheritAuthority = p_TargetUnit.UnitAuthorityFlagMask;
            p_TargetUnit.SetDead(true);
            await UnitSpawnManager.GetInstance.SpawnUnitAsync<Unit>(p_TargetSpawnIndex, targetUnitPosition, ResourceLifeCycleType.Scene, ObjectDeployTool.ObjectDeploySurfaceDeployType.ForceSurface, inheritAuthority);*/
        }

        #endregion
    }
}