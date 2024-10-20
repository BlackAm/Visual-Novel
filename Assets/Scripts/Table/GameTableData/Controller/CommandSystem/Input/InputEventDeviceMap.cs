using Cysharp.Threading.Tasks;

namespace BlackAm
{
    public class InputEventDeviceMap : GameTable<InputEventDeviceMap, ControllerTool.InputEventType, InputEventDeviceMap.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public ControllerTool.InputControllerType DeviceType { get; private set; }

#if SERVER_DRIVE
            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();

                switch (DeviceType)
                {
                    case ControllerTool.InputControllerType.TouchPanel:
                        DeviceType = ControllerTool.InputControllerType.Keyboard;
                        break;
                    case ControllerTool.InputControllerType.Keyboard_TouchPanel:
                        DeviceType = ControllerTool.InputControllerType.Keyboard;
                        break;
                }
            }
#endif
        }

        protected override string GetDefaultTableFileName()
        {
            return "InputEventDeviceMap";
        }
 
        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}