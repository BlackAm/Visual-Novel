#if !SERVER_DRIVE && UNITY_EDITOR && ON_GUI

using System;
using System.Collections.Generic;
using BDG;
using UnityEngine.Rendering;
using UI2020;
using UnityEngine;

namespace k514
{
    public partial class TestManager
    {
        #region <Fields>

        private Vocation ChangeVocation;
        private LamiereUnit Spawned;
        private int _TestMonster = 1117;

        private int testCount = 1;
        private uint keyCount = 0;
        //private int partCount;

        private VFXUnit testUIEffect;
        
        #endregion

        #region <Callbacks>

        void OnAwakeObjectControl()
        {
            var targetControlType = TestControlType.ObjectControlTest;
            
            SetExtraIntInput(targetControlType, _TestMonster);
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.Q, ObjectSpawn, "테스트 프리팹 생성");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.W, ObjectKill, "테스트 프리팹 제거");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.E, ObjectAllyChange, "테스트 프리팹 동맹");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.R, ObjectMove, "테스트 프리팹 집합");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.T, PlayerMove, "테스트 프리팹 추적");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.A, PlayerChange, "플레이어 변경");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.D, UseQuickItem, "체력 마나 차감");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.F, CooldownClear, "쿨초");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.G, BackHome, "프리팹 리턴홈");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.H, SetTinyTinyFlag, "데미지 On/Off");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.V, RemoveBuffTest, "버프 해제");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.B, DisposeBuff, "버프 파기");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.N1, PlayerKill, "플레이어 사망");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.N2, PlayerFatal, "플레이어 체력 1%");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.N3, SpawnUIEffect, "UI 이펙트 출력테스트");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.N4, RetrieveUIEffect, "UI 이펙트 제거");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.N5, RetrieveAllUIEffect, "UI 이펙트 모두 제거");
            ChangeVocation = Vocation.KNIGHT;
        }

        #endregion

        #region <Methods>

        private async void ObjectSpawn(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            /*if (p_Type.IsInputRelease && PlayerManager.GetInstance.Player.IsValid())
            {
                //LamiereGameManager.GetInstanceUnSafe._ClientPlayer.UnEquipAllItem();
                
                var vocation = (uint)LamiereGameManager.GetInstanceUnSafe._ClientPlayer.Vocation * 1000;
                
                //LamiereGameManager.GetInstanceUnSafe._ClientPlayer.EquipItem(vocation + 71000 + keyCount, 0, UnitPartModelTool.LamiereUnitPart.Belt);
                
                
                var item = Inventory.GetInstance.MakeInputdata(71000 + keyCount);
                Inventory.GetInstance.AddItemInBag(item);
                Debug.LogError($"{item.uiTableKey}를 착용합니다...");
                
                LamiereGameManager.GetInstanceUnSafe._ClientPlayer.EquipItem(vocation + 70000 + keyCount, 0, UnitPartModelTool.LamiereUnitPart.Wristband);
                Inventory.GetInstance.EquipItem(item, (byte)item.ucEquipPart);
                
                MainGameUI.Instance.functionUI.inventory.Category.Selected();
                keyCount++;
                if (keyCount > 14) keyCount = 0;
            }*/
            
            
            if (p_Type.IsInputRelease && PlayerManager.GetInstance.Player.IsValid())
            {
                LamiereGameManager.GetInstanceUnSafe._ClientPlayer.UnEquipAllItem();
                
                var vocation = (uint)LamiereGameManager.GetInstanceUnSafe._ClientPlayer.Vocation * 10000;
                
                Debug.LogError($"=======================");
                
                LamiereGameManager.GetInstanceUnSafe._ClientPlayer.EquipItem(10000000 + vocation + keyCount * 10, 0, UnitPartModelTool.LamiereUnitPart.Weapon);
                if (vocation == 1000)
                {
                    LamiereGameManager.GetInstanceUnSafe._ClientPlayer.EquipItem(10000000 + 100000 + vocation + keyCount * 10, 0, UnitPartModelTool.LamiereUnitPart.SubWeapon);
                }
                LamiereGameManager.GetInstanceUnSafe._ClientPlayer.EquipItem(10000000 + 200000 + vocation + keyCount * 10, 0, UnitPartModelTool.LamiereUnitPart.Head);
                LamiereGameManager.GetInstanceUnSafe._ClientPlayer.EquipItem(10000000 + 300000 + vocation + keyCount * 10, 0, UnitPartModelTool.LamiereUnitPart.Body);
                LamiereGameManager.GetInstanceUnSafe._ClientPlayer.EquipItem(10000000 + 400000 + vocation + keyCount * 10, 0, UnitPartModelTool.LamiereUnitPart.Arm);
                LamiereGameManager.GetInstanceUnSafe._ClientPlayer.EquipItem(10000000 + 500000 + vocation + keyCount * 10, 0, UnitPartModelTool.LamiereUnitPart.Pants);
                LamiereGameManager.GetInstanceUnSafe._ClientPlayer.EquipItem(10000000 + 600000 + vocation + keyCount * 10, 0, UnitPartModelTool.LamiereUnitPart.Boots);

                keyCount++;
                if (keyCount > 14) keyCount = 0;
            }
        }

        private void ObjectKill(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (Spawned.IsValid() && p_Type.IsInputRelease)
            {
                Spawned.SetDead(false);
                Spawned = null;
            }
        }

        private void ObjectAllyChange(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (Spawned.IsValid() && p_Type.IsInputRelease)
            {
                Spawned.SetUnitGroupMask(UnitTool.UnitGroupType.None, UnitTool.UnitGroupType.Player);
            }
        }
        
        private void ObjectMove(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (Spawned.IsValid() && p_Type.IsInputRelease)
            {
                Spawned.OrderAIMoveTo(PlayerManager.GetInstance.Player, false);
            }
        }
        
        private void PlayerMove(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (Spawned.IsValid() && p_Type.IsInputRelease)
            {
                PlayerManager.GetInstance.Player.OrderAIMoveTo(Spawned._Transform.position - Vector3.up * 10f, false);
            }
        }
        
        private void PlayerChange(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            //PartyManager.GetInstanceUnSafe.partyExit();
            // momo6346: 캐릭터 변경 시 장착정보와 인벤토리 아이템 정보들이 초기화됩니다.
            MainGameUI.Instance.functionUI.top.OnClickCloseButton();
            foreach (var type in Enum.GetValues(typeof(Item.EquipType)))
            {
                LamiereGameManager.GetInstanceUnSafe._ClientPlayer.UnEquipItem(LamiereGameManager.GetInstanceUnSafe._ClientPlayer.GetPartModel(((Item.EquipType)type)));
            }

            MainGameUI.Instance.functionUI.inventory.AddInventorySlot(new List<ST_INVENTORY_ITEM>());
            Inventory.GetInstance.ResetItem();
            if (p_Type.IsInputRelease)
            {
                ChangeVocation = (Vocation)((int)ChangeVocation % 3) + 1;
                switch (ChangeVocation)
                {
                    case Vocation.KNIGHT:
                    case Vocation.ARCHER:
                    case Vocation.MAGICIAN:
                        LamiereGameManager.GetInstanceUnSafe.DeployPlayer(ChangeVocation);
                        break;
                }
                MainGameUI.Instance.mainUI.InitSkillList(ChangeVocation);
            }
        }
        
        private void UseQuickItem(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            var tryPlayer = LamiereGameManager.GetInstanceUnSafe._ClientPlayer;
            if (p_Type.IsInputRelease && tryPlayer.IsValid())
            {
                tryPlayer.AddValueRate(BattleStatusPreset.BattleStatusType.HP_Base,
                    new UnitPropertyChangePreset(null), -0.05f);
                tryPlayer.AddValueRate(BattleStatusPreset.BattleStatusType.MP_Base,
                    new UnitPropertyChangePreset(null), -0.05f);
                
                //LamiereGameManager.GetInstanceUnSafeUnSafe.ApplyQuickItem((LamiereGameManager.QuickSlotAutoItemType.QuickHpPortion, 0));
            }
        }
                
        private void CooldownClear(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            var tryPlayer = LamiereGameManager.GetInstanceUnSafe._ClientPlayer;
            if (p_Type.IsInputRelease && tryPlayer.IsValid())
            {
                tryPlayer._ActableObject.ResetCooldown();                
            }
        }
                        
        private void BackHome(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (Spawned.IsValid() && p_Type.IsInputRelease)
            {
                Spawned._MindObject.ReturnPosition(false, false);
            }
        }
        
        private void SetTinyTinyFlag(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            var tryPlayer = LamiereGameManager.GetInstanceUnSafe._ClientPlayer;
            if (p_Type.IsInputRelease && tryPlayer.IsValid())
            {
                CustomDebug.TinyTinySwing = !CustomDebug.TinyTinySwing;
            }
        }

        private void DisposeBuff(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue){
            if(p_Type.IsInputRelease){
                LamiereGameManager.GetInstanceUnSafe._ClientPlayer.BuffRelese();
            }
        }

        private void RemoveBuffTest(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue){
            if(p_Type.IsInputRelease){
                LamiereGameManager.GetInstanceUnSafe._ClientPlayer.OnTriggerAllBuffRemoved();
            }
        }
        
        private void PlayerKill(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if(p_Type.IsInputRelease)
            {
                LamiereGameManager.GetInstanceUnSafe._ClientPlayer?.SetDead(false);
            }
        }
        
        private void PlayerFatal(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if(p_Type.IsInputRelease)
            {
                var player = LamiereGameManager.GetInstanceUnSafe._ClientPlayer;
                if (ReferenceEquals(null, player))
                {
                }
                else
                {
                    player.AddValueRate(BattleStatusPreset.BattleStatusType.HP_Base, default, -0.99f);
                }
            }
        }

        private void SpawnUIEffect(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if(p_Type.IsInputRelease)
            {
                testUIEffect = MainGameUI.Instance._UIEffect.SpawnUIEffect(80000, MainGameUI.Instance.mainUI.AutoButtonPos()).Item2;
            }
        }

        private void RetrieveUIEffect(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if(p_Type.IsInputRelease)
            {
                MainGameUI.Instance._UIEffect.RetrieveUIEffect(testUIEffect);
                testUIEffect = default;
            }
        }

        private void RetrieveAllUIEffect(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if(p_Type.IsInputRelease)
            {
                MainGameUI.Instance._UIEffect.RetrieveAllUIEffect();
                testUIEffect = default;
            }
        }
        
        #endregion
    }
}

#endif