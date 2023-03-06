#if !SERVER_DRIVE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BDG;
using k514;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

/*
 *  >> 인벤토리 관련
 *      포션은 FunctionUI의 OpenUI 함수에서 인벤토리를 열게 될 경우 생성됩니다.(튜토리얼 시에만)
 *      포션은 메인화면에 퀵슬롯으로 배치(BottomCenter_QuickSlot.cs)합니다.
 *      인벤토리 버튼 클릭관련: InventoryItemButton.cs
 * 
 *  >> 위치 관련
 *      인벤토리 아이템 위치는 아이템이 추가될 경우, InventoryUI의 AddInventoryItem함수에서
 *      슬롯마다 적용되는 InventoryItemButton 스크립트에 행과 열의 값을 저장합니다.
 *      해당 슬롯의 위치를 알고싶을 땐, row와 col을 가져오면 됩니다.
 *
 *  >> 퀵슬롯 관련
 *      InventoryUI.SelectedInventoryItem에서 선택된 슬롯의 행열정보를 가져올 수 있습니다.
 *
 * 
 */
namespace UI2020
{
    /// 튜토리얼 클래스. 외부에서 SetTargetFocus메소드로 암전효과를 주거나 끌 수 있습니다.
    /// 시작 맵은 'ch_new_zone_B'입니다.
    /*public class InfoUI : AbstractUI
    {
        public ulong infoCharacterKey;
        public GameObject hideArea;
        public Button hideButton;
        private RectTransform upHide, downHide;
        private Image npcIcon, control;
        public Text infoName, infoContent;
        private Button nextPage;
        public GameObject point;
        private GameObject point2, square1, square2;
        public GameObject square3;
        private GameObject square4;
        public GameObject info;
        private VerticalLayoutGroup sort;
        private GameObject finishInfo, finish;
        public GameObject subInfo;
        private Text subText;
        private Image subImage;
        public GameObject skipView;
        private Button skipYes, skipNo, skipButton;
        private Image rewardHP, rewardMP;
        public CanvasGroup infoCanvas;

        private Image CirclePoint;
        // 보조안내 달성했는지?
        private bool isSubDone = false;
        private Sprite lodingCircle, check, npcImage, joystick;
        public bool isPlayerDead = false;

        public bool isSceneNext = false;

        private PrefabInstance UIObject;

        // 스킬을 배치한 위치 저장.
        public int placeSkillPosition;
        
        // 암전 오브젝트
        private GameObject targetButton,
            targetClose,
            targetMenu,
            targetQuest,
            targetMiniQuest,
            targetHome,
            targetPotion,
            targetInven;
        // 암전되는 오브젝트
        private GameObject sortNormal, sortSkill01, sortSkill, sortInven, sortClose, sortMenu, sortQuest, sortMiniQuest;
        private GameObject sortSkill02, sortSkill03, sortSkill04;
        private RectTransform move,
            attack,
            map,
            topMenu,
            topQuest,
            status,
            close,
            closeTop,
            questIcon,
            questPanel,
            menu,
            menuQuest,
            skill,
            detailInfo,
            QuestNormal,
            home,
            QuestMission,
            QuestAdven;

        // 테스트용 제어
        private Button back, next;
        // 다른 스크립트에서 현재 암전상태를 알기위한 변수.
        public FocusTypeList focusState = FocusTypeList.Intro;
        // 시나리오 갯수.
        private int tableCount = 0;
        // 현재 시나리오에 따른 키값.
        public int sceneState = 1;
        // 컨트롤러 투명도 조절.
        private bool controller = false;
        // 포인터 투명도 조절.
        private bool pointer = false;
        private bool pointer2 = false;
        private bool squareBool = false;
        private bool squareBool2 = false;
        private bool squareBool3 = false;
        private bool squareBool4 = false;
        private float time = 2f;
        private int pointerCount = 1;
        private int pointerCount2 = 1;
        private bool isTutorial;

        // 씬 내역
        public enum FocusTypeList
        {
            Skip1, Skip2, Skip3,
            Intro,                  // 나레이션만
            Move1, Move2,           // 이동
            Attack1, Attack2, Attack3, // 공격/스킬
            Skill1, Skill2, Skill3, Skill4,
            Skill5, Skill6, Skill7, Skill8,
            Skill9,
            Map1, Map2, Map3,       // 미니맵
            Potion1, Potion2, Potion3, Potion4, // 포션
            Potion5, Potion6, Potion7, Potion8,
            Status1, Status2, Status3, Status4, // 캐릭터 스탯
            Quest1, Quest2, Quest3, Quest4,
            Quest5, Quest6, Quest7, Quest8,
            Quest9, Quest10, Quest11, Quest12,
            Quest13,
            DEAD
        }

        public void Initialize()
        {
            LoadUIObjectAsync("MainGameInformationUI.prefab",()=>
            {
                // 시나리오 갯수 알아오기.
                for (int i = 1; i < 100; i++)
                {
                    try
                    {
                        var temp = InformationData.GetInstanceUnSafe.GetTableData(i);
                    }
                    catch (Exception e)
                    {
                        tableCount = --i;
                        break;
                    }
                }

                tableCount = 47;
                isTutorial = false;

                hideArea = Find("MainGameInformationUI/HideArea").gameObject;
                hideButton = hideArea.GetComponent<Button>();
                hideButton.onClick.AddListener(NextPage);
                upHide = GetComponent<RectTransform>("MainGameInformationUI/HideArea/UpHide");
                downHide = GetComponent<RectTransform>("MainGameInformationUI/HideArea/DownHide");
                npcIcon = GetComponent<Image>("MainGameInformationUI/Info/NPC");
                infoName = GetComponent<Text>("MainGameInformationUI/Info/BG/Title/Text");
                infoContent = GetComponent<Text>("MainGameInformationUI/Info/BG/Text");
                nextPage = GetComponent<Button>("MainGameInformationUI/Info/BG");
                point = Find("MainGameInformationUI/Pointer").gameObject;
                point2 = Find("MainGameInformationUI/Pointer2").gameObject;
                control = GetComponent<Image>("MainGameInformationUI/Controller");



                square4 = Find("MainGameInformationUI/Pointer3").gameObject;
                info = Find("MainGameInformationUI/Info").gameObject;
                sort = GetComponent<VerticalLayoutGroup>("MainGameInformationUI/HideArea");
                nextPage.onClick.AddListener(NextPage);
                subInfo = Find("MainGameInformationUI/SubInfo").gameObject;
                subText = GetComponent<Text>("MainGameInformationUI/SubInfo/Image/Text");
                subImage = GetComponent<Image>("MainGameInformationUI/SubInfo/Image/Image");
                lodingCircle = LoadAssetManager.GetInstanceUnSafe.LoadAsset<Sprite>(ResourceType.Image, ResourceLifeCycleType.Scene, "PublicIcon_Loading2.png").Item2;
                check = LoadAssetManager.GetInstanceUnSafe.LoadAsset<Sprite>(ResourceType.Image, ResourceLifeCycleType.Scene, "Decoration_Check.png").Item2;


                skipView = Find("MainGameInformationUI/SkipView").gameObject;
                skipYes = GetComponent<Button>("MainGameInformationUI/SkipView/Bottom/Yes");
                skipNo = GetComponent<Button>("MainGameInformationUI/SkipView/Bottom/No");
                skipButton = GetComponent<Button>("MainGameInformationUI/SkipButton");
                infoCanvas = GetComponent<CanvasGroup>();


                rewardHP = GetComponent<Image>("MainGameInformationUI/Finish/Middle/Slot01/Slot/Image");
                rewardHP.sprite = LoadAssetManager.GetInstanceUnSafe.LoadAsset<Sprite>(ResourceType.Image, ResourceLifeCycleType.Scene, "Hp_Duration_Midium.png").Item2;
                rewardMP = GetComponent<Image>("MainGameInformationUI/Finish/Middle/Slot02/Slot/Image");
                rewardMP.sprite = LoadAssetManager.GetInstanceUnSafe.LoadAsset<Sprite>(ResourceType.Image, ResourceLifeCycleType.Scene, "Mp_Duration_Midium.png").Item2;

                // 테스트 제어
                back = GetComponent<Button>("MainGameInformationUI/TestControl/Back");
                next = GetComponent<Button>("MainGameInformationUI/TestControl/Next");

                SetActive(false);
            });
        }

        public override void OnActive()
        {
            CameraManager.GetInstanceUnSafe.MainCamera.gameObject.tag = "MainCamera";
            /*npcIcon.sprite = npcImage;
            control.sprite = joystick#1#
            npcIcon.sprite = LoadAssetManager.GetInstanceUnSafe.LoadAsset<Sprite>(ResourceType.Image, ResourceLifeCycleType.Scene, "NPC2.png").Item2;
        }


        /// 비활성화 될 때마다.
        public override void OnDisable()
        {
            Disable();
        }

        public override void OnSceneStarted()
        {
            
        }
        public void Disable()
        {
            info.SetActive(false);
            hideArea.SetActive(false);
            ResetUI();
            CameraManager.GetInstanceUnSafe.MainCamera.gameObject.tag = "Untagged";
            /*
            // 퀘스트 미니뷰가 숨김 상태인지?
            if (!MainGameUI.Instance.mainUI._isQuestSlotMinimized)
            {
                MainGameUI.Instance.mainUI.QuestSlotMinimize();
            }
            #1#
        }

        private void Update()
        {
            // 보조 안내가 실행중일 때만.
            if (!ReferenceEquals(_UIObject,null) && subInfo.activeSelf)
            {
                // 달성이 안 됐을 때만 회전.
                if (!isSubDone)
                {
                    subImage.transform.Rotate(Vector3.back, 100 * Time.deltaTime);
                }
            }
        }

        /// 키에 따른 암전,포인트,대화를 적용하는 메소드.
        /// 해당 튜토리얼로 이동합니다.
        /// 일반 시나리오와 보조 안내를 구분합니다.
        /// 예시> var avatarData = CreateCharacterData.GetInstanceUnSafe.GetTableData((int)classID);
        public void MoveSceneToKey(int key)
        {
            string script = "";
            // 튜토리얼 여부 판단.
            if (key == 1)
            {
                /#1#/ 튜토리얼을 한 적이 없었다면 4번째로.
                if (!MenuUI.Instance._charselect.isDoneTutorial)
                {
                    // 건너뛰기.
                    MoveSceneToKey(4);
                    return;
                }#1#
                isTutorial = true;
                MoveSceneToKey(4);
            }
            if (tableCount != 0)
            {
                if (key > 0 && key <= tableCount)
                {
                    var data = InformationData.GetInstanceUnSafe.GetTableData(key);
                    // xml에서 키에 따른 내용을 저장한다.
                    // 개행 체크.
                    if (data.Script.Contains("\\n"))
                    {
                        script = data.Script.Replace("\\n", "\n");
                    }
                    else
                    {
                        script = data.Script;
                    }

                    // 일반 시나리오와 보조 시나리오를 구분합니다.
                    if (data.Script.Contains("TIP."))
                    {
                        subText.text = script.Replace("TIP.", ""); ;
                    }
                    else
                    {
                        infoContent.text = script;
                    }
                    
                    sceneState = key;
                }
                else
                {
                    Debug.LogError("InfoUI: 잘못된 키를 입력했습니다. 요청한 키 값: " + key);
                }
            }
            else
            {
                Debug.LogError("InfoUI: 테이블이 비었습니다.");
            }
        }

        /// 다음 안내로 문자변경.
        public void NextPage()
        {
            if(skipView.activeSelf) return;
            
            if (focusState == FocusTypeList.Quest12)
            {
                return;
            }
            else if (focusState == FocusTypeList.DEAD)
            {
                info.SetActive(false);
                skipView.SetActive(true);
                return;
            }
        }

        /// 암전배경 너비,높이 초기화(해상도 사이즈에 맞게..)
        private void InitSizeBG()
        {
            upHide.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
            upHide.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);
            downHide.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
            downHide.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);
        }

        /// 보조안내 달성하기... 체크표시 출력.
        public void DoneSubInfo()
        {
            isSubDone = true;
            subImage.transform.localEulerAngles = new Vector3(0, 0, 0);
            subImage.sprite = check;
        }

        /// 튜토리얼 이후 메인으로 가기위해 저장하는 캐릭터 키.
        public void SetCharacterKey(ulong key)
        {
            infoCharacterKey = key;
        }

        /// 플레이어가 사망할 시.
        public void DeadInfo()
        {
            if (isPlayerDead)
            {
                SceneControllerManager.GetInstance.TurnSceneTo(1304, SceneControllerTool.LoadingSceneType.SolidImage);
                ResetUI();
            }
            else
            {
                isPlayerDead = true;
                // 안내하기.
                MoveSceneToKey(tableCount);
            }

        }

        /// UI 초기화입니다.
        public void ResetUI()
        {
            // 암전 UI 비활성화. 나중에 반복문으로 비활성해야 합니다.
            if(!ReferenceEquals(null, CirclePoint))
            {
                CirclePoint.gameObject.SetActive(false);
                targetButton.SetActive(false);
                targetMenu.SetActive(false);
                targetClose.SetActive(false);
                targetQuest.SetActive(false);
                targetMiniQuest.SetActive(false);
                targetHome.SetActive(false);
                // 암전되는 UI
                sortNormal.transform.SetAsFirstSibling();
                sortSkill01.transform.SetAsFirstSibling();
                sortSkill02.transform.SetAsFirstSibling();
                sortSkill03.transform.SetAsFirstSibling();
                sortSkill04.transform.SetAsFirstSibling();
                sortSkill.transform.SetAsFirstSibling();
                sortInven.transform.SetAsFirstSibling();
                sortClose.transform.SetAsFirstSibling();
                sortMenu.transform.SetAsFirstSibling();
                sortQuest.transform.SetAsFirstSibling();

                transform.parent.Find("MainUI/Overlay/MenuPanel/BG").SetAsFirstSibling();
                sortMiniQuest.transform.SetAsFirstSibling();
                topMenu.SetSiblingIndex(5);
                // 보조안내.
                isSubDone = false;
                subInfo.SetActive(false);
                subImage.transform.localEulerAngles = new Vector3(0, 0, 0);
                subImage.sprite = lodingCircle;
                point.transform.SetParent(Find("MainGameInformationUI").transform);

                square1.SetActive(false);
                square2.SetActive(false);
                square3.SetActive(false);
                square4.SetActive(false);

                CancelInvoke("SparklePoint");
                CancelInvoke("SparklePoint2");
                CancelInvoke("SparkleControl");
                CancelInvoke("SparkleSquare");
                CancelInvoke("SparkleSquare2");
                CancelInvoke("SparkleSquare3");
                CancelInvoke("SparkleSquare4");
            }
            
            

            hideArea.SetActive(true);
            info.SetActive(true);
            InitSizeBG();
            control.gameObject.SetActive(false);
            point.SetActive(false);
            point2.SetActive(false);
           
            nextPage.enabled = true;
        }

        public void StopInvoke(string name)
        {
            CancelInvoke(name);
        }
    }*/
}
#endif