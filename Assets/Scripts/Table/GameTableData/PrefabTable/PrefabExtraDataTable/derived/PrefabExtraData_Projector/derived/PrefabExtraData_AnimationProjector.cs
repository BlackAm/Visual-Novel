namespace BlackAm
{
    public class PrefabExtraData_AnimationProjector : PrefabExtraData_ProjectorBase<PrefabExtraData_AnimationProjector, PrefabExtraData_AnimationProjector.PrefabExtraDataAnimationProjectorRecord>
    {
        public class PrefabExtraDataAnimationProjectorRecord : PrefabExtraDataProjectorBaseRecord
        {
            public UITool.AnimationSpriteType AnimationSpriteType { get; private set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "AnimationProjectorExtraDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 135000;
            EndIndex = 137500;
        }

        public override PrefabExtraDataRoot.PrefabExtraDataType GetThisLabelType()
        {
            return PrefabExtraDataRoot.PrefabExtraDataType.ProjectorAnimation;
        }
    }
}