using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace k514
{
    public abstract class UnitActablePresetDataBase<M, T> : MultiTableBase<M, int, T, UnitActionDataRoot.ActableType, IActableTableRecordBridge>, IActableTableBridge
        where M : UnitActablePresetDataBase<M, T>, new()
        where T : UnitActablePresetDataBase<M, T>.ActableTableRecordBase, new()
    {
        public abstract class ActableTableRecordBase : GameTableRecordBase, IActableTableRecordBridge
        {
            /// <summary>
            /// 해당 유닛의 액션 타입에 적용할 유닛액션 인덱스 정보
            /// </summary>
            public Dictionary<ControllerTool.CommandType, ActableTool.UnitActionCluster> UnitActionRecords { get; protected set; }

            /// <summary>
            /// 기본으로 사용할 커맨드 타입
            /// </summary>
            public ControllerTool.CommandType DefaultCommand { get; protected set; }

            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();
                
                if (UnitActionRecords.CheckGenericCollectionSafe())
                {
                    if (DefaultCommand == default)
                    {
                        DefaultCommand = UnitActionRecords.Keys.First();
                    }
                }
                else
                {
                    DefaultCommand = ControllerTool.CommandType.None;
                }
            }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}