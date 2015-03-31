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
    // If you want to cutomize other specialized ReactiveProperty
    // [UnityEditor.CustomPropertyDrawer(typeof(YouSpecializedReactiveProperty))]
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
                    if (propInfo != null)
                    {
                        var value = SerializedPropertyToObjectValue(targetSerializedProperty);
                        if (value != null)
                        {
                            var attachedComponent = property.serializedObject.targetObject;
                            var targetProp = fieldInfo.GetValue(attachedComponent);

                            // specialized for case of ReactiveProperty<Enum>
                            if (targetSerializedProperty.propertyType == SerializedPropertyType.Enum)
                            {
                                var baseType = fieldInfo.FieldType.BaseType;
                                if (baseType != null && baseType.IsGenericType)
                                {
                                    var generic = baseType.GetGenericArguments();
                                    if (generic != null && generic.Length == 1)
                                    {
                                        var typeInfo = generic[0];
                                        if (typeInfo.IsEnum)
                                        {
                                            var e = Enum.Parse(typeInfo, (string)value);
                                            propInfo.SetValue(targetProp, e, null);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                propInfo.SetValue(targetProp, value, null);
                            }
                        }
                    }
                }

                property.serializedObject.ApplyModifiedProperties();
            }
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

        protected virtual object SerializedPropertyToObjectValue(UnityEditor.SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return property.intValue;
                case SerializedPropertyType.Boolean:
                    return property.boolValue;
                case SerializedPropertyType.Float:
                    return property.floatValue;
                case SerializedPropertyType.String:
                    return property.stringValue;
                case SerializedPropertyType.Color:
                    return property.colorValue;
                case SerializedPropertyType.Enum:
                    return property.enumNames[property.enumValueIndex]; // return enumName
                case SerializedPropertyType.Vector2:
                    return property.vector2Value;
                case SerializedPropertyType.Vector3:
                    return property.vector3Value;
                case SerializedPropertyType.Vector4:
                    return property.vector4Value;
                case SerializedPropertyType.Rect:
                    return property.rectValue;
                case SerializedPropertyType.ArraySize:
                    return property.arraySize;
                case SerializedPropertyType.AnimationCurve:
                    return property.animationCurveValue;
                case SerializedPropertyType.Bounds:
                    return property.boundsValue;
                case SerializedPropertyType.Quaternion:
                    return property.quaternionValue;
                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.Gradient:
                case SerializedPropertyType.Character:
                case SerializedPropertyType.ObjectReference:
                case SerializedPropertyType.Generic:
                default:
                    return null; // I don't know but customize chance for inherited drawer
            }
        }
    }

#endif
}