namespace k514
{
    public class UnitRoleSlavePresetData : UnitRolePresetDataBase<UnitRoleSlavePresetData, UnitRoleSlavePresetData.RoleTableRecord>
    {
        public enum MasterType
        {
            /// <summary>
            /// 주인이 없음, 별도 함수로 주인을 지정해야함
            /// </summary>
            None,
            
            /// <summary>
            /// 처음 조우한 유닛이 주인이 됨
            /// </summary>
            FirstEncounter,
            
            /// <summary>
            /// 시스템 플레이어가 주인이 됨
            /// </summary>
            Player,
        }

        public class RoleTableRecord : RoleTableBaseRecord
        {
            public MasterType MasterType { get; private set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitRoleSlavePresetTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 100;
            EndIndex = 200;
        }

        public override UnitRoleDataRoot.UnitRoleType GetThisLabelType()
        {
            return UnitRoleDataRoot.UnitRoleType.Slave;
        }
    }
}