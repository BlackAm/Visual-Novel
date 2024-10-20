#if !SERVER_DRIVE
using UnityEngine;

namespace BlackAm
{
    public abstract class TouchEventCommandTableBase<M, T> 
        : MultiTableBase<M, int, T, TouchEventRoot.TouchMappingKeyCodeType, IIndexableTouchEventRecordBridge>, IIndexableTouchEventTableBridge
        where M : TouchEventCommandTableBase<M, T>, new() 
        where T : TouchEventCommandTableBase<M, T>.TouchEventRecordBase, new()
    {
        public abstract class TouchEventRecordBase : GameTableRecordBase, IIndexableTouchEventRecordBridge
        {
            public KeyCode KeyCode { get; protected set; }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
        
        public override MultiTableIndexer<int, TouchEventRoot.TouchMappingKeyCodeType, IIndexableTouchEventRecordBridge> GetMultiGameIndex()
        {
            return TouchEventRoot.GetInstanceUnSafe.GameDataTableCluster;
        }
    }
    
    public class UnitControlTouchEventCommandTable : TouchEventCommandTableBase<UnitControlTouchEventCommandTable, UnitControlTouchEventCommandTable.TouchEventRecord>
    {
        public class TouchEventRecord : TouchEventRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitControlTouchEventCommand";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 0;
            EndIndex = 50;
        }

        public override TouchEventRoot.TouchMappingKeyCodeType GetThisLabelType()
        {
            return TouchEventRoot.TouchMappingKeyCodeType.UnitControl;
        }
    }
    
    public class ViewControlTouchEventCommandTable : TouchEventCommandTableBase<ViewControlTouchEventCommandTable, ViewControlTouchEventCommandTable.TouchEventRecord>
    {
        public class TouchEventRecord : TouchEventRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "ViewControlTouchEventCommand";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 50;
            EndIndex = 100;
        }

        public override TouchEventRoot.TouchMappingKeyCodeType GetThisLabelType()
        {
            return TouchEventRoot.TouchMappingKeyCodeType.ViewControl;
        }
    }
        
    public class SystemControlTouchEventCommandTable : TouchEventCommandTableBase<SystemControlTouchEventCommandTable, SystemControlTouchEventCommandTable.TouchEventRecord>
    {
        public class TouchEventRecord : TouchEventRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "SystemControlTouchEventCommand";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 100;
            EndIndex = 150;
        }

        public override TouchEventRoot.TouchMappingKeyCodeType GetThisLabelType()
        {
            return TouchEventRoot.TouchMappingKeyCodeType.SystemControl;
        }
    }

    public class DialogueControlTouchEventCommandTable : TouchEventCommandTableBase<
        DialogueControlTouchEventCommandTable, DialogueControlTouchEventCommandTable.TouchEventRecord>
    {
        public class TouchEventRecord : TouchEventRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "DialogueControlTouchEventCommand";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 150;
            EndIndex = 200;
        }

        public override TouchEventRoot.TouchMappingKeyCodeType GetThisLabelType()
        {
            return TouchEventRoot.TouchMappingKeyCodeType.DialogueControl;
        }
    }
}
#endif