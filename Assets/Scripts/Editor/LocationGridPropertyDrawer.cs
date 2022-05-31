using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(LocationGrid))]
public class LocationGridPropertyDrawer : PropertyDrawer {


    public override void OnGUI(Rect position,SerializedProperty property,GUIContent label){
        EditorGUI.PrefixLabel(position,label);
        Rect newposition = position;
        newposition.y += 18f;
        SerializedProperty data = property.FindPropertyRelative("rows");
        int width = property.FindPropertyRelative("width").intValue;
        int height = property.FindPropertyRelative("height").intValue;
        
        Debug.Log("Width: " + width);
        Debug.Log("Height: " + height);

        
        data.arraySize = height;
        for(int j=0;j<height;j++){
            SerializedProperty row = data.GetArrayElementAtIndex(j).FindPropertyRelative("row");
            newposition.height = 18f;
                row.arraySize = width;
            newposition.width = position.width/ width;
            for(int i=0;i<width;i++){
                EditorGUI.PropertyField(newposition,row.GetArrayElementAtIndex(i),GUIContent.none);
                newposition.x += newposition.width;
            }

            newposition.x = position.x;
            newposition.y += 18f;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property,GUIContent label){
        return 18f * 8;
    }
}