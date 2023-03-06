#if !SERVER_DRIVE
namespace k514
{
    public class UIPrefabSpawnData : PrefabSpawnDataBase<UIPrefabSpawnData, UIPrefabSpawnData.SpawnTableRecord>
    {
        public class SpawnTableRecord : SpawnTableRecordBase
        {
            
        }

        protected override string GetDefaultTableFileName()
        {
            return "UIPrefabSpawnDataTable";
        }
    }
}
#endif