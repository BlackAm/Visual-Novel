using System;
using UnityEngine;
using System.Collections;

namespace Almond.Editor
{
    public class PopObjectMoveScript : MonoBehaviour
    {

        bool positionCheck;
        private Transform drawlineTarget;
        private string headName;
        private int objectIndex;
        private int editorState;

        private Vector3 originPos;
        private Vector3 originAngle;
        private Vector3 originSize;
        private GameObject nameGameObject;
#if UNITY_EDITOR
        private LabelIcon labelIcon;
#endif
        public Action MouseDownEventHandler;
        
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

        public void SetObjectIndex(int index)
        {
            objectIndex = index;
            transform.name = headName + index;
        }
        public void SetEditorState(int state)
        {
            editorState = state;
        }
#if UNITY_EDITOR
        public void SetHeadName(string name,string prefabName, LabelIcon labelIcon = LabelIcon.Green)
        {
            headName = name;
            if(nameGameObject == null)
            {
                nameGameObject = new GameObject(prefabName);
                nameGameObject.transform.parent = transform;
                nameGameObject.transform.localPosition = Vector3.zero;
                this.labelIcon = labelIcon;
            }
        }
#endif
        public void SendKeyEvent(EventType key)
        {
            if (key == EventType.MouseDown)
            {
                SetPositionCheck(false);
                MouseDownEventHandler?.Invoke();
                MouseDownEventHandler = null;
            }
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            //GizmosUtils.DrawText(GUI.skin, "AAAA", transform.position, Color.red, 12, 5);
            if(nameGameObject != null)
                DrawGizmoNameLabel.SetIcon(nameGameObject, labelIcon);
        }
#endif
        public void SetDrawLine(Transform tf)
        {
            drawlineTarget = tf;
        }
        public void DrawLine()
        {
            if (drawlineTarget == null) return;

            Debug.DrawLine(transform.position, drawlineTarget.position, Color.red);
        }
        public int GetObjectIndex() { return objectIndex; }
        public int GetEditorState() { return editorState; }

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
#if UNITY_EDITOR
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
#endif
            }
        }
    }


}
