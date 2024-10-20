
#if !SERVER_DRIVE
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace BlackAm
{
    public abstract class AbstractUI : UIManagerBase
    {
        public bool IsActive => gameObject.activeSelf;

        public virtual void OnActive()
        {
            
        }

        public virtual void OnDisable()
        {
            
        }
        public virtual void Initialize() { }
        
        public virtual void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        
        protected T GetComponent<T>(string path)
        {
            return transform.Find(path).GetComponent<T>();
        }

        protected T AddComponent<T>(string path) where T : Component
        {
            return Find(path).gameObject.AddComponent<T>();
        }

        protected Transform Find(string path)
        {
            return transform.Find(path);
        }

        public void AddButtonEvent(UnityAction function)
        {
            AddButtonEvent(gameObject, function);
        }

        public void AddButtonEvent(GameObject target, UnityAction function)
        {
            if(target)
            {
                target.gameObject.GetComponent<Button>().onClick.AddListener(() => function());
            }
        }
        public void AddButtonEvent(string path, UnityAction function)
        {
            AddButtonEvent(transform.Find(path).gameObject, function);
        }

        public override void OnUpdateUI(float p_DeltaTime)
        {
        }

        protected override void DisposeUnManaged()
        {
        }
        
        public void SetScrollViewList(RectTransform content, GridLayoutGroup layoutGroup, int itemCount, RectTransform.Axis axis)
        {
            content.sizeDelta = new Vector2(axis == RectTransform.Axis.Horizontal ? (layoutGroup.cellSize.x + layoutGroup.spacing.x) * itemCount + layoutGroup.padding.left : content.sizeDelta.x,
                axis == RectTransform.Axis.Vertical ? (layoutGroup.cellSize.y + layoutGroup.spacing.y) * itemCount + layoutGroup.padding.top : content.sizeDelta.y);
        }
        public async void LoadUIObjectAsync(string prefabName, System.Action action, ResourceLifeCycleType lifeCycle = ResourceLifeCycleType.Free_Condition)
        {
            if (!ReferenceEquals(_UIObject, null)) return;
            var UI = await PrefabPoolingManager.GetInstance.PoolInstanceAsync(prefabName, lifeCycle, ResourceType.GameObjectPrefab);

            _UIObject = UI.Item1;

            var rt = _UIObject.GetComponent<RectTransform>();
            rt.SetParent(transform);
            rt.localPosition = Vector3.zero;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.localScale = Vector2.one;
            rt.name = prefabName.Replace(".prefab", "");
            action?.Invoke();
        }
        public void LoadUIObject(string prefabName, ResourceLifeCycleType lifeCycle = ResourceLifeCycleType.Free_Condition)
        {
            if (!ReferenceEquals(_UIObject, null)) return;
            _UIObject = PrefabPoolingManager.GetInstance.PoolInstance(prefabName, lifeCycle, ResourceType.GameObjectPrefab).Item1;
            var rt = _UIObject.GetComponent<RectTransform>();
            rt.SetParent(transform);
            rt.localPosition = Vector3.zero;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.localScale = Vector2.one;
            rt.name = prefabName.Replace(".prefab", "");
        }
        public virtual void UIObjectRelease()
        {
            if (!ReferenceEquals(_UIObject, null))
            {
                PrefabPoolingManager.GetInstance.ReleasePrefab(_UIObject.GetPrefabKey());
                _UIObject = null;
            }
        }
    }
}
#endif