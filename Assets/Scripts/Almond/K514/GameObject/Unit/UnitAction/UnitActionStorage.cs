using System.Collections.Generic;
using System.Linq;

namespace k514
{
    public class UnitActionStorage : Singleton<UnitActionStorage>
    {
        #region <Fields>
 
        private Dictionary<int, UnitActionTool.UnitAction> _UnitActionRecord;
        public ActableTool.UnitActionType[] _ActionTypeEnumerator;
        public UnitActionTool.UnitAction.UnitActionFlag[] _ActionEntryConditionEnumerator;
         
        #endregion
 
        #region <Indexes>
 
        public UnitActionTool.UnitAction this[int p_Index] => _UnitActionRecord[p_Index];
 
        #endregion
         
        #region <Callbacks>

        protected override void OnCreated()
        {
            _UnitActionRecord = UnitActionPresetData.GetInstanceUnSafe.GetTable().Keys
                .ToDictionary(key => key, key => new UnitActionTool.UnitAction(key));
            _ActionTypeEnumerator = SystemTool.GetEnumEnumerator<ActableTool.UnitActionType>(SystemTool.GetEnumeratorType.ExceptNone);
            _ActionEntryConditionEnumerator = SystemTool.GetEnumEnumerator<UnitActionTool.UnitAction.UnitActionFlag>(SystemTool.GetEnumeratorType.ExceptMaskNone);
        }
 
        public override void OnInitiate()
        {
        }
 
        public void OnActionBind(int p_ActionIndex)
        {
            OnActionBind(this[p_ActionIndex]);
        }
         
        public void OnActionBind(UnitActionTool.UnitAction p_Action)
        {
            var eventPresetMap = p_Action._UnitActionPresetRecord.EventPresetMap;
            if (eventPresetMap != null)
            {
                foreach (var eventPresetCollectionKV in eventPresetMap)
                {
                    var eventPresetCollection = eventPresetCollectionKV.Value;
                    foreach (var eventPresetKV in eventPresetCollection)
                    {
                        var eventPreset = eventPresetKV.Value;
                        eventPreset.Preload();
                    }
                }
            }
        }
 
        #endregion
         
        #region <Methods>
 
        public UnitActionTool.UnitAction GetUnitAction(int p_Index)
        {
            return this[p_Index];
        }
 
        public (ControllerTool.CommandType, int) FindUnitAction(Unit p_Target, int p_ActionIndex)
        {
            var tryModule = p_Target._ActableObject;
            if (!ReferenceEquals(null, tryModule))
            {
                return tryModule.FindAction(p_ActionIndex);
            }
            else
            {
                return (ControllerTool.CommandType.None, -2);
            }
        }
 
        #endregion
    }
}