// no use

#if false

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace UniRx.InternalUtil
{
    public interface IReflectionAccessor
    {
        object GetValue(object source);
    }

    public static class ReflectionAccessor
    {
        public static IReflectionAccessor Create(MemberInfo memberInfo)
        {
            var propInfo = memberInfo as PropertyInfo;
            if (propInfo != null)
            {
                return new PropertyInfoAccessor(propInfo);
            }

            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                return new FieldInfoAccessor(fieldInfo);
            }

            throw new ArgumentException("invalid member info:" + memberInfo.GetType());
        }

        public static IReflectionAccessor Create(MemberExpression rootExpression)
        {
            if (rootExpression == null) throw new ArgumentNullException("rootExpression");

            var accessor = new RecursiveAccessor(rootExpression);
            // minimum optimization
            return (accessor.AccessorCount == 1)
                ? accessor.GetFirstAccessor()
                : accessor;
        }

        class PropertyInfoAccessor : IReflectionAccessor
        {
            readonly MethodInfo methodInfo;

            public PropertyInfoAccessor(PropertyInfo propInfo)
            {
                methodInfo = propInfo.GetGetMethod();
            }

            public object GetValue(object source)
            {
                return methodInfo.Invoke(source, null);
            }
        }

        class FieldInfoAccessor : IReflectionAccessor
        {
            readonly FieldInfo fieldInfo;

            public FieldInfoAccessor(FieldInfo fieldInfo)
            {
                this.fieldInfo = fieldInfo;
            }

            public object GetValue(object source)
            {
                return fieldInfo.GetValue(source);
            }
        }

        class RecursiveAccessor : IReflectionAccessor
        {
            readonly List<IReflectionAccessor> accessors;

            public int AccessorCount { get { return accessors.Count; } }
            public IReflectionAccessor GetFirstAccessor()
            {
                return accessors[0];
            }

            public RecursiveAccessor(Expression expression)
            {
                var reflectionAccessors = new List<IReflectionAccessor>();
                while (expression is MemberExpression)
                {
                    var memberExpression = (MemberExpression)expression;
                    reflectionAccessors.Add(ReflectionAccessor.Create(memberExpression.Member));
                    expression = memberExpression.Expression;
                }

                this.accessors = reflectionAccessors;
            }

            public object GetValue(object source)
            {
                var result = source;
                for (int i = accessors.Count - 1; i >= 0; i--)
                {
                    var accessor = accessors[i];
                    result = accessor.GetValue(result);
                }
                return result;
            }
        }
    }
}

#endif