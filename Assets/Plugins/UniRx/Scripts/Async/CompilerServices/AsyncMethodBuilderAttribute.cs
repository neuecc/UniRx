#if CSHARP_7_OR_LATER

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace System.Runtime.CompilerServices
{
    public sealed class AsyncMethodBuilderAttribute : Attribute
    {
        public Type BuilderType { get; }

        public AsyncMethodBuilderAttribute(Type builderType)
        {
            BuilderType = builderType;
        }
    }
}

#endif