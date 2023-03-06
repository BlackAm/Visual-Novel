using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public class TestSceneEnvironment : LamierePlayerDeploySceneEnvironment
    {
        /// <summary>
        /// 플레이어 UnitStateType.IMMORTAL 설정하는 곳 플레이어가 데미지를 받으려면 false 설정해서 IMMORTAL을 비활성화함
        /// </summary>
        bool _Immortal = false;

        public override async UniTask OnScenePreload()
        {
            await base.OnScenePreload();
            
#if !SERVER_DRIVE
            if(_Immortal) 
            {
                LamiereGameManager.GetInstanceUnSafe._ClientPlayer.AddState(Unit.UnitStateType.IMMORTAL);
            }
#endif
        }

        public override void OnSceneStarted()
        {
            base.OnSceneStarted();
            
#if !SERVER_DRIVE
            LamiereGameManager.GetInstanceUnSafe._ClientPlayer.SwitchActable(UnitActionDataRoot.ActableType.PassivePhaseTransition);
#endif
        }
    }
}
