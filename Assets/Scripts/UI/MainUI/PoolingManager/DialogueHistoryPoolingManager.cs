using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public class DialogueHistoryPoolingManager : AbstractPrefabPoolingManager<DialogueHistoryItem>
    {
        private int debug = 0;

        public override UnityPrefabObjectPool<DialogueHistoryItem> Initialize()
        {
            base.Initialize();
            prefabName = "DialogueHistory.prefab";
            return this;
        }

        protected override void OnCreate(DialogueHistoryItem obj)
        {
            obj.Initialize();
            obj.name = (debug++).ToString();
        }

        protected override void OnActive(DialogueHistoryItem obj)
        {
            obj.OnActive();
            obj.SetActive(true);
        }

        protected override void OnPooled(DialogueHistoryItem obj)
        {
            obj.SetActive(false);
        }
    }
}