#if SERVER_DRIVE
using UnityEngine;

namespace BlackAm
{
    public partial class SystemBoot
    {
        #region <Consts>

        private const float __LOG_INTERVAL = 20f;

        #endregion
        
        #region <Fields>

        private float _AccumulatedTime;

        #endregion

        #region <Callbacks>

        private void OnCreated_ServerDrive()
        {
        }

        private void OnUpdate_ServerDrive(float p_DeltaTime)
        {
            if (_AccumulatedTime > __LOG_INTERVAL)
            {
                Debug.Log($"[Log] Fps({HeadlessServerManager.GetInstance.CurrentSceneEntryIndex}/{HeadlessServerManager.GetInstance.CurrentChannelIndex}) : {1f / Time.unscaledDeltaTime}, Spawned : [{UnitInteractManager.GetInstance.GetUnitNumberInfo()}]");
                _AccumulatedTime = 0f;
            }
            else
            {
                _AccumulatedTime += p_DeltaTime;
            }
        }

        #endregion
    }
}
#endif