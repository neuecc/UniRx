using UnityEngine;

namespace UniRx
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class InspectorDisplayAttribute : PropertyAttribute
    {
        public string FieldName { get; private set; }

        public InspectorDisplayAttribute(string fieldName = "value")
        {
            FieldName = fieldName;
        }
    }

#if UNITY_EDITOR

    [UnityEditor.CustomPropertyDrawer(typeof(InspectorDisplayAttribute))]
    public class InspectorDisplayDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            var attr = this.attribute as InspectorDisplayAttribute;
            var latestValue = property.FindPropertyRelative(attr.FieldName);
            UnityEditor.EditorGUI.PropertyField(position, latestValue, label);
        }
    }
#endif
}