#if !SERVER_DRIVE
using System;
using Cysharp.Threading.Tasks;
using k514;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI2020
{
    /*public class ActionButton : KeyCodeTouchEventSenderBase
    {
        private Button _button;
        public Image _actionImage, _coolTimeMask, skillImage;
        public RectTransform _actionRect;
        public float saveCoolTime = 0f;
        public Image effect;
        public UI2DSpriteAnimation _animation;
        public Image chatImage;
        private Sprite noneSprite, emptySprite;

        public bool isEmpty = true;
        private float coolDownTime = 0;

        public void ResetSlot()
        {
            _coolTimeMask.sprite = _actionImage.sprite = noneSprite;
            skillImage.sprite = emptySprite;
            isEmpty = true;
            SetCoolTime(0);
            coolDownTime = 0;
            _animation.Pause();
        }

        public override void OnSpawning()
        {
            base.OnSpawning();
            noneSprite = LoadAssetManager.GetInstanceUnSafe.LoadAsset<Sprite>(ResourceType.Image,
                ResourceLifeCycleType.WholeGame, "MainUI_lobby_button_skill_1.png").Item2;

            emptySprite = LoadAssetManager.GetInstanceUnSafe.LoadAsset<Sprite>(ResourceType.Image, ResourceLifeCycleType.WholeGame, "none.png").Item2;
            skillImage = transform.Find("Skill").GetComponent<Image>();
            
            _button = GetComponent<Button>();
            _actionImage = GetComponent<Image>();
            if (transform.name.Equals("NormalAttack"))
            {
                chatImage = transform.Find("Image").GetComponent<Image>();
                chatImage.gameObject.SetActive(false);
            }
            _actionRect = _actionImage.GetComponent<RectTransform>();
            _coolTimeMask = transform.Find("CoolTimeMask").GetComponent<Image>();
            SetCoolTime(0);
            coolDownTime = 0;
            if (ThisInputEvent == ControllerTool.InputEventType.ControlUnit
                && TouchEventFlagMask == TouchEventRoot.TouchInputType.KeyCodeEvent)
            {
                TouchEventManager.GetInstance.OnRegistActionButton(SoloCommandType, this);
            }

            effect = transform.Find("Effect").GetComponent<Image>();
            _animation = transform.gameObject.AddComponent<UI2DSpriteAnimation>();
            _animation.Init();
            _animation.frames = LoadAssetManager.GetInstanceUnSafe.LoadMultipleAsset<Sprite>(ResourceType.Image,
                ResourceLifeCycleType.WholeGame, "PublicIcon_IconSkillSelect.png").Item2;
            _animation.mImageSprite = effect;
            _animation.framerate = 70;
            SetEffect(false);
        }

        /// 채팅 이미지로 변경합니다.
        public void SetChatButton(bool active, Sprite image = null)
        {
            chatImage.gameObject.SetActive(active);
            if (chatImage.gameObject.activeSelf)
            {
                _actionImage.sprite = noneSprite;
                skillImage.sprite = emptySprite;
                isEmpty = true;
            }
            else
            {
                _actionImage.sprite = skillImage.sprite = image;
                isEmpty = false;
            }
        }
        
        public void SetEffect(bool active)
        {
            if (active)
            {
                if (LamiereGameManager.GetInstanceUnSafe.GetCurrentPlayerSkill(KeyCodeCommandMapData.GetInstanceUnSafe.GetTableData((KeyCode) ButtonKeyCode).SoloCommandCode) != null)
                {
                    _animation.Play();
                }
            }
            else
            {
                _animation.Pause();
            }

            effect.enabled = active;
        }

        public void SetImage()
        {
            // 커맨드 타입
            var soloCommandType = KeyCodeCommandMapData.GetInstanceUnSafe.GetTableData((KeyCode)ButtonKeyCode).SoloCommandCode;

            // 기본공격 버튼의 경우 NPC와 가까이 있지 않을 때 이미지 최신화.
            if (transform.name.Equals("NormalAttack"))
            {
                if(chatImage.IsActive()) return;
            }
            
            if (tableRecord == null)
            {
                // 스킬이 등록되어 있지 않은 경우 비어있는 이미지 적용.
                /*_coolTimeMask.sprite = _actionImage.sprite = noneSprite;
                isEmpty = true;#1#
                ResetSlot();
            }
            else
            {
                var skillLangRecord = SkillPropertyLanguage.GetInstanceUnSafe.GetSkillIconSprite(tableRecord.ActionLanguage);
                _coolTimeMask.sprite = _actionImage.sprite = skillImage.sprite = skillLangRecord.Item1.Asset as Sprite;
                isEmpty = false;

                // 해당 이미지를 언로드하고자 하는 경우, 아래 코드를 활용할 것.
                // LoadAssetManager.GetInstanceUnSafe.UnloadAsset(skillLangRecord);
            }
        }

        /// <summary>
        /// 쿨타임 표시를 변경 (0~1)
        /// </summary>
        public void SetCoolTime(float coolTime)
        {
            OnUpdate();
           // _coolTimeMask.fillAmount = coolTime;
           // if (coolTime > 0)
            {
            //    if (!_animation.isPlaying) _animation.Play();
            }
        }

        void OnUpdate()
        {
            if(!ReferenceEquals(tableRecord,null) && coolDownTime != 0)
            {
                float coolTime = 0;

                coolDownTime += Time.deltaTime;

                coolTime = 1 - (coolDownTime / (tableRecord.ActionCoolDown));
                _coolTimeMask.fillAmount = coolTime;
                if (coolTime > 0)
                {
                    //if (_animation.isPlaying && !effect.enabled) SetEffect(true);
                    if (!_animation.isPlaying) _animation.Play();
                }
                if (coolTime <= 0) coolDownTime = 0;
            }
        }

        protected override void OnKeyCodeEventPointerDown(PointerEventData p_EventData)
        {
            // momo6346 - 아직 스킬이 등록되지 않음 = 스킬이미지 없음 = 스킬액션 안되게 조작.
            if (MainGameUI.Instance.infoUI.IsActive)
            {
                if (chatImage != null && chatImage.IsActive())
                {
                    if (MainGameUI.Instance.infoUI.sceneState < 22)
                    {
                        MainGameUI.Instance.infoUI.NextScene();
                    }
                    return;
                }
            }
            var soloCommandType = KeyCodeCommandMapData.GetInstanceUnSafe.GetTableData((KeyCode)ButtonKeyCode).SoloCommandCode;
            if (!(LamiereGameManager.GetInstanceUnSafe._ClientPlayer._ActableObject as ActableBase).GetIsSkill(soloCommandType))
            {
                return;
            }

            // 쿨타임 이면 사용안됨.
            if (_coolTimeMask.fillAmount > 0)
            {
                return;
            }
            
            if (LamiereGameManager.GetInstanceUnSafe.IsQuest)
            {
                LamiereGameManager.GetInstanceUnSafe.ResetQuest(LamiereGameManager.PlayerAutoMode.Passive);
                if (LamiereGameManager.GetInstanceUnSafe.ShowTouchedMovementWay)
                {
                    if (LamiereGameManager.GetInstanceUnSafe._ClientPlayer.HasTargetPosition())
                    {
                        LamiereGameManager.GetInstanceUnSafe._ClientPlayer.SetTargetPosition(Vector3.zero);
                        LamiereGameManager.GetInstanceUnSafe.ReSearchWayPoint = false;
                    }
                }
            }
            var lamiereAutomodeState = LamiereGameManager.GetInstanceUnSafe._AutoPlayMode;
            if (LamiereGameManager.GetInstanceUnSafe._ClientPlayer._ActableObject.IsUnWeaponModule() &&
                soloCommandType != ControllerTool.CommandType.X)
            {
                return;
            }
            switch (lamiereAutomodeState)
            {
                case LamiereGameManager.PlayerAutoMode.Passive:
                    if ((LamiereGameManager.GetInstanceUnSafe._ClientPlayer._ActableObject as ActableBase).GetIsSkill(soloCommandType) && !LamiereGameManager.GetInstanceUnSafe.SwitchAutoModeOneKill(null, soloCommandType))
                    {
                        base.OnKeyCodeEventPointerDown(p_EventData);
                    }
                    break;
                case LamiereGameManager.PlayerAutoMode.OneKillAutoMode:
                    // 적을 타겟팅 했었다면 해당 적을 공격합니다.
                    if (LamiereGameManager.GetInstanceUnSafe._TourchTarget != null)
                    {
                        LamiereGameManager.GetInstanceUnSafe.SwitchAutoModeOneKill(
                            LamiereGameManager.GetInstanceUnSafe._TourchTarget, 
                            soloCommandType);
                    }
                    else
                    {
                        LamiereGameManager.GetInstanceUnSafe.ReserveAICommand(soloCommandType, ThinkableTool.AIReserveCommand.SpellEntry_Instant | ThinkableTool.AIReserveCommand.TurnToDefaultCommand_OnceFlag, ThinkableTool.AIReserveHandleType.TurnToDefault);
                    }
                    break;
                case LamiereGameManager.PlayerAutoMode.AutoMode:
                    LamiereGameManager.GetInstanceUnSafe.ReserveAICommand(soloCommandType, ThinkableTool.AIReserveHandleType.TurnToDefault);
                    break;
            }
        }
        public void StartCoolDown()
        {
            coolDownTime = Time.deltaTime;
        }
        public void ResetCoolDown()
        {
            coolDownTime = 0;
            _coolTimeMask.fillAmount = 0;
            _animation.Pause();
        }

        public void AddButtonAction(UnityAction action)
        {
            _button.onClick.AddListener(action);
        }
    }*/
}
#endif