namespace k514
{
    public class ProjectorSpawnData : PrefabSpawnDataBase<ProjectorSpawnData, ProjectorSpawnData.SpawnTableRecord>
    {
        public class SpawnTableRecord : SpawnTableRecordBase
        {
            
        }

        protected override string GetDefaultTableFileName()
        {
            return "ProjectorSpawnDataTable";
        }
    }
}