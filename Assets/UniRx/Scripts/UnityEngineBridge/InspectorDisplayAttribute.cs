using System.Linq;

using UnityEngine;

namespace UniRx
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class InspectorDisplayAttribute : PropertyAttribute
    {
        public string FieldName { get; private set; }
        public bool NotifyPropertyChanged { get; private set; }

        public InspectorDisplayAttribute(string fieldName = "value", bool notifyPropertyChanged = true)
        {
            this.FieldName = fieldName;
            this.NotifyPropertyChanged = notifyPropertyChanged;
        }
    }
}