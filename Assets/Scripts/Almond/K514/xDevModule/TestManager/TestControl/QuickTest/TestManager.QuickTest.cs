#if UNITY_EDITOR && ON_GUI
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI2020;

namespace k514
{
    public partial class TestManager
    {
        #region <Fields>

        
        #endregion

        #region <Callbacks>

        private void OnAwakeQuick()
        {
            var targetControlType = TestControlType.QuickTest;
        
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.Q, QuickTest1, "퀵 테스트 1");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.W, QuickTest2, "퀵 테스트 2");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.E, QuickTest3, "퀵 테스트 3");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.R, SoundTest, "친구 알림 테스트");
            //BindKeyTestEvent(targetControlType, ControllerTool.CommandType.R, PartyTest, "초대 승인테스트");
            //BindKeyTestEvent(targetControlType, ControllerTool.CommandType.T, PartyTest, "파티장 위임 테스트");
            //BindKeyTestEvent(targetControlType, ControllerTool.CommandType.A, PartyTest, "추방 테스트");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.S, FriendTest, "상대방 수락(체크)");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.D, FriendTest, "상대방 요청(체크)");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.F, FriendTest, "상대방 거절(체크)");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.G, FriendTest, "상대방 삭제(체크)");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.H, RaidJoinTest, "상대방 레이드 가입(체크)");
        }

        #endregion

        #region <Methods>

        private void QuickTest3(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
            }
        }
        /*
                private void PartyTest(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
                {

                    if (p_Type.IsInputRelease)
                    {
                        if(!PartyManager.GetInstanceUnSafe.PartyRooms.ContainsKey(200)){
                            PartyManager.GetInstanceUnSafe.PartyRooms.Add(200, new PartyRoom("테스트전용파티", SceneEnvironmentManager.GetInstanceUnSafe.GetSceneVariableData().SceneName, default, 0, 100, 5, UI2020.PartySetting.DivisionType.Get));
                        }

                        var functionType = p_Type.CommandType;
                        var clientRoom = PartyManager.GetInstanceUnSafe._CurrentJoinedPartyRoom.Value;
                        var player = LamiereGameManager.GetInstanceUnSafeUnSafe._ClientPlayer;
                        switch(functionType){
                            case ControllerTool.CommandType.R:
                                if (!ReferenceEquals(clientRoom, default))
                                {
                                    DefaultUIManagerSet.GetInstanceUnSafe._UiMessageBoxController.Pop(UIMessageBoxController.MessageType.PartyCreated);
                                    return;
                                }

                                if(PartyManager.GetInstanceUnSafe.PartyRooms[200].PartyJoinFlag(player)){
                                    UI2020.MainGameUI.Instance.mainUI.JoinPartyComplete( new KeyValuePair<int, PartyRoom>(200, PartyManager.GetInstanceUnSafe.PartyRooms[200]), player);
                                    return;
                                }
                                break;
                            case ControllerTool.CommandType.T:
                                if(!ReferenceEquals(PartyManager.GetInstanceUnSafe._CurrentJoinedPartyRoom.Value, default)) {
                                    if(!ReferenceEquals(clientRoom._PartyLeader, player)){
                                        clientRoom.UserMandate(player);
                                    }
                                }

                                break;
                            case ControllerTool.CommandType.A:
                                if(!ReferenceEquals(PartyManager.GetInstanceUnSafe._CurrentJoinedPartyRoom.Value, default)) {
                                    if(!ReferenceEquals(clientRoom._PartyLeader, player)){
                                        clientRoom.UserExile(player);
                                    }
                                }
                                break;
                        }

                    }
                }
        */
        private void SoundTest(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue){
            if (p_Type.IsInputRelease)
            {
                var functionType = p_Type.CommandType;
                switch(functionType){
                    case ControllerTool.CommandType.R:
                        Debug.Log("친구 알림 출력");
#if !SERVER_DRIVE
                        if(MainGameUI.Instance.functionUI.setting.Friend.btType.Equals(Setting.ButtonType.On))
                        {
                            MainGameUI.Instance.mainUI.OpenSoundMessage("친구 알림 출력 테스트");
                        }
#endif
                        break;
                    case ControllerTool.CommandType.T:

                        break;
                    case ControllerTool.CommandType.A:

                        break;
                }
                
            }
        }
        private void FriendTest(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                var functionType = p_Type.CommandType;
#if !SERVER_DRIVE
                switch(functionType){
                    case ControllerTool.CommandType.S:
                        if(!MainGameUI.Instance.functionUI.friend._MenuList.Equals(Friend.MenuList.Send)) return;
                        Debug.Log("상대방 수락(체크)");

                        foreach (var item in MainGameUI.Instance.functionUI.friend._FriendUserPresetItems)
                        {
                            if(item._Check.isOn){
                                FriendManager.GetInstance.OpponentRequestAccept(item.friendUserPreset);
                            }
                        }
                        break;
                    case ControllerTool.CommandType.D:
                        if(!MainGameUI.Instance.functionUI.friend._MenuList.Equals(Friend.MenuList.Recommend)) return;
                        Debug.Log("상대방 요청(체크)");

                        foreach (var item in MainGameUI.Instance.functionUI.friend._FriendUserPresetItems)
                        {
                            if(item._Check.isOn){
                                FriendManager.GetInstance.OpponentFriendRequest(item.friendUserPreset);
                            }
                        }
                        break;
                    case ControllerTool.CommandType.F:
                        if(MainGameUI.Instance.functionUI.friend._MenuList.Equals(Friend.MenuList.Receive)){
                            Debug.Log("상대방 거절(체크)");

                            foreach (var item in MainGameUI.Instance.functionUI.friend._FriendUserPresetItems)
                            {
                                if(item._Check.isOn){
                                    FriendManager.GetInstance.OpponentRequestDelete(item.friendUserPreset, Friend.MenuList.Receive);
                                }
                            }
                        }
                        else if(MainGameUI.Instance.functionUI.friend._MenuList.Equals(Friend.MenuList.Send)) {
                            Debug.Log("상대방 거절(체크)");

                            foreach (var item in MainGameUI.Instance.functionUI.friend._FriendUserPresetItems)
                            {
                                if(item._Check.isOn){
                                    FriendManager.GetInstance.OpponentRequestDelete(item.friendUserPreset, Friend.MenuList.Send);
                                }
                            }

                        }

                        break;
                    case ControllerTool.CommandType.G:
                        if(!MainGameUI.Instance.functionUI.friend._MenuList.Equals(Friend.MenuList.My)) return;
                        Debug.Log("상대방 삭제(체크)");

                        foreach (var item in MainGameUI.Instance.functionUI.friend._FriendUserPresetItems)
                        {
                            if(item._Check.isOn){
                                FriendManager.GetInstance.OpponentFriendDelete(item.friendUserPreset);
                            }
                        }
                        break;
                }
#endif
            }
        }
        
        private void RaidJoinTest(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue){
            if (p_Type.IsInputRelease)
            {
#if !SERVER_DRIVE
                RaidManager.GetInstance.OpponentJoinRoom(new RaidUser((Vocation)Random.Range(1,4), 50, "클론유저"));
#endif
            }
        }
        #endregion
    }
}
#endif