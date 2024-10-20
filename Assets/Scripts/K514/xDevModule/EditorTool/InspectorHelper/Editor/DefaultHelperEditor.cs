using UnityEditor;
using UnityEngine;

namespace BlackAm
{
    [CustomEditor(typeof(DefaultHelper))]
    [CanEditMultipleObjects]
    public class DefaultHelperEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Test"))
            {
                var targetActor = target as DefaultHelper;
                targetActor.DoAct();
            }
        }
    }
}
