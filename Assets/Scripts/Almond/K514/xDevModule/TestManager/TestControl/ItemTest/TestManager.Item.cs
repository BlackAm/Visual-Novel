#if !SERVER_DRIVE && UNITY_EDITOR && ON_GUI

using UnityEngine;
using BDG;

namespace k514
{
    public partial class TestManager
    {
        /// <summary>
        /// 드랍할 장비의 키(EquipItemDataTable참조)
        /// </summary>
        readonly int[][] equipIndexCollection = new[] {
            new[] {1005, 2005, 3005},
            new[] {21005, 22005, 23005},
            new[] {31005, 32005, 33005},
            new[] {41005, 42005, 43005},
            new[] {51005, 52005, 53005},
            new[] {61005, 62005, 63005}
        };

        #region <Callbacks>

        private void OnAwakeItem()
        {
            var targetControlType = TestControlType.ItemTest;
            
            SetExtraIntInput(targetControlType, 0);
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.Q, EquipItem, "장착", "장비 장착");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.W, UnAllEquipItem, "모든 장비 해제", "모든 장비 해제");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.A, PlayerChange, "직업 변경", "직업 변경");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.E, SpawnDropItemEquip, "아이템드랍(무기)", "아이템드랍(무기)");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.R, SpawnDropItemEquip, "아이템드랍(건틀렛)", "아이템드랍(건틀렛)");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.T, SpawnDropItemEquip, "아이템드랍(갑옷)", "아이템드랍(갑옷)");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.S, SpawnDropItemEquip, "아이템드랍(장화)", "아이템드랍(장화)");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.D, SpawnDropItemEquip, "아이템드랍(투구)", "아이템드랍(투구)");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.F, SpawnDropItemEquip, "아이템드랍(하의)", "아이템드랍(하의)");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.Z, SpawnAddItem, "HP포션드랍", "HP포션드랍");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.X, SpawnAddItem, "MP포션드랍", "MP포션드랍");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.C, SpawnAddBuffItem, "버프포션드랍", "버프포션드랍");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.H, ReleseItemList, "드랍 아이템 객체 파기", "드랍 아이템 객체 파기");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.N1, SpawnOtherItem, "장비재료드랍", "장비재료드랍");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.N2, SpawnOtherItem, "보석드랍", "보석드랍");
        }

        #endregion

        #region <Methods>

        private void EquipItem(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                //var targetUnit = LamiereGameManager.GetInstanceUnSafeUnSafe._ClientPlayer;
                //targetUnit.EquipItem((uint)_CurrentExtraIntInput, false);
                
                var typeKey = int.Parse(_CurrentExtraIntInputString);

                if (UIItemData.GetInstanceUnSafe.GetTable().ContainsKey(typeKey))
                {
                    var item = Inventory.GetInstance.MakeInputdata((uint)typeKey);
                    
                    Inventory.GetInstance.AddItemInBag(item);
                    Inventory.GetInstance._inventoryUI.SetEquipItem(Inventory.GetInstance.GetEquipSlotString((uint)typeKey), item);
                    var part = LamiereGameManager.GetInstanceUnSafe._ClientPlayer.GetPartModel(UIItemData.GetInstanceUnSafe.GetTableData(typeKey).Type);
                    LamiereGameManager.GetInstanceUnSafe._ClientPlayer.EquipItem(item.uiTableKey, item.ullServerKey, part);
                }
            }
        }

        private void UnAllEquipItem(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                var targetUnit = LamiereGameManager.GetInstanceUnSafe._ClientPlayer;
                targetUnit.UnEquipAllItem();
            }
        }

        private void SpawnDropItemEquip(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease && PlayerManager.GetInstance.Player.IsValid())
            {
                var functionType = p_Type.CommandType;
                (int vocationNum, int equipTypeNum) equipIndex = (0,0);
                
                if (p_Type.IsInputRelease)
                {
                    var _vocation = LamiereGameManager.GetInstanceUnSafe._ClientPlayer.Vocation;
                    equipIndex.vocationNum = (int)_vocation - 1;
                    switch (functionType) {
                        case ControllerTool.CommandType.E:
                            equipIndex.equipTypeNum = 0;
                            break;
                        case ControllerTool.CommandType.R:
                            equipIndex.equipTypeNum = 1;
                            break;
                        case ControllerTool.CommandType.T:
                            equipIndex.equipTypeNum = 2;
                            break;
                        case ControllerTool.CommandType.S:
                            equipIndex.equipTypeNum = 3;
                            break;
                        case ControllerTool.CommandType.D:
                            equipIndex.equipTypeNum = 4;
                            break;
                        case ControllerTool.CommandType.F:
                            equipIndex.equipTypeNum = 5;
                            break;
                    }
                }
                //var player = PlayerManager.GetInstanceUnSafe.Player;
                //var spawnTo = player._Transform.position;
                
                uint _index = (uint)equipIndexCollection[equipIndex.equipTypeNum][equipIndex.vocationNum];
                Debug.Log($"장비 아이템 찾은 키[{equipIndex.equipTypeNum}][{equipIndex.vocationNum}] : {_index}");
                Inventory.GetDropItem(Inventory.GetInstance.MakeInputdata(_index, 0));
                //ItemManager.GetInstanceUnSafe.SpawnDropEquip(_index , spawnTo, LamiereGameManager.GetInstanceUnSafeUnSafe._ClientPlayer);

                UI2020.MainGameUI.Instance.functionUI.inventory.RefreshInventory();
            }
        }

        private void SpawnAddItem(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease && PlayerManager.GetInstance.Player.IsValid())
            {
                //var _amount = 1;
                var functionType = p_Type.CommandType;
                //int _index = 0;
                
                if (p_Type.IsInputRelease) {
                    switch (functionType) {
                        case ControllerTool.CommandType.Z:
                            Inventory.GetDropItem(Inventory.GetInstance.MakeInputdata(100, 0));
                            Inventory.GetDropItem(Inventory.GetInstance.MakeInputdata(101, 0));
                            Inventory.GetDropItem(Inventory.GetInstance.MakeInputdata(102, 0));
                            Inventory.GetDropItem(Inventory.GetInstance.MakeInputdata(103, 0));
                            Inventory.GetDropItem(Inventory.GetInstance.MakeInputdata(104, 0));
                            break;
                        case ControllerTool.CommandType.X:
                            Inventory.GetDropItem(Inventory.GetInstance.MakeInputdata(120, 0));
                            Inventory.GetDropItem(Inventory.GetInstance.MakeInputdata(121, 0));
                            Inventory.GetDropItem(Inventory.GetInstance.MakeInputdata(122, 0));
                            Inventory.GetDropItem(Inventory.GetInstance.MakeInputdata(123, 0));
                            Inventory.GetDropItem(Inventory.GetInstance.MakeInputdata(124, 0));
                            break;       
                        default:
                        return;
                    }
                }
                //var player = PlayerManager.GetInstanceUnSafe.Player;
                //var spawnTo = player._Transform.position;
                
                //ItemManager.GetInstanceUnSafe.SpawnAddItem(_index , spawnTo, LamiereGameManager.GetInstanceUnSafeUnSafe._ClientPlayer, _amount);
                UI2020.MainGameUI.Instance.functionUI.inventory.RefreshInventory();
            }
        }

        private void SpawnAddBuffItem(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue){
            if (p_Type.IsInputRelease && PlayerManager.GetInstance.Player.IsValid())
            {
                //var player = PlayerManager.GetInstanceUnSafe.Player;
                //var spawnTo = player._Transform.position;
                Inventory.GetDropItem(Inventory.GetInstance.MakeInputdata(500, 0));
                Inventory.GetDropItem(Inventory.GetInstance.MakeInputdata(501, 0));
                Inventory.GetDropItem(Inventory.GetInstance.MakeInputdata(502, 0));
                Inventory.GetDropItem(Inventory.GetInstance.MakeInputdata(503, 0));
                Inventory.GetDropItem(Inventory.GetInstance.MakeInputdata(504, 0));

                UI2020.MainGameUI.Instance.functionUI.inventory.RefreshInventory();
            }
        }

        private void SpawnOtherItem(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue){
            if (p_Type.IsInputRelease && PlayerManager.GetInstance.Player.IsValid())
            {
                var functionType = p_Type.CommandType;
                switch (functionType) {
                    case ControllerTool.CommandType.N1:
                        Inventory.GetInstance.AddItemInBag(Inventory.GetInstance.MakeInputdata(606, 29));
                        Inventory.GetInstance.AddItemInBag(Inventory.GetInstance.MakeInputdata(607, 19));
                        break;
                    case ControllerTool.CommandType.N2:
                        Inventory.GetInstance.AddItemInBag(Inventory.GetInstance.MakeInputdata(130001, 0));
                        Inventory.GetInstance.AddItemInBag(Inventory.GetInstance.MakeInputdata(130101, 0));
                        Inventory.GetInstance.AddItemInBag(Inventory.GetInstance.MakeInputdata(130201, 0));
                        Inventory.GetInstance.AddItemInBag(Inventory.GetInstance.MakeInputdata(130301, 0));
                        Inventory.GetInstance.AddItemInBag(Inventory.GetInstance.MakeInputdata(130010, 0));
                        Inventory.GetInstance.AddItemInBag(Inventory.GetInstance.MakeInputdata(130110, 0));
                        Inventory.GetInstance.AddItemInBag(Inventory.GetInstance.MakeInputdata(130210, 0));
                        Inventory.GetInstance.AddItemInBag(Inventory.GetInstance.MakeInputdata(130310, 0));
                        break;       
                    default:
                    return;
                }
                UI2020.MainGameUI.Instance.functionUI.inventory.RefreshInventory();
            }
        }


        private void ReleseItemList(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue) => ItemManager.GetInstanceUnSafe.ReleseItemPoolingList();

        #endregion
    }
}
#endif