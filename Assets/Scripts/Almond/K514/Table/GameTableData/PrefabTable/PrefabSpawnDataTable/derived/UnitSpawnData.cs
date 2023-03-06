namespace k514
{
    public class UnitSpawnData : PrefabSpawnDataBase<UnitSpawnData, UnitSpawnData.SpawnTableRecord>
    {
        public class SpawnTableRecord : SpawnTableRecordBase
        {
            
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitSpawnDataTable";
        }
    }
}