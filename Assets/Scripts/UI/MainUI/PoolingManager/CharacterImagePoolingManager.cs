using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public class CharacterImagePoolingManager : AbstractPrefabPoolingManager<CharacterImage>
    {
        private int debug=0;
        public override UnityPrefabObjectPool<CharacterImage> Initialize()
        {
            base.Initialize();
            prefabName = "CharacterImagePanel.prefab";
            return this;
        }

        protected override void OnCreate(CharacterImage obj)
        {
            obj.Initialize();
            obj.name = (debug++).ToString();
        }

        protected override void OnActive(CharacterImage obj)
        {
            obj.SetActive(true);
            obj.OnPooling();
        }

        protected override void OnPooled(CharacterImage obj)
        {
            obj.SetActive(false);
        }
    }

}
