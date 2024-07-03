using System.Collections;
using System.Collections.Generic;
using BDG;
using UnityEngine;

namespace UI2020
{
    public class SelectDialoguePoolingManager : AbstractPrefabPoolingManager<SelectDialogueButton>
    {
        private int debug=0;
        public override UnityPrefabObjectPool<SelectDialogueButton> Initialize()
        {
            base.Initialize();
            prefabName = "SelectDialogueObject.prefab";
            return this;
        }

        protected override void OnCreate(SelectDialogueButton obj)
        {
            obj.Initialize();
            obj.name = (debug++).ToString();
        }

        protected override void OnActive(SelectDialogueButton obj)
        {
            obj.SetActive(true);
        }

        protected override void OnPooled(SelectDialogueButton obj)
        {
            obj.SetActive(false);
        }
    }
}