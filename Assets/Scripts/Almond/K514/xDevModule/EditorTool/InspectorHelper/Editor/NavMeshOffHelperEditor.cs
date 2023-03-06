using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace k514
{
    [CustomEditor(typeof(OffMeshLink))]
    [CanEditMultipleObjects]
    public class NavMeshOffHelperEditor : Editor
    {
        #region <Callbacks>

        private void OnEnable()
        {
            var offMeshLink = target as OffMeshLink;
            var offMeshTransform = offMeshLink.transform;
            if (offMeshLink.startTransform == null)
            {
                var startPivot = new GameObject("StartPivot").transform;
                startPivot.SetParent(offMeshTransform, false);
                offMeshLink.startTransform = startPivot;
                Undo.RegisterCreatedObjectUndo(startPivot.gameObject, "Offmesh Link Build");
            }
            if (offMeshLink.endTransform == null)
            {
                var endPivot = new GameObject("EndPivot").transform;
                endPivot.SetParent(offMeshTransform, false);
                offMeshLink.endTransform = endPivot;
                Undo.RegisterCreatedObjectUndo(endPivot.gameObject, "Offmesh Link Build");
            }
        }

        private void OnSceneGUI()
        {
            Event currentEvent = Event.current;
            
            // 현재 에디터 모드이고, 마우스 버튼이 눌린 경우
            if (!Application.isPlaying)
            {
                // 현재 마우스 클릭의 이벤트 핸들러(혹은 UI 컨트롤) 아이디를 가져옴
                int id = GUIUtility.GetControlID(FocusType.Passive);
                // 마우스 클릭 이벤트 아이디를 통해, 현재 마우스 이벤트를 종료시킴
                HandleUtility.AddDefaultControl(id);
                
                switch (currentEvent.type)
                {
                    case EventType.MouseDown :
                        var offMeshLink = target as OffMeshLink;
                        var camera = Camera.current;
                        var mousePos = currentEvent.mousePosition;
                        var ppp = EditorGUIUtility.pixelsPerPoint;
                        
                        mousePos.y = camera.pixelHeight - mousePos.y * ppp;
                        mousePos.x *= ppp;
                        
                        if (Physics.Raycast(camera.ScreenPointToRay(mousePos), out var hit))
                        {
                            switch (currentEvent.button)
                            {
                                case 0 :
                                    Debug.Log("L");
                                    offMeshLink.startTransform.position = hit.point;
                                    EditorTool.OffMeshLinkCorrentHeight(offMeshLink);
                                    break;
                                case 1 :
                                    Debug.Log("R");
                                    offMeshLink.endTransform.position = hit.point;
                                    EditorTool.OffMeshLinkCorrentHeight(offMeshLink);
                                    break;
                            }
                        }
                        currentEvent.Use();
                        break;
                    case EventType.MouseUp :
                        currentEvent.Use();
                        break;
                }
            }

            DrawOfflinkPivot();
        }

        public override void OnInspectorGUI()
        {
            // 유니티 컴포넌트 레이아웃을 그린다.
            base.OnInspectorGUI();

            // 버튼 생성
            if (GUILayout.Button("Correct Heights"))
            {
                EditorTool.OffMeshLinkCorrentHeight(target as OffMeshLink);
            }

            DrawOfflinkPivot();
        }

        #endregion

        #region <Methods>

        private void DrawOfflinkPivot()
        {
            var offMeshLink = target as OffMeshLink;
            var startTransform = offMeshLink.startTransform;
            var endTransform = offMeshLink.endTransform;

            if (startTransform != null)
            {
                var startPivot = startTransform.position;
                CustomDebug.DrawCircle(startPivot, 0f, 2f, Vector3.up, Color.blue, 16);
            }

            if (endTransform != null)
            {
                var endPivot = endTransform.position;
                CustomDebug.DrawCircle(endPivot, 0f, 1f, Vector3.up, Color.blue, 16);
                CustomDebug.DrawCircle(endPivot, 0f, 2f, Vector3.up, Color.blue, 16);
                CustomDebug.DrawCircle(endPivot, 0f, 3f, Vector3.up, Color.blue, 16);
            }

            if (startTransform != null && endTransform != null)
            {
                var startPivot = startTransform.position;
                var endPivot = endTransform.position;
                CustomDebug.DrawArrow(startPivot, endPivot, 3f, Color.red);
            }
        }

        #endregion
    }
}