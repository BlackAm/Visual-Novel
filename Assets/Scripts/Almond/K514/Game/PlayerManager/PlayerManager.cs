#if !SERVER_DRIVE
using System;
using Cysharp.Threading.Tasks;

namespace k514
{
    public partial class PlayerManager : PropertySceneChangeEventSingleton<PlayerChangeEventSender, PlayerManager.PlayerChangeEventType, Unit, PlayerManager>
    {
        #region <Fields>

        /// <summary>
        /// 플레이어 유닛의 백엔드
        /// </summary>
        private Unit _BackingField_Player;


        /// <summary>
        /// 현재 선정된 플레이어 유닛
        /// 프로퍼티 접근자를 통해 관련 필드들을 초기화하는 기능을 가진다.
        /// </summary>
        public Unit Player
        {
            get => _BackingField_Player.IsValid()
                ? _BackingField_Player
                : null;
            
            set
            {
                if (!ReferenceEquals(_BackingField_Player, value))
                {
                    var prevUnit = _BackingField_Player;
                    _BackingField_Player = value;
                    OnPlayerChanged(prevUnit);
                }
            }
        }

        #endregion
 
        #region <Enums>
 
        [Flags]
        public enum PlayerChangeEventType
        {
            None = 0,
            PlayerChanged = 1 << 0,
        }

        #endregion
       
        #region <Operator>

        public static implicit operator bool(PlayerManager p_PlayerManager) => p_PlayerManager.Player.IsValid();
        public static implicit operator Unit(PlayerManager p_PlayerManager) => p_PlayerManager.Player;

        #endregion

        #region <Callback>

        protected override void OnCreated()
        {
            SingletonTool.CreateSingleton(typeof(CameraManager));
            SingletonTool.CreateSingleton(typeof(LamiereGameManager));

            OnCreated_PlayerEventReceivePartial();
        }
 
        public override void OnInitiate()
        {
        }
 
        private void OnPlayerChanged(Unit p_Prev)
        {
            if (p_Prev.IsValid())
            {
                p_Prev.SwitchPersona(UnitAIDataRoot.UnitMindType.Autonomy_MeleeSimple);
            }
            
            if (_BackingField_Player.IsValid())
            {
                _BackingField_Player.SwitchPersona(UnitAIDataRoot.UnitMindType.Passivity_Base);

                OnUpdatePlayer();
            }
            else
            {
                TriggerPropertyEvent(PlayerChangeEventType.PlayerChanged, _BackingField_Player);
            }
        }

        public void OnPlayerReleased(Unit p_Target)
        {
            if (ReferenceEquals(_BackingField_Player, p_Target))
            {
                Player = null;
            }
        }

        private void OnUpdatePlayer()
        {
            TriggerPropertyEvent(PlayerChangeEventType.PlayerChanged, _BackingField_Player);
                
            /*UpdatePlayerInfo();
            UpdatePlayerLevel(_BackingField_Player.GetCurrentLevel());
            UpdatePlayerExp(_BackingField_Player.GetCurrentExpRate());
            UpdatePlayerBaseStatus();
            UpdatePlayerBattleStatus();*/
        }

        public override async UniTask OnScenePreload()
        {
            await UniTask.CompletedTask;
        }
 
        public override void OnSceneStarted()
        {
            DeployPlayer();
        }
 
        public override void OnSceneTerminated()
        {
        }
         
        public override void OnSceneTransition()
        {
        }
 
        #endregion

        #region <Methods>

        public bool IsPlayerValid() => _BackingField_Player.IsValid();

        public bool TryGetPlayer(out Unit o_Player)
        {
            o_Player = _BackingField_Player;
            return _BackingField_Player.IsValid();
        }
        
        public void DeployPlayer()
        {
            if (_BackingField_Player.IsValid())
            {
                var (t_Pos, t_Rot) = SceneEnvironmentManager.GetInstance.GetPlayerStartPreset();
                _BackingField_Player.ApplyAffinePreset(t_Pos, t_Rot, true);
                OnUpdatePlayer();
            }
        }

        #endregion
    }
}
#endif