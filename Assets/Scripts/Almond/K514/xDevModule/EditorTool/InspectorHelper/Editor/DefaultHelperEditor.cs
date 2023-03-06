using UnityEditor;
using UnityEngine;

namespace k514
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
