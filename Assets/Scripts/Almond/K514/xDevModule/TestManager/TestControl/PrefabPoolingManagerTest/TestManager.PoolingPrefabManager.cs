#if UNITY_EDITOR && ON_GUI
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public partial class TestManager
    {
        #region <Fields>

        private string prefabName;
        private PrefabPoolingTool.PrefabIdentifyKey _Key;
#if !SERVER_DRIVE
        List<FriendUserPreset> _FriendUserPreset = new List<FriendUserPreset>();
#endif
        
        #endregion

        #region <Callbacks>

        void OnAwakePrefabPooling()
        {
            var targetControlType = TestControlType.PoolingPrefabTest;
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.Q, PoolingPrefab, "인스턴스 생성");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.W, PoolingPrefab, "인스턴스 파기");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.E, PoolingPrefab, "인스턴스 로드");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.R, PoolingPrefab, "인스턴스 회수");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.T, NewPlayerUnit, "또다른 플레이어 생성");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.A, GetItem, "리더 아이템 획득");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.S, GetItem, "파티원 1 아이템 획득");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.D, GetItem, "파티원 2 아이템 획득");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.F, GetItem, "파티원 3 아이템 획득");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.G, GetItem, "파티원 4 아이템 획득");

            prefabName = "BounceBall.prefab";
        }

        #endregion

        #region <Methods>

        private void PoolingPrefab(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            var functionType = p_Type.CommandType;
            if (p_Type.IsInputRelease)
            {
                switch (functionType)
                {
                    case ControllerTool.CommandType.Q:
                        (_, _Key) = PrefabPoolingManager.GetInstance.PoolInstance(prefabName, ResourceLifeCycleType.WholeGame, ResourceType.GameObjectPrefab, _Transform.position);
                        break;
                    case ControllerTool.CommandType.W:
                        PrefabPoolingManager.GetInstance.ReleasePrefab(_Key);
                        break;
                    case ControllerTool.CommandType.E:
                        break;
                    case ControllerTool.CommandType.R:
                        PrefabPoolingManager.GetInstance.RetrievePrefab(_Key);
                        break;                
                }
            }

        }

        private async void NewPlayerUnit(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
#if !SERVER_DRIVE
            if (p_Type.IsInputRelease && PlayerManager.GetInstance.Player.IsValid())
            {
                var player = PlayerManager.GetInstance.Player;
                var spawnTo = player._Transform.position;

                if (player.IsValid())
                {
                    var (valid, spawned) = await UnitSpawnManager.GetInstance.SpawnUnitAsync<LamiereUnit>(UnityEngine.Random.Range(11, 14), spawnTo, ResourceLifeCycleType.Scene, ObjectDeployTool.ObjectDeploySurfaceDeployType.ForceSurface);

                    Unit unit = spawned;
                    if (valid)
                    {
                        unit.SetLevel(LamiereGameManager.GetInstanceUnSafe._ClientPlayer.GetCurrentLevel());
                        unit.SetUnitGroupMask(UnitTool.UnitGroupType.Player, UnitTool.UnitGroupType.Monster | UnitTool.UnitGroupType.Bot | UnitTool.UnitGroupType.Bot2);
                    }

                    string spawnName = unit.GetUnitName();
                    
                    foreach (var item in _FriendUserPreset)
                    {
                        if(item.GetName().Equals(unit.name)){
                            item.SetGuildName("1234");
                            item.SetUnit(spawned);
                            item.SetIsConnect(true);
                            return;
                        }
                    }

                    ST_ACCOUNT_CHARACTER_INFO info = FriendManager.GetInstance.AddST_ACCOUNT_CHARACTER_INFO((ushort)spawned.Vocation, (ushort)spawned.GetCurrentLevel(), (ulong)1210617164700, spawned.GetUnitName());
                    
                    FriendUserPreset __FriendUserPreset = new FriendUserPreset(info, true, spawned, "", 0);

                    FriendManager.GetInstance._RecommendFriendUserPreset.Add(__FriendUserPreset);
                    _FriendUserPreset.Add(__FriendUserPreset);
                    Debug.Log($"또다른 플레이어 생성 : [{unit.GetUnitName()}](lv {unit.GetCurrentLevel()} 생성:({valid}))");
                }
            }
#endif
        }

        private void GetItem(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue){
            /*
            if (p_Type.IsInputRelease && PlayerManager.GetInstanceUnSafe.Player.IsValid())
            {
                if (ReferenceEquals(PartyManager.GetInstanceUnSafe._CurrentJoinedPartyRoom.Value, default)) {
                    Debug.Log("파티에 가입되지 않았습니다.");
                    return;
                }
                var usParty = PartyManager.GetInstanceUnSafe._CurrentJoinedPartyRoom.Value;
                var functionType = p_Type.CommandType;
                switch (functionType)
                {
                    case ControllerTool.CommandType.A:
                        if(!ReferenceEquals(usParty._PartyLeader, default)){
                            Debug.Log($"{usParty._PartyLeader.GetUnitName()} : 아이템 드랍");
                            usParty.ItemDivision(usParty._PartyLeader);
                        }
                        else{
                            Debug.Log("파티리더를 찾을 수 없습니다.");
                        }
                        break;
                    case ControllerTool.CommandType.S:
                        if(usParty._PartyUser.Count > 0){
                            if(ReferenceEquals(usParty._PartyUser[0], default)){
                                Debug.Log("1번 파티원을 찾을 수 없습니다.");
                                return;
                            }
                            Debug.Log($"{usParty._PartyUser[0].GetUnitName()} : 아이템 드랍");
                            usParty.ItemDivision(usParty._PartyUser[0]);
                        }
                        else{
                            Debug.Log("1번 파티원이 존재하지 않습니다.");
                        }
                        break;
                    case ControllerTool.CommandType.D:
                        if(usParty._PartyUser.Count > 1){
                            if(ReferenceEquals(usParty._PartyUser[1], default)){
                                Debug.Log("2번 파티원을 찾을 수 없습니다.");
                                return;
                            }
                            Debug.Log($"{usParty._PartyUser[1].GetUnitName()} : 아이템 드랍");
                            usParty.ItemDivision(usParty._PartyUser[1]);
                        }
                        else{
                            Debug.Log("2번 파티원이 존재하지 않습니다.");
                        }
                        break;
                    case ControllerTool.CommandType.F:
                        if(usParty._PartyUser.Count > 2){
                            if(ReferenceEquals(usParty._PartyUser[2], default)){
                                Debug.Log("3번 파티원을 찾을 수 없습니다.");
                                return;
                            }
                            Debug.Log($"{usParty._PartyUser[2].GetUnitName()} : 아이템 드랍");
                            usParty.ItemDivision(usParty._PartyUser[2]);
                        }
                        else{
                            Debug.Log("3번 파티원이 존재하지 않습니다.");
                        }
                        break;
                    case ControllerTool.CommandType.G:
                        if(usParty._PartyUser.Count > 3){
                            if(ReferenceEquals(usParty._PartyUser[3], default)){
                                Debug.Log("4번 파티원을 찾을 수 없습니다.");
                                return;
                            }
                            Debug.Log($"{usParty._PartyUser[3].GetUnitName()} : 아이템 획득");
                            usParty.ItemDivision(usParty._PartyUser[3]);
                        }
                        else{
                            Debug.Log("4번 파티원이 존재하지 않습니다.");
                        }
                        break;                
                }
            }*/
        }

        #endregion
    }
}
#endif