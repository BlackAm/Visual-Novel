using System.Linq;
using UnityEditor;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 유니티 입체 오브젝트의 기본 랜더러인 MeshRenderer 컴포넌트에 대해, 해당 컴포넌트가 공개하지 않는 sorting layer 등의
    /// 정보를 인스펙터 상에 공개하도록 도와주는 헬퍼 클래스
    /// ** 메쉬랜더러 중에서 머티리얼이 Zwrite off 인 셰이더를 가질 때만 적용되는데,
    /// 그러는 경우 3D 오브젝트의 은면까지 표시되므로 활용할 곳이 적다.
    /// </summary>
    [CustomEditor(typeof(MeshRenderer)), CanEditMultipleObjects]
    public class MeshRendererSortingOrderModifier : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawSortingLayerDropdownMenuBar();
            DrawSortingOrderIntField();
        }
        
        private void DrawSortingLayerDropdownMenuBar()
        {
            var helperTarget = target as MeshRenderer;
            var helperTargetSortingLayerID = helperTarget.sortingLayerID;
            
            EditorGUILayout.Space();
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            
            var sortingLayerGroup = SortingLayer.layers;
            var sortingLayerNameGroup = sortingLayerGroup.Select(l => l.name).ToArray();
            
            // 해당 랜더러가 유효하지 않은 sorting layer id를 가지는 경우 0번 레이어로 취급.
            if (!SortingLayer.IsValid(helperTargetSortingLayerID))
            {
                helperTargetSortingLayerID = sortingLayerGroup[0].id;
            }
            
            // 유니티 소팅 레이어의 id는 서수가 아니라 -11847274, 12790301 와 같은 난수이고 value가 0부터 시작하는 서수이다.
            var helperTargetSortingLayerValue = SortingLayer.GetLayerValueFromID(helperTargetSortingLayerID);
            // 팝업 GUI를 변경하는 경우 해당 변경사항이 타겟 랜더러의 sorting layer id에도 적용된다.
            var selectedSortingLayerValue = EditorGUILayout.Popup("Sorting Layer", helperTargetSortingLayerValue, sortingLayerNameGroup);
            if(EditorGUI.EndChangeCheck()) {
                helperTarget.sortingLayerID = sortingLayerGroup[selectedSortingLayerValue].id;
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSortingOrderIntField()
        {
            var helperTarget = target as MeshRenderer;
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            int order = EditorGUILayout.IntField("Sorting Order", helperTarget.sortingOrder);
            if(EditorGUI.EndChangeCheck()) {
                helperTarget.sortingOrder = order;
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}