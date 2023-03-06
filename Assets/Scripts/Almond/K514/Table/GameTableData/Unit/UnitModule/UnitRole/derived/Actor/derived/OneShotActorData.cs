namespace k514
{
    public class OneShotActorData : UnitRoleActorPresetDataBase<OneShotActorData, OneShotActorData.ActorTableRecord>
    {
        public class ActorTableRecord : ActorTableBaseRecord
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "OneShotActorDataTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 1000;
            EndIndex = 1100;
        }

        public override UnitRoleDataRoot.UnitRoleType GetThisLabelType()
        {
            return UnitRoleDataRoot.UnitRoleType.OneShotActor;
        }
    }
}