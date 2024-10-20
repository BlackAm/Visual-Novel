using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public class SaveLoadPoolingManager : AbstractPrefabPoolingManager<SaveLoadItem>
    {
        private int debug = 0;
        public override UnityPrefabObjectPool<SaveLoadItem> Initialize()
        {
            base.Initialize();
            prefabName = "SaveLoadItem.prefab";
            return this;
        }

        protected override void OnCreate(SaveLoadItem obj)
        {
            obj.name = (++debug).ToString();
            obj.Initialize();
        }

        protected override void OnActive(SaveLoadItem obj)
        {
            obj.SetActive(true);
        }

        protected override void OnPooled(SaveLoadItem obj)
        {
            obj.SetActive(false);
        }
    }   
}