using System;

namespace UniRx
{
    /// <summary>
    /// <para>Inspectable ReactiveProperty.</para>
    /// <para>If you use with InspectorDisplayAttribute that beautified show on inspector.</para>
    /// </summary>
    [Serializable]
    public class IntReactiveProperty : ReactiveProperty<int>
    {
        public IntReactiveProperty()
            : base()
        {
            
        }

        public IntReactiveProperty(int initialValue)
            : base(initialValue)
        {

        }
    }

    /// <summary>
    /// <para>Inspectable ReactiveProperty.</para>
    /// <para>If you use with InspectorDisplayAttribute that beautified show on inspector.</para>
    /// </summary>
    [Serializable]
    public class LongReactiveProperty : ReactiveProperty<long>
    {
        public LongReactiveProperty()
            : base()
        {

        }

        public LongReactiveProperty(long initialValue)
            : base(initialValue)
        {

        }
    }


    /// <summary>
    /// <para>Inspectable ReactiveProperty.</para>
    /// <para>If you use with InspectorDisplayAttribute that beautified show on inspector.</para>
    /// </summary>
    [Serializable]
    public class ByteReactiveProperty : ReactiveProperty<byte>
    {
        public ByteReactiveProperty()
            : base()
        {

        }

        public ByteReactiveProperty(byte initialValue)
            : base(initialValue)
        {

        }
    }

    /// <summary>
    /// <para>Inspectable ReactiveProperty.</para>
    /// <para>If you use with InspectorDisplayAttribute that beautified show on inspector.</para>
    /// </summary>
    [Serializable]
    public class FloatReactiveProperty : ReactiveProperty<float>
    {
        public FloatReactiveProperty()
            : base()
        {

        }

        public FloatReactiveProperty(float initialValue)
            : base(initialValue)
        {

        }
    }

    /// <summary>
    /// <para>Inspectable ReactiveProperty.</para>
    /// <para>If you use with InspectorDisplayAttribute that beautified show on inspector.</para>
    /// </summary>
    [Serializable]
    public class DoubleReactiveProperty : ReactiveProperty<double>
    {
        public DoubleReactiveProperty()
            : base()
        {

        }

        public DoubleReactiveProperty(double initialValue)
            : base(initialValue)
        {

        }
    }

    /// <summary>
    /// <para>Inspectable ReactiveProperty.</para>
    /// <para>If you use with InspectorDisplayAttribute that beautified show on inspector.</para>
    /// </summary>
    [Serializable]
    public class StringReactiveProperty : ReactiveProperty<string>
    {
        public StringReactiveProperty()
            : base()
        {

        }

        public StringReactiveProperty(string initialValue)
            : base(initialValue)
        {

        }
    }
}