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
            var fieldValue = property.FindPropertyRelative(attr.FieldName);
            if (fieldValue == null)
            {
                UnityEditor.EditorGUI.LabelField(position, label, new GUIContent() { text = "InspectorDisplay can't find target:" + attr.FieldName });
            }
            else
            {
                UnityEditor.EditorGUI.PropertyField(position, fieldValue, label);
            }
        }
    }

#endif
}