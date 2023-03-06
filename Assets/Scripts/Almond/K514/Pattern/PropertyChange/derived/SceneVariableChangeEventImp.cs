using System;

namespace k514
{
    public class SceneVariableChangeEventSender : PropertyModifyEventSenderImp<SceneEnvironmentManager.SceneVariableEventType, SceneVariableData.TableRecord>
    {
        public override bool HasEvent(SceneEnvironmentManager.SceneVariableEventType p_Type, SceneEnvironmentManager.SceneVariableEventType p_Compare)
        {
            return p_Type.HasAnyFlagExceptNone(p_Compare);
        }
    }
    
    public class SceneVariableChangeEventReceiver : PropertyModifyEventReceiverImp<SceneEnvironmentManager.SceneVariableEventType, SceneVariableData.TableRecord>
    {
        #region <Constructor>

        public SceneVariableChangeEventReceiver() : base()
        {
        }

        public SceneVariableChangeEventReceiver(SceneEnvironmentManager.SceneVariableEventType p_EventType,
            Action<SceneEnvironmentManager.SceneVariableEventType, SceneVariableData.TableRecord> p_EventHandler) : base(p_EventType, p_EventHandler)
        {
        }

        #endregion
    }
}