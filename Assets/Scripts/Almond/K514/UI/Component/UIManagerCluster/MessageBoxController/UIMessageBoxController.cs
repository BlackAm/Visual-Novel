#if !SERVER_DRIVE
using System;
using System.Collections.Generic;
using UI2020;
using UnityEngine;
using Almond.RPC;

namespace k514
{
    /// 팝업 메세지.
    /// 참조 XML: UILanguage.xml(SetContentSet)
    public class UIMessageBoxController : UIManagerClusterBase
    {
        //public List<SimpleMessageBox> simpleBox = new List<SimpleMessageBox>(); 
        //public List<EitherOrMessageBox> eitherBox = new List<EitherOrMessageBox>();

        public MessageType CurrentPopup;

        private bool isViewPopUP;

        /// 메세지 유형
        public enum MessageType
        {
            connect,        // 접속오류.
            TutoSkip,       // 튜토리얼: 스킵
            PartyCreated,   // 파티 중복생성
            PartySetting,   // 파티설정 수정거부.
            PartyLevelEqual,
            PartyRoomDelete, // 파티 해산.
            AlreadyHaveParty,      // 파티방 입장 불가.
            JoinSamePartyRoom,       // 현재 속해 있는 파티방에 입장.
            PartyExitCheck,          // 파티 퇴장 묻기.
            Dead,
            CharcterDelete,
            Test,            // 테스트
            DiaShopPopUpPrint,
            DiaShopBuyError,
            DiaShopBuySuccess,
            MoneyAdd,
            ItemDelete,
            NotMyVocation,
            NotPartyMap,        // 파티생성이 허용되지 않는 맵.
            NotPVPMap,          // PVP가 허용되지 않는 맵.
            NotPVPLevel,
            NickNameError,      // 캐릭터 생성: 이름오류.
            NotJoinParty,        // 파티에 가입되지 않음.
            NotHaveParty,        // 초대하기 전에 파티생성 경고.
            HaveOtherParty,        // 다른 파티에 가입됨.
            SaveOption,
            NotSellBecauseEquip,    // 아이템을 판매할 수 없음.
            SkillLevelLow,          // 스킬 요구레벨에 충족되지 않을 때.
            DangerMap,          // 위험한 맵.
            ItemSlotFull,        // 아이템창 가득 참.
            BuyLock,             // 구매 잠금.
            Quit                // 게임 종료.
        }

        #region 공통
        /// <summary>
        /// 팝업메세지 출력.
        /// </summary>
        /// <param name="type">출력타입</param>
        /// <param name="p_One">버튼1 이벤트</param>
        /// <param name="p_Two">버튼2 이벤트</param>
        public void Pop(MessageType type, Action p_One = null, Action p_Two = null)
        {
            /*CurrentPopup = type;
            isViewPopUP = true;
            switch (type)
            {
                case MessageType.connect:
                    PopMessageBT1(200208, p_One);
                    break;
                case MessageType.TutoSkip:
                    break;
                case MessageType.PartyCreated:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT1(210000, p_One);
                    break;
                case MessageType.PartyLevelEqual:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT1(210003, p_One);
                    break;
                case MessageType.PartyRoomDelete:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT2(210021, p_One, p_Two);
                    break;
                case MessageType.AlreadyHaveParty:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT1(210022, p_One);
                    break;
                case MessageType.Dead:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT2(210001, p_One, p_Two);
                    break;
                case MessageType.CharcterDelete:
                    MenuUI.Instance.touchLock.SetActive(true);
                    PopMessageBT2(210002, p_One, p_Two);
                    break;
                case MessageType.ItemSlotFull:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT1(210003, p_One);
                    break;
                case MessageType.DiaShopPopUpPrint:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT1(210004, p_One);
                    break;
                case MessageType.DiaShopBuyError:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT1(210005, p_One);
                    break;
                case MessageType.DiaShopBuySuccess:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT1(210006, p_One);
                    break;
                case MessageType.MoneyAdd:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT1(210007, p_One);
                    break;
                case MessageType.ItemDelete:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT2(210008, p_One, p_Two);
                    break;
                case MessageType.NotMyVocation:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT1(210009, p_One);
                    break;
                case MessageType.NickNameError:
                    PopMessageBT1(210010, p_One);
                    break;
                case MessageType.NotPVPMap:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT1(210011, p_One);
                    break;
                case MessageType.NotPartyMap:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT1(210012, p_One);
                    break;
                case MessageType.NotJoinParty:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT1(210013, p_One);
                    break;
                case MessageType.NotHaveParty:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT1(210014, p_One);
                    break;
                case MessageType.HaveOtherParty:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT1(210015, p_One);
                    break;
                case MessageType.SaveOption:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT2(210016, p_One, p_Two);
                    break;
                case MessageType.NotSellBecauseEquip:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT1(210017, p_One);
                    break;
                case MessageType.SkillLevelLow:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT1(210018, p_One);
                    break;
                case MessageType.DangerMap:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT2(210019, p_One, p_Two);
                    break;
                case MessageType.NotPVPLevel:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT1(210020, p_One);
                    break;
                case MessageType.PartySetting:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT1(210023, p_One);
                    break;
                case MessageType.BuyLock:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT1(210024, p_One);
                    break;
                case MessageType.JoinSamePartyRoom:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT1(210025, p_One);
                    break;
                case MessageType.PartyExitCheck:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT2(210026, p_One, p_Two);
                    break;
                case MessageType.Quit:
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(true);
                    PopMessageBT2(210027, p_One, p_Two);
                    break;
                
                case MessageType.Test:
                default:
                    PopMessageBT1(200001, p_One);
                    break;
            }*/
        }
        #endregion

