namespace LightOnSpotCore
{
    public abstract class ShortMath
    {
        public static (double, double) SinCos(double value) => (Math.Sin(value), Math.Cos(value));
        public static T GetT<T>(int index, params T[] values) => values[index] ?? values[0];
    }
}
