namespace k514
{
    /// <summary>
    /// 애니메이션 모델 프리팹을 기술하는 테이블
    /// 지정 레코드 정수 인덱스 50 ~ 9999
    /// </summary>
    public class PrefabModelData_LamiereComputerModel : PrefabModelData_UnitModelBase<PrefabModelData_LamiereComputerModel, PrefabModelData_LamiereComputerModel.TableRecord>, PrefabModelDataTableBridge
    {
        public class TableRecord : UnitBaseTableRecord
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "ComputerModelPrefabDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 50;
            EndIndex = 10000;
        }

        public override PrefabModelDataRoot.PrefabModelDataType GetThisLabelType()
        {
            return PrefabModelDataRoot.PrefabModelDataType.ComputerModel;
        }
    }
}