        #region 버튼1개

        public void PopMessageBT1(int p_Index, Action p_OnClicked)
        {
            Set_UI_Hide(false);
            var (isValid, spawned) =
                UISpawnManager.GetInstance.PopUI<SimpleMessageBox>(3, Vector3.zero, ResourceLifeCycleType.Scene);
            if (isValid)
            {
                spawned._Transform.SetParent(_Transform);
                spawned.SetContentSet(p_Index);
                spawned._RectTransform.SetPivotPreset(UITool.XAnchorType.Center, UITool.YAnchorType.Middle);
                spawned.SetHandler(p_OnClicked, CallBack);
                spawned.viewType = CurrentPopup;
                //  isViewPopUP = true;
                //  simpleBox.Add(spawned);
            }
        }
        public void PopMessageBT1(string p_Contents, Action p_OnClicked)
        {
            Set_UI_Hide(false);
            var (isValid, spawned) =
                UISpawnManager.GetInstance.PopUI<SimpleMessageBox>(3, Vector3.zero, ResourceLifeCycleType.Scene);
            if (isValid)
            {
                spawned._Transform.SetParent(_Transform);
                spawned.SetContents(p_Contents);
                spawned._RectTransform.SetPivotPreset(UITool.XAnchorType.Center, UITool.YAnchorType.Middle);
                spawned.SetHandler(p_OnClicked, CallBack);
                spawned.viewType = CurrentPopup;
                //  isViewPopUP = true;
                // simpleBox.Add(spawned);
            }
        }

        void CallBack()
        {
            isViewPopUP = false;
        }
        public void PopMessageBT1(string p_Title, string p_Content, string p_Button, Action p_OnClicked)
        {
            Set_UI_Hide(false);

            var (isValid, spawned) =
                UISpawnManager.GetInstance.PopUI<SimpleMessageBox>(3, Vector3.zero, ResourceLifeCycleType.Scene);

            if (isValid)
            {
                spawned._Transform.SetParent(_Transform);
                spawned.SetTitle(p_Title);
                spawned.SetContents(p_Content);
                spawned.SetButtonContents(p_Button);
                spawned._RectTransform.SetPivotPreset(UITool.XAnchorType.Center, UITool.YAnchorType.Middle);
                spawned.SetHandler(p_OnClicked, CallBack);
                spawned.viewType = CurrentPopup;
                //  isViewPopUP = true;
                // simpleBox.Add(spawned);
            }
        }

        #endregion

        #region 버튼2개

        public void PopMessageBT2(int p_Index, Action p_OnLeftClicked, Action p_OnRightClicked)
        {
            Set_UI_Hide(false);
            var (isValid, spawned) =
                UISpawnManager.GetInstance.PopUI<EitherOrMessageBox>(1, Vector3.zero, ResourceLifeCycleType.Scene);
            if (isValid)
            {
                spawned._Transform.SetParent(_Transform);
                spawned.SetContentSet(p_Index);
                spawned._RectTransform.SetPivotPreset(UITool.XAnchorType.Center, UITool.YAnchorType.Middle);
                spawned.SetHandler(p_OnLeftClicked, p_OnRightClicked, CallBack);
                spawned.viewType = CurrentPopup;
                // isViewPopUP = true;
                //eitherBox.Add(spawned);
            }
        }

        public void PopMessageBT2(string p_Title, string p_Content, string p_LeftButton, string p_RightButton, Action p_OnLeftClicked, Action p_OnRightClicked)
        {
            Set_UI_Hide(false);

            var (isValid, spawned) =
                UISpawnManager.GetInstance.PopUI<EitherOrMessageBox>(1, Vector3.zero, ResourceLifeCycleType.Scene);

            if (isValid)
            {
                spawned._Transform.SetParent(_Transform);
                spawned.SetTitle(p_Title);
                spawned.SetContents(p_Content);
                spawned.SetButtonContents(p_LeftButton, p_RightButton);
                spawned._RectTransform.SetPivotPreset(UITool.XAnchorType.Center, UITool.YAnchorType.Middle);
                spawned.SetHandler(p_OnLeftClicked, p_OnRightClicked, CallBack);
                spawned.viewType = CurrentPopup;

                //eitherBox.Add(spawned);
            }
        }


        #endregion

        /// 해당 팝업이 현재 활성화 중인지?
        public bool IsPopUpActive()
        {
            return isViewPopUP;
        }


        #region 테스트
        /// 테스트 팝업 출력.
        public void PopTestMessage()
        {
            Set_UI_Hide(false);

            var (isValid, spawned) =
                UISpawnManager.GetInstance.PopUI<EitherOrMessageBox>(1, Vector3.zero, ResourceLifeCycleType.Scene);

            if (isValid)
            {
                spawned._Transform.SetParent(_Transform);
                spawned.SetContentSet(200000);
                spawned._RectTransform.SetPivotPreset(UITool.XAnchorType.Center, UITool.YAnchorType.Middle);
            }
        }
        #endregion
    }
}
#endif