using System;
using Table;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(StackDisplayer))]
    public class StackDisplayerEditor : UnityEditor.Editor
    {
        private int stack = 1;
        
        public override void OnInspectorGUI()
        {
            if (!EditorApplication.isPlaying) return;
            
            DrawDefaultInspector();

            stack = Math.Max(1, EditorGUILayout.IntField("Stack", stack));
            
            if (GUILayout.Button("Update stack"))
            {
                ((StackDisplayer) target).UpdateStack(stack);
            }
        }
    }
}
