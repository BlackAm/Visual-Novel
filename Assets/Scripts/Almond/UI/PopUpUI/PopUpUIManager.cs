using System;
using System.Collections;
using k514;
using UnityEngine;
using UnityEngine.UI;

#if !SERVER_DRIVE
namespace UI2020
{
    /*public class PopUpUIManager : AbstractUI
    {
        private static PopUpUIManager _instance;
        public static PopUpUIManager Instance => _instance;

        public ItemSelectPopUp itemSelectPopUp;
        public GameObject touchLock, levelUp, lowBlood, pvpActive, itemSell;
        public bool isDead = false;

        public Text title, subTitle, pvpText, pvpSubText;
        public Image line;
        public Image bloodBG, pvpImage;
        float count = 0.05f;
        public bool isLowUIActive = false;
        public bool isPvp = false;
        private float pvpCount, pvpHideCount;

        private CanvasGroup pkCanvas;

        public Unit me, other;
        
        public void Init()
        {
            if (_instance == null) _instance = this;
            itemSelectPopUp = AddComponent<ItemSelectPopUp>("ItemSelectPopUp");
            itemSelectPopUp.Init();

            itemSelectPopUp.SetActive(false);

            touchLock = Find("TouchLock").gameObject;
            levelUp = Find("LevelUp").gameObject;
            touchLock.SetActive(false);

            pvpActive = Find("PvpActive").gameObject;
            pkCanvas = pvpActive.GetComponent<CanvasGroup>();
            pvpImage = GetComponent<Image>("PvpActive/BG");
            pvpText = GetComponent<Text>("PvpActive/Text");
            pvpSubText = GetComponent<Text>("PvpActive/SubText");

            title = GetComponent<Text>("LevelUp/Text");
            subTitle = GetComponent<Text>("LevelUp/Content");
            line = GetComponent<Image>("LevelUp/Image");
            lowBlood = Find("LowBlood").gameObject;
            bloodBG = lowBlood.GetComponentInChildren<Image>();

            // 아이템 판매 팝업
            itemSell = Find("ItemSellPopup").gameObject;
            GetComponent<Text>("ItemSellPopup/Text").text =
                "판매할 아이템에" + "<color=#E35200>높은 등급의 아이템</color>" +
                "도\n포함되어있습니다.\n" + "판매하시겠습니까?";
            GetComponent<Button>("ItemSellPopup/Bottom/Sell").onClick.AddListener(SellItem);
            GetComponent<Button>("ItemSellPopup/Bottom/Cancel").onClick.AddListener(ActiveItemSellPopup);

            SetActive(true);
        }
        
        /// 아이템 판매 팝업을 출력합니다.
        public void ActiveItemSellPopup()
        {
            itemSell.SetActive(!itemSell.activeSelf);
        }
        
        /// 아이템 판매.
        public void SellItem()
        {
#if UNITY_EDITOR
            Debug.LogError("아이템을 판매합니다.");
#endif
        }

        private void OnEnable()
        {
            if (_instance != null)
            {
                lowBlood.SetActive(false);
                MainGameUI.Instance.popUpUI.isPvp = false;
                MainGameUI.Instance.popUpUI.isLowUIActive = false;
            }
        }

        private void Update()
        {
            if (isLowUIActive)
            {
                lowBlood.SetActive(true);
                bloodBG.color = new Color(1, 1, 1, bloodBG.color.a + count);

                if (bloodBG.color.a > 1f)
                    count = -0.013f;
                else if (bloodBG.color.a < 0.15f)
                    count = 0.013f;
            }
            else
            {
                if (!isPvp) lowBlood.SetActive(false);
            }


            if (pvpCount >= 0)
            {
                pvpCount -= 0.01f;
            }
            else if (pvpCount <= 0)
            {
                if (pvpHideCount >= 0)
                {
                    pvpHideCount -= 0.01f;
                    pkCanvas.alpha = pvpHideCount;
                    
                } else if (pvpHideCount <= 0)
                {
                    pvpActive.SetActive(false);   
                }
            }
            /#1#/ pvp 활성화.
            if (isPvpActive)
            {
                pvpActive.SetActive(true);
                pvpImage.color = new Color(1, 1, 1, pvpImage.color.a + count);

                if (pvpImage.color.a > 1f)
                    count = -0.05f;
                else if (pvpImage.color.a < 0f)
                    count = 0.05f;
            }
            else
            {
                pvpActive.SetActive(false);
            }#1#
        }

        public void OnPlayerPKModeChanged(bool p_Flag)
        {
            isPvp = p_Flag;

            if (isPvp)
            {
                //lowBlood.SetActive(true);
                bloodBG.color = new Color(1, 1, 1, 1);
                pvpText.text = "<color=#ff0000>" + "PVP" + "</color>" + "가 활성화 되었습니다.";
            }
            else
            {
                //lowBlood.SetActive(false);
                bloodBG.color = new Color(1, 1, 1, bloodBG.color.a + 0);
                pvpText.text = "<color=#ff0000>" + "PVP" + "</color>" + "가 비활성화 되었습니다.";
            }

            Color color = new Vector4(1, 1, 1, 1);
            //pvpImage.color = pvpText.color = pvpSubText.color = color;
            pkCanvas.alpha = 1f;
            pvpActive.SetActive(true);
            pvpCount = 1f;
            pvpHideCount = 1f;

        }

        public void OpenDeadPopup()
        {
            DefaultUIManagerSet.GetInstanceUnSafe._UiMessageBoxController.Pop(UIMessageBoxController.MessageType.Dead, MoveHome, ClosePopUp);
            //deadPopUp.SetActive(true);
        }

        /// <summary>
        /// 사망 팝업 출력.
        /// </summary>
        public void ClosePopUp()
        {
            MainGameUI.Instance.popUpUI.touchLock.SetActive(false);
            
            isDead = true;
        }
        public void MoveHome()
        {
            MainGameUI.Instance.popUpUI.touchLock.SetActive(false);
            /*if(LamiereGameManager.GetInstance._AutoPlayMode == LamiereGameManager.PlayerAutoMode.AutoMode)
                LamiereGameManager.GetInstance.OnToggleAutoMode();#1#
            SceneControllerManager.GetInstance.TurnSceneTo(SceneControllerTool.SceneControllerShortCutType.MainHomeScene);
        }

        /// 레벨 업 팝업 출력.
        public void ViewLevelUp(int upLevel = 1)
        {
            if (!MainGameUI.Instance.isActiveAndEnabled) return;
            Color color = new Vector4(1, 1, 1, 1);
            title.color = subTitle.color = line.color = color;
            subTitle.text = $"{upLevel}레벨이 되었습니다.";
            // 버프 이펙트 출력.
            if (LamiereGameManager.GetInstanceUnSafe._ClientPlayer.IsValid())
            {
                UnitRenderingManager.GetInstance.CastUnitAttachedVfx(60000,
                    LamiereGameManager.GetInstanceUnSafe._ClientPlayer, Vector3.zero);
            }

            levelUp.SetActive(true);

            var _SpawnIntervalTimer = GameEventTimerHandlerManager.GetInstance.SpawnEventTimerHandler(SystemBoot.TimerType.GameTimer, false);
            _SpawnIntervalTimer
                .AddEvent(
                    2000,
                    handler =>
                    {
                        // 이 부분이 딜레이 후 실행됩니다.
                        handler.Arg1.StartCoroutine(FadeInLevelUp());
                        return true;
                    }, null, this);
            _SpawnIntervalTimer.StartEvent();
        }
        IEnumerator FadeInLevelUp()
        {
            float fadeCount = 1f;
            while (fadeCount >= 0f)
            {
                fadeCount -= 0.01f;
                title.color = new Color(1, 1, 1, fadeCount);
                subTitle.color = new Color(1, 1, 1, fadeCount);
                line.color = new Color(1, 1, 1, fadeCount);
                yield return new WaitForSeconds(0.01f);
            }
            levelUp.SetActive(false);
        }
        public void DisableLevelUPUI()
        {
            StopCoroutine(FadeInLevelUp());
            levelUp.SetActive(false);
        }
        public void ClearPopUpUI()
        {
            DisableLevelUPUI();
            lowBlood.SetActive(false);
        }
        /// 피없음 애니메이션 출력.(타이머 버전)
        public void SetBloodWarring()
        {
            if (isLowUIActive)
            {
                lowBlood.SetActive(true);
                bloodBG.color = new Color(1, 1, 1, bloodBG.color.a + count);

                if (bloodBG.color.a > 1f)
                    count = -0.05f;
                else if (bloodBG.color.a < 0f)
                    count = 0.05f;

                /*var _SpawnIntervalTimer = TerminateEventReceiverSpawnManager.GetInstanceUnSafe.SpawnEmptyTimer(false);
                _SpawnIntervalTimer
                    .AddEvent(
                        0, 50,
                        handler =>
                        {
                            bloodBG.color = new Color(1, 1, 1, bloodBG.color.a + count);

                            if (bloodBG.color.a > 1f)
                                count = -0.05f;
                            else if (bloodBG.color.a < 0f)
                                count = 0.05f;
                            return true;
                        });
                _SpawnIntervalTimer.StartEvent();#1#
            }
            else
            {
                lowBlood.SetActive(false);
            }
        }
    }*/
}
#endif