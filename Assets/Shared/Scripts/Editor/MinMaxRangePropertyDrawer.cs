using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

//http://www.grapefruitgames.com/blog/2013/11/a-min-max-range-for-unity/

[CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
public class MinMaxRangeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + 16;
    }

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Now draw the property as a Slider or an IntSlider based on whether it’s a float or integer.
        if (property.type != "MinMaxRange")
        {
            Debug.LogWarning("Use only with MinMaxRange type");
            return;
        }

        MinMaxRangeAttribute range = attribute as MinMaxRangeAttribute;
        SerializedProperty minValue = property.FindPropertyRelative("m_Min");
        SerializedProperty maxValue = property.FindPropertyRelative("m_Max");
        float newMin = minValue.floatValue;
        float newMax = maxValue.floatValue;

        float xDivision = position.width * 0.33f;
        float yDivision = position.height * 0.5f;
        EditorGUI.LabelField(new Rect(position.x, position.y, xDivision, yDivision), label);

        EditorGUI.LabelField(new Rect(position.x, position.y + yDivision, position.width, yDivision), range.MinLimit.ToString("0.##"));
        EditorGUI.LabelField(new Rect(position.x + position.width - 28f, position.y + yDivision, position.width, yDivision), range.MaxLimit.ToString("0.##"));
        EditorGUI.MinMaxSlider(new Rect(position.x + 24f, position.y + yDivision, position.width - 48f, yDivision), ref newMin, ref newMax, range.MinLimit, range.MaxLimit);

        EditorGUI.LabelField(new Rect(position.x + xDivision, position.y, xDivision, yDivision), "From: ");
        newMin = Mathf.Clamp(EditorGUI.FloatField(new Rect(position.x + xDivision + 30, position.y, xDivision - 30, yDivision), newMin), range.MinLimit, newMax);

        EditorGUI.LabelField(new Rect(position.x + xDivision * 2f, position.y, xDivision, yDivision), "To: ");
        newMax = Mathf.Clamp(EditorGUI.FloatField(new Rect(position.x + xDivision * 2f + 24, position.y, xDivision - 24, yDivision), newMax), newMin, range.MaxLimit);

        minValue.floatValue = newMin;
        maxValue.floatValue = newMax;
    }
}