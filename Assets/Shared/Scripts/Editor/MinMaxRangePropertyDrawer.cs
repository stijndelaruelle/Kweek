using UnityEngine;
using UnityEditor;

namespace Kweek
{
    //http://www.grapefruitgames.com/blog/2013/11/a-min-max-range-for-unity/
    [CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
    public class MinMaxRangeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
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

            float minBoxX = (position.width * 0.445f);
            float maxBoxX = position.width - 37.0f;

            EditorGUI.LabelField(new Rect(position.x, position.y, position.width - minBoxX, position.height), label);

            newMin = Mathf.Clamp(EditorGUI.FloatField(new Rect(minBoxX, position.y, 52.0f, position.height), newMin), range.MinLimit, newMax);
            newMax = Mathf.Clamp(EditorGUI.FloatField(new Rect(maxBoxX, position.y, 52.0f, position.height), newMax), newMin, range.MaxLimit);

            EditorGUI.MinMaxSlider(new Rect(minBoxX + 57.0f, position.y, maxBoxX - (minBoxX + 65.0f), position.height), ref newMin, ref newMax, range.MinLimit, range.MaxLimit);

            minValue.floatValue = newMin;
            maxValue.floatValue = newMax;
        }
    }
}