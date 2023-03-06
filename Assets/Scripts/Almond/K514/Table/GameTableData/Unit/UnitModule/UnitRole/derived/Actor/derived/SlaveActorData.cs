namespace k514
{
    public class SlaveActorData : UnitRoleActorPresetDataBase<SlaveActorData, SlaveActorData.ActorTableRecord>
    {
        public class ActorTableRecord : ActorTableBaseRecord
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "SlaveActorDataTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 1100;
            EndIndex = 1200;
        }

        public override UnitRoleDataRoot.UnitRoleType GetThisLabelType()
        {
            return UnitRoleDataRoot.UnitRoleType.SlaveActor;
        }
    }
}