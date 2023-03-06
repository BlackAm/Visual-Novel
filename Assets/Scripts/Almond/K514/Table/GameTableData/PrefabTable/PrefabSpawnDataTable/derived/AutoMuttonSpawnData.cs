namespace k514
{
    public class AutoMuttonSpawnData : PrefabSpawnDataBase<AutoMuttonSpawnData, AutoMuttonSpawnData.SpawnTableRecord>
    {
        public class SpawnTableRecord : SpawnTableRecordBase
        {
            
        }

        protected override string GetDefaultTableFileName()
        {
            return "AutoMuttonSpawnDataTable";
        }
    }
}