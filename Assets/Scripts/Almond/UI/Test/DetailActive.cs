#if !SERVER_DRIVE
using k514;
using UnityEngine;
using UnityEngine.UI;
namespace UI2020.Test
{
    public class DetailActive : AbstractUI
    {
        public Text attackTypeTx, buffTx, coolTime, mana;
        public Image attackType, buff;
        public Sprite physics, magic;
        
        public override void Initialize()
        {
            attackTypeTx = GetComponent<Text>("Top/AttackType/Text");
            buffTx = GetComponent<Text>("Top/AttackType/Text2");
            coolTime = GetComponent<Text>("Top/CoolTime/Text");
            mana = GetComponent<Text>("Top/Mana/Text");
            attackType = GetComponent<Image>("Top/AttackType/Image");
            buff = GetComponent<Image>("Top/AttackType/Image2");
            
            physics = LoadAssetManager.GetInstanceUnSafe.LoadAsset<Sprite>(ResourceType.Image, ResourceLifeCycleType.WholeGame, "Icon_PhysicsDmg.png").Item2;
            magic = LoadAssetManager.GetInstanceUnSafe.LoadAsset<Sprite>(ResourceType.Image, ResourceLifeCycleType.WholeGame, "Icon_MagicDmg.png").Item2;
            isBuffSkill(false);
        }
        
        /// 스킬 타입 지정하기.
        public void SettingType(Vocation type)
        {
            switch (type)
            {
                case Vocation.KNIGHT:
                case Vocation.ARCHER:
                    attackType.sprite = physics;
                    attackTypeTx.text = "물리";
                    break;
                case Vocation.MAGICIAN:
                    attackType.sprite = magic;
                    attackTypeTx.text = "마법";
                    break;
            }
        }

        /// 이 액티브 스킬이 버프인지?
        public void isBuffSkill(bool answer)
        {
            if (answer)
            {
                attackType.color = new Color32(255, 255, 255, 100);
                attackTypeTx.color = new Color32(255, 255, 255, 100);
                buff.color = new Color32(255, 255, 255, 255);
                buffTx.color = new Color32(255, 255, 255, 255);
            }
            else
            {
                attackType.color = new Color32(255, 255, 255, 255);
                attackTypeTx.color = new Color32(255, 255, 255, 255);
                buff.color = new Color32(255, 255, 255, 100);
                buffTx.color = new Color32(255, 255, 255, 100);
            }
        }
    }
}
#endif