namespace k514
{
    /// <summary>
    /// 애니메이션 모델 프리팹을 기술하는 테이블
    /// 지정 레코드 정수 인덱스 0 ~ 15
    /// </summary>
    public class PrefabModelData_LamierePlayerModel : PrefabModelData_UnitModelBase<PrefabModelData_LamierePlayerModel, PrefabModelData_LamierePlayerModel.TableRecord>, PrefabModelDataTableBridge
    {
        public class TableRecord : UnitBaseTableRecord
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "PlayerModelPrefabDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 0;
            EndIndex = 16;
        }

        public override PrefabModelDataRoot.PrefabModelDataType GetThisLabelType()
        {
            return PrefabModelDataRoot.PrefabModelDataType.PlayerModel;
        }
    }
}