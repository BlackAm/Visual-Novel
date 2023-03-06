namespace k514
{
    public class PrefabExtraData_PetPetProjector : PrefabExtraData_ProjectorBase<PrefabExtraData_PetPetProjector, PrefabExtraData_PetPetProjector.PrefabExtraDataPPProjectorRecord>
    {
        public class PrefabExtraDataPPProjectorRecord : PrefabExtraDataProjectorBaseRecord
        {
            public int ImageIndex { get; private set; }
            public float MinScale { get; private set; }
            public float MaxScale { get; private set; }
            public float Interval { get; private set; }
            public int LoopCount { get; private set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "PetPetProjectorExtraDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 137500;
            EndIndex = 140000;
        }

        public override PrefabExtraDataRoot.PrefabExtraDataType GetThisLabelType()
        {
            return PrefabExtraDataRoot.PrefabExtraDataType.ProjectorPetPet;
        }
    }
}