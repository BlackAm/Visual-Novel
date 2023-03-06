#if UNITY_EDITOR

using System;
using UnityEngine;
using System.Collections;

namespace Almond.Editor.ServerEditor
{
    public class ServerPopObjectScript : MonoBehaviour
    {

        bool positionCheck;
        private Transform drawlineTarget;
        private string headName;

        private Vector3 originPos;
        private Vector3 originAngle;
        private Vector3 originSize;
        private GameObject nameGameObject;
        private LabelIcon labelIcon;
        public Action MouseDownEventHandler;
        private object tableData;
        private bool isSelect;

        public int monsterKey;
        public int level;
        public int regentime;//min
        public int maxRegentime;
        public string originalName;
        public float spawnRange;//범위 내에 무작위 스폰
        public TargetType numberType; //0 : 몬스터, 1 : Npc
        public void Init()
        {
            PositionSet();
            AngleSet();
            ScaleSet();
        }
        public void SetPositionCheck(bool state)
        {
            positionCheck = state;
        }
        public bool GetPositionCheck()
        {
            return positionCheck;
        }
        public bool SearchObjectCheck(bool state)
        {
            isSelect = state;
            return isSelect;
        }
        public void SetRandomRotation()
        {
            gameObject.transform.localEulerAngles = new Vector3(0, UnityEngine.Random.Range(0,36) * 10, 0);
        }
        public void SetTableData(object data)
        {
            tableData = data;
        }
        public void SetHeadName(string name,string prefabName, LabelIcon labelIcon = LabelIcon.Orange)
        {
            headName = name;
            gameObject.name = prefabName;
            originalName = name;
            if(nameGameObject == null)
            {
                nameGameObject = new GameObject(prefabName);
                nameGameObject.transform.parent = transform;
                nameGameObject.transform.localPosition = Vector3.zero;
                this.labelIcon = labelIcon;
            }
        }
        public void SetData()
        {

        }
        public void SendKeyEvent(EventType key)
        {
            if (key == EventType.MouseDown)
            {
                SetPositionCheck(false);
                MouseDownEventHandler?.Invoke();
                MouseDownEventHandler = null;
            }
        }
        
        private void OnDrawGizmos()
        {
            //GizmosUtils.DrawText(GUI.skin, "AAAA", transform.position, Color.red, 12, 5);
            if(nameGameObject != null)
            {
                nameGameObject.name = $"(Lv.{level}) {originalName}";
                DrawGizmoNameLabel.SetIcon(nameGameObject, isSelect ? LabelIcon.Green : labelIcon);

                Gizmos.color = isSelect ? Color.green : Color.red;
                Gizmos.DrawWireSphere(transform.position, spawnRange);
            }
        }

        public void SetDrawLine(Transform tf)
        {
            drawlineTarget = tf;
        }
        public void DrawLine()
        {
            if (drawlineTarget == null) return;

            Debug.DrawLine(transform.position, drawlineTarget.position, Color.red);
        }

        public void PositionSet() { originPos = transform.position; }
        public void PositionSet(Vector3 val) { transform.position = val; }
        public void PositionReset() { transform.position = originPos; }

        public void AngleSet() { originAngle = transform.eulerAngles; }
        public void AngleSet(Vector3 val) { transform.eulerAngles = val; }
        public void AngleReset() { transform.eulerAngles = originAngle; }

        public void ScaleSet() { originSize = transform.localScale; }
        public void ScaleSet(Vector3 val) { transform.localScale = val; }
        public void ScaleReset() { transform.localScale = originSize; }

        public static class GizmosUtils
        {
            public static void DrawText(GUISkin guiSkin, string text, Vector3 position, Color? color = null, int fontSize = 0, float yOffset = 0)
            {

                var prevSkin = GUI.skin;
                if (guiSkin == null)
                    Debug.LogWarning("editor warning: guiSkin parameter is null");
                else
                    GUI.skin = guiSkin;

                GUIContent textContent = new GUIContent(text);

                GUIStyle style = (guiSkin != null) ? new GUIStyle(guiSkin.GetStyle("Label")) : new GUIStyle();
                if (color != null)
                    style.normal.textColor = (Color)color;
                if (fontSize > 0)
                    style.fontSize = fontSize;

                Vector2 textSize = style.CalcSize(textContent);
                Vector3 screenPoint = Camera.current.WorldToScreenPoint(position);

                if (screenPoint.z > 0) // checks necessary to the text is not visible when the camera is pointed in the opposite direction relative to the object
                {
                    var worldPosition = Camera.current.ScreenToWorldPoint(new Vector3(screenPoint.x - textSize.x * 0.5f, screenPoint.y + textSize.y * 0.5f + yOffset, screenPoint.z));
                    UnityEditor.Handles.Label(worldPosition, textContent, style);
                }
                GUI.skin = prevSkin;
            }
        }
    }
}
#endif
