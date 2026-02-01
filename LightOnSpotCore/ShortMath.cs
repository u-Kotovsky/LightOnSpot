namespace LightOnSpotCore
{
    public abstract class ShortMath
    {
        public static T GetT<T>(int index, params T[] values) => values[index] ?? values[0];
    }
}
