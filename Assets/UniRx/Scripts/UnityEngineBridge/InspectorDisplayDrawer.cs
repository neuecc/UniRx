using System;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UniRx
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class InspectorDisplayAttribute : PropertyAttribute
    {
        public string FieldName { get; private set; }
        public bool NotifyPropertyChanged { get; private set; }

        public InspectorDisplayAttribute(string fieldName = "value", bool notifyPropertyChanged = true)
        {
            FieldName = fieldName;
            NotifyPropertyChanged = notifyPropertyChanged;
        }
    }

#if UNITY_EDITOR


    // InspectorDisplay and for Specialized ReactiveProperty
    // If you want to customize other specialized ReactiveProperty
    // [UnityEditor.CustomPropertyDrawer(typeof(YourSpecializedReactiveProperty))]
    // public class ExtendInspectorDisplayDrawer : InspectorDisplayDrawer { } 

    [UnityEditor.CustomPropertyDrawer(typeof(InspectorDisplayAttribute))]
    [UnityEditor.CustomPropertyDrawer(typeof(IntReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(LongReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(ByteReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(FloatReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(DoubleReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(StringReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(BoolReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(Vector2ReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(Vector3ReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(Vector4ReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(ColorReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(RectReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(AnimationCurveReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(BoundsReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(QuaternionReactiveProperty))]
    public class InspectorDisplayDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            string fieldName;
            bool notifyPropertyChanged;
            {
                var attr = this.attribute as InspectorDisplayAttribute;
                fieldName = (attr == null) ? "value" : attr.FieldName;
                notifyPropertyChanged = (attr == null) ? true : attr.NotifyPropertyChanged;
            }

            if (notifyPropertyChanged)
            {
                EditorGUI.BeginChangeCheck();
            }
            var targetSerializedProperty = property.FindPropertyRelative(fieldName);
            if (targetSerializedProperty == null)
            {
                UnityEditor.EditorGUI.LabelField(position, label, new GUIContent() { text = "InspectorDisplay can't find target:" + fieldName });
                if (notifyPropertyChanged)
                {
                    EditorGUI.EndChangeCheck();
                }
                return;
            }
            else
            {
                EmitPropertyField(position, targetSerializedProperty, label);
            }

            if (notifyPropertyChanged)
            {
                if (EditorGUI.EndChangeCheck())
                {
                    var propInfo = fieldInfo.FieldType.GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    property.serializedObject.ApplyModifiedProperties(); // deserialize to field

                    var paths = property.propertyPath.Split('.'); // X.Y.Z...
                    var attachedComponent = property.serializedObject.targetObject;

                    var targetProp = (paths.Length == 1)
                        ? fieldInfo.GetValue(attachedComponent)
                        : GetValueRecursive(attachedComponent, 0, paths);
                    var modifiedValue = propInfo.GetValue(targetProp, null); // retrieve new value

                    var methodInfo = fieldInfo.FieldType.GetMethod("SetValueAndForceNotify", BindingFlags.IgnoreCase | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (methodInfo != null)
                    {
                        methodInfo.Invoke(targetProp, new object[] { modifiedValue });
                    }
                }
                else
                {
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        object GetValueRecursive(object obj, int index, string[] paths)
        {
            var fieldInfo = obj.GetType().GetField(paths[index], BindingFlags.IgnoreCase | BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var v = fieldInfo.GetValue(obj);

            if (index < paths.Length - 1)
            {
                return GetValueRecursive(v, ++index, paths);
            }

            return v;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attr = this.attribute as InspectorDisplayAttribute;
            var fieldName = (attr == null) ? "value" : attr.FieldName;

            var height = base.GetPropertyHeight(property, label);
            var valueProperty = property.FindPropertyRelative(fieldName);
            if (valueProperty == null)
            {
                return height;
            }

            if (valueProperty.propertyType == SerializedPropertyType.Rect)
            {
                return height * 2;
            }
            if (valueProperty.propertyType == SerializedPropertyType.Bounds)
            {
                return height * 3;
            }

            if (valueProperty.isExpanded)
            {
                var count = 0;
                var e = valueProperty.GetEnumerator();
                while (e.MoveNext()) count++;
                return ((height + 4) * count) + 6; // (Line = 20 + Padding) ?
            }

            return height;
        }

        protected virtual void EmitPropertyField(Rect position, UnityEditor.SerializedProperty targetSerializedProperty, GUIContent label)
        {
            UnityEditor.EditorGUI.PropertyField(position, targetSerializedProperty, label, includeChildren: true);
        }
    }

#endif
}