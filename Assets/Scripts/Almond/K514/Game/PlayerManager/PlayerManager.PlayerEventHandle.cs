#if !SERVER_DRIVE
using k514.xTrial;
using BDG;
using UI2020;
using UnityEngine;

namespace k514
{
    public partial class PlayerManager
    {
        #region <Callbacks>

        public void OnPlayerReachedDestination()
        {
            // LamiereGameManager.GetInstanceUnSafe.OnReachedDestination();
        }

        public void OnPlayerFocusChanged(Unit p_PrevFocusUnit, Unit p_FocusUnit)
        {
            // LamiereGameManager.GetInstanceUnSafe.SetEnemyTargetingDecalEnable(p_FocusUnit as LamiereUnit);
        }

        public void OnPlayerStriked(Unit p_Target, HitResult p_HitResult)
        {
            /*if (_BackingField_Player.FocusNode.IsNodeEquals(p_Target) && !p_Target.HasState_Or(Unit.UnitStateType.DEAD))
            {
                MainGameUI.Instance.mainUI.SetTargetUnit(p_Target);
            }*/
        }
         
        public void OnPlayerStrikedCritical(Unit p_Target, HitResult p_HitResult)
        {
            /*if (_BackingField_Player.FocusNode.IsNodeEquals(p_Target) && !p_Target.HasState_Or(Unit.UnitStateType.DEAD))
            {
                MainGameUI.Instance.mainUI.SetTargetUnit(p_Target);
            }*/
            // CameraManager.GetInstanceUnSafe.SetShake(Vector3.left, 5, 0, 300, 10);
        }
         
        public void OnPlayerHitted(Unit p_Trigger, HitResult p_HitResult)
        {
            // MainGameUI.Instance.mainUI.SetTargetUnit(p_Trigger);
        }
         
        public void OnPlayerHittedCritical(Unit p_Trigger, HitResult p_HitResult)
        {
            // MainGameUI.Instance.mainUI.SetTargetUnit(p_Trigger);
        }
                 
        public void OnPlayerStucked(Unit p_Target, HitResult p_HitResult)
        {
        }

        public void OnPlayerKilled(Unit p_Target)
        {
        }

        public void OnPlayerDead()
        {
        }
        
        public void OnPlayerPKModeSwitched(bool p_Flag)
        {
        }
        
        public void OnPlayerPKStateTransition(PK_Tool.PK_State p_State)
        {
        }

        #endregion
 
        #region <Callback/UIInteract>

        #endregion
    }
}
#endif