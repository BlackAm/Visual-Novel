using System;
using System.Collections.Generic;

namespace BDG
{
    public class Item
    {
        public enum EquipType
        {
            None,
            
            // 무기
            Weapon = 1,
            SubWeapon,
            
            // 방어구
            Helmet,
            Armor,
            Gloves,
            Pants,
            Shoes,
            
            // 악세사리
            Ring = 10,
            Earring = 12,
            Necklace = 14,
            Belt,
            Wristband,
            
            // 펫
            Pet = 19
        }

        public enum FunctionalItemType
        {
            None,
            
            Reinforce, // 강화부
            ReinforceSupport, // 강화 지원부
            
            ProductionMaterial, // 제작 재료
            
            ChangeRandomOption, // 도장
            
            OpenJewelrySlot, // 황금 열쇠
        }
    }
}