#if !SERVER_DRIVE
using System;

namespace k514
{
    public static class TouchEventTool
    {
    }

    public class TouchEventRoot : MultiTableProxy<TouchEventRoot, int, TouchEventRoot.TouchMappingKeyCodeType, IIndexableTouchEventTableBridge, IIndexableTouchEventRecordBridge>
    {
        #region <Enums>

        [Flags]
        public enum TouchInputType
        {
            None = 0,
            KeyCodeEvent = 1 << 0,
            UnitClickEvent = 1 << 1,
        }
        public static readonly TouchInputType[] _TouchEventTypeEnumerator;

        [Flags]
        public enum TouchEventType
        {
            None = 0,
            PlayerSelected = 1 << 0,
            UnitSelected = 1 << 1,
            PositionSelected = 1 << 2,
        }

        
        public enum TouchMappingKeyCodeType
        {
            UnitControl,
            ViewControl,
            UnitSpell,
            SystemControl,
        }
        
        #endregion

        #region <Constructor>

        static TouchEventRoot()
        {
            _TouchEventTypeEnumerator = SystemTool.GetEnumEnumerator<TouchInputType>(SystemTool.GetEnumeratorType.ExceptMaskNone);
        }

        #endregion

        #region <Methods>

        protected override MultiTableIndexer<int, TouchMappingKeyCodeType, IIndexableTouchEventRecordBridge> SpawnGameDataTableCluster()
        {
            return new IntegerMultiTableIndexer<TouchMappingKeyCodeType, IIndexableTouchEventRecordBridge>();
        }
        
        #endregion
    }
}
#endif