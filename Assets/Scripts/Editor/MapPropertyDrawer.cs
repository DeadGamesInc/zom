using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(Map))]
public class MapPropertyDrawer : PropertyDrawer {

    public override void OnGUI(Rect position,SerializedProperty property,GUIContent label) {
        // EditorGUI.PrefixLabel(position,label);
        // locationGrid = property.FindPropertyRelative("data").d;
        Debug.Log("yo");
    }

    public override float GetPropertyHeight(SerializedProperty property,GUIContent label){
        return 18f * 8;
    }
}