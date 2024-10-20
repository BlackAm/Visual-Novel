namespace BlackAm
{
    public class VfxSpawnData : PrefabSpawnDataBase<VfxSpawnData, VfxSpawnData.SpawnTableRecord>
    {
        public class SpawnTableRecord : SpawnTableRecordBase
        {
            
        }

        protected override string GetDefaultTableFileName()
        {
            return "VfxSpawnDataTable";
        }
    }
}