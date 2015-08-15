using System;
using UnityEngine;

namespace UniRx
{
    /// <summary>
    /// <para>Inspectable ReactiveProperty.</para>
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

        public IntReactiveProperty(IObservable<int> source, int initialValue)
          : base(source, initialValue)
        {

        }

        public IntReactiveProperty(IObservable<int> source)
          : base(source)
        {

        }
    }

    /// <summary>
    /// <para>Inspectable ReactiveProperty.</para>
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

        public LongReactiveProperty(IObservable<long> source, long initialValue)
          : base(source, initialValue)
        {

        }

        public LongReactiveProperty(IObservable<long> source)
          : base(source)
        {

        }
    }


    /// <summary>
    /// <para>Inspectable ReactiveProperty.</para>
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

        public ByteReactiveProperty(IObservable<byte> source, byte initialValue)
          : base(source, initialValue)
        {

        }

        public ByteReactiveProperty(IObservable<byte> source)
          : base(source)
        {

        }
    }

    /// <summary>
    /// <para>Inspectable ReactiveProperty.</para>
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

        public FloatReactiveProperty(IObservable<float> source, float initialValue)
          : base(source, initialValue)
        {

        }

        public FloatReactiveProperty(IObservable<float> source)
          : base(source)
        {

        }
    }

    /// <summary>
    /// <para>Inspectable ReactiveProperty.</para>
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

        public DoubleReactiveProperty(IObservable<double> source, double initialValue)
          : base(source, initialValue)
        {

        }

        public DoubleReactiveProperty(IObservable<double> source)
          : base(source)
        {

        }
    }

    /// <summary>
    /// <para>Inspectable ReactiveProperty.</para>
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

        public StringReactiveProperty(IObservable<string> source, string initialValue)
          : base(source, initialValue)
        {

        }

        public StringReactiveProperty(IObservable<string> source)
          : base(source)
        {

        }
    }

    /// <summary>
    /// <para>Inspectable ReactiveProperty.</para>
    /// </summary>
    [Serializable]
    public class BoolReactiveProperty : ReactiveProperty<bool>
    {
        public BoolReactiveProperty()
            : base()
        {

        }

        public BoolReactiveProperty(bool initialValue)
            : base(initialValue)
        {

        }

        public BoolReactiveProperty(IObservable<bool> source, bool initialValue)
          : base(source, initialValue)
        {

        }

        public BoolReactiveProperty(IObservable<bool> source)
          : base(source)
        {

        }
    }

    /// <summary>Inspectable ReactiveProperty.</summary>
    [Serializable]
    public class Vector2ReactiveProperty : ReactiveProperty<Vector2>
    {
        public Vector2ReactiveProperty()
        {

        }

        public Vector2ReactiveProperty(Vector2 initialValue)
            : base(initialValue)
        {

        }

        public Vector2ReactiveProperty(IObservable<Vector2> source, Vector2 initialValue)
          : base(source, initialValue)
        {

        }

        public Vector2ReactiveProperty(IObservable<Vector2> source)
          : base(source)
        {

        }
    }

    /// <summary>Inspectable ReactiveProperty.</summary>
    [Serializable]
    public class Vector3ReactiveProperty : ReactiveProperty<Vector3>
    {
        public Vector3ReactiveProperty()
        {

        }

        public Vector3ReactiveProperty(Vector3 initialValue)
            : base(initialValue)
        {

        }

        public Vector3ReactiveProperty(IObservable<Vector3> source, Vector3 initialValue)
          : base(source, initialValue)
        {

        }

        public Vector3ReactiveProperty(IObservable<Vector3> source)
          : base(source)
        {

        }
    }

    /// <summary>Inspectable ReactiveProperty.</summary>
    [Serializable]
    public class Vector4ReactiveProperty : ReactiveProperty<Vector4>
    {
        public Vector4ReactiveProperty()
        {

        }

        public Vector4ReactiveProperty(Vector4 initialValue)
            : base(initialValue)
        {

        }

        public Vector4ReactiveProperty(IObservable<Vector4> source, Vector4 initialValue)
          : base(source, initialValue)
        {

        }

        public Vector4ReactiveProperty(IObservable<Vector4> source)
          : base(source)
        {

        }
    }

    /// <summary>Inspectable ReactiveProperty.</summary>
    [Serializable]
    public class ColorReactiveProperty : ReactiveProperty<Color>
    {
        public ColorReactiveProperty()
        {

        }

        public ColorReactiveProperty(Color initialValue)
            : base(initialValue)
        {

        }

        public ColorReactiveProperty(IObservable<Color> source, Color initialValue)
          : base(source, initialValue)
        {

        }

        public ColorReactiveProperty(IObservable<Color> source)
          : base(source)
        {

        }
    }

    /// <summary>Inspectable ReactiveProperty.</summary>
    [Serializable]
    public class RectReactiveProperty : ReactiveProperty<Rect>
    {
        public RectReactiveProperty()
        {

        }

        public RectReactiveProperty(Rect initialValue)
            : base(initialValue)
        {

        }

        public RectReactiveProperty(IObservable<Rect> source, Rect initialValue)
          : base(source, initialValue)
        {

        }

        public RectReactiveProperty(IObservable<Rect> source)
          : base(source)
        {

        }
    }

    /// <summary>Inspectable ReactiveProperty.</summary>
    [Serializable]
    public class AnimationCurveReactiveProperty : ReactiveProperty<AnimationCurve>
    {
        public AnimationCurveReactiveProperty()
        {

        }

        public AnimationCurveReactiveProperty(AnimationCurve initialValue)
            : base(initialValue)
        {

        }

        public AnimationCurveReactiveProperty(IObservable<AnimationCurve> source, AnimationCurve initialValue)
          : base(source, initialValue)
        {

        }

        public AnimationCurveReactiveProperty(IObservable<AnimationCurve> source)
          : base(source)
        {

        }
    }

    /// <summary>Inspectable ReactiveProperty.</summary>
    [Serializable]
    public class BoundsReactiveProperty : ReactiveProperty<Bounds>
    {
        public BoundsReactiveProperty()
        {

        }

        public BoundsReactiveProperty(Bounds initialValue)
            : base(initialValue)
        {

        }

        public BoundsReactiveProperty(IObservable<Bounds> source, Bounds initialValue)
          : base(source, initialValue)
        {

        }

        public BoundsReactiveProperty(IObservable<Bounds> source)
          : base(source)
        {

        }
    }

    /// <summary>Inspectable ReactiveProperty.</summary>
    [Serializable]
    public class QuaternionReactiveProperty : ReactiveProperty<Quaternion>
    {
        public QuaternionReactiveProperty()
        {

        }

        public QuaternionReactiveProperty(Quaternion initialValue)
            : base(initialValue)
        {

        }

        public QuaternionReactiveProperty(IObservable<Quaternion> source, Quaternion initialValue)
          : base(source, initialValue)
        {

        }

        public QuaternionReactiveProperty(IObservable<Quaternion> source)
          : base(source)
        {

        }
    }
}