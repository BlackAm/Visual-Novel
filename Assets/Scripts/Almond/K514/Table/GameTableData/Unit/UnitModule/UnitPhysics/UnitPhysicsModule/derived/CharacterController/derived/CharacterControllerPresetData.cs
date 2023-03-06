namespace k514
{
    public class CharacterControllerPresetData : CharacterControllerPresetDataBase<CharacterControllerPresetData, CharacterControllerPresetData.PhysicsTableRecord>
    {
        public class PhysicsTableRecord : CharacterControllerTableBaseRecord
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "CharacterControllerPresetDataTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 50;
            EndIndex = 100;
        }

        public override UnitPhysicsDataRoot.UnitPhysicsType GetThisLabelType()
        {
            return UnitPhysicsDataRoot.UnitPhysicsType.CharacterController;
        }
    }
}