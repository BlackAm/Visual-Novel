#if !SERVER_DRIVE
namespace BlackAm
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