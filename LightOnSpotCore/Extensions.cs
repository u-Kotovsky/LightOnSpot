using System.Drawing;
using System.Numerics;
using System.Text;
using NAudio.Midi;

namespace LightOnSpotCore
{
    public static class Extensions
    {
        public static int Map(this int value, int fromLow, int fromHigh, int toLow, int toHigh)
        {
            return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
        }

        public static float Map(this float value, float fromLow, float fromHigh, float toLow, float toHigh)
        {
            return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
        }

        /// <summary>
        /// Sets each axis to a random value 0 .. 1
        /// </summary>
        /// <param name="vec3"></param>
        /// <returns></returns>
        public static Vector3 RandomSingle(this Vector3 vec3)
        {
            vec3.X = Random.Shared.NextSingle();
            vec3.Y = Random.Shared.NextSingle();
            vec3.Z = Random.Shared.NextSingle();

            return vec3;
        }

        public static Color Lerp(this Color min, Color max, float amount)
        {
            amount = Math.Clamp(amount, 0, 1);
            int r = (int)float.Lerp(min.R, max.R, amount);
            int g = (int)float.Lerp(min.G, max.G, amount);
            int b = (int)float.Lerp(min.B, max.B, amount);
            int a = (int)float.Lerp(min.A, max.A, amount);
            Color color = Color.FromArgb(a, r, g, b);
            return color;
        }

        public static string GetReadableNumber(int width, double value)
        {
            long powerOfTen = 1;
            for (int i = 0; i < width; i++)
                powerOfTen *= 10;

            long paddedValue = (long)(value % powerOfTen) + powerOfTen;

            return paddedValue.ToString()[1..];

            //int factor = (int)(Math.Abs(min - number) / min) % (int)(min * 0.1);
            //int count = Math.Clamp(factor - 1, 0, 1000);
            //return string.Join(string.Empty, Enumerable.Repeat("0", factor)) + number;
        }

        public static string GetReadableTime(SmpteOffsetEvent @event)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(GetReadableNumber(2, @event.Hours));
            builder.Append(":");
            builder.Append(GetReadableNumber(2, @event.Minutes));
            builder.Append(":");
            builder.Append(GetReadableNumber(2, @event.Seconds));
            //builder.Append("\n");
            //builder.Append(GetReadableNumber(3, @event.Frames));
            //builder.Append(":");
            //builder.Append(GetReadableNumber(3, @event.SubFrames));

            return builder.ToString();
        }

        public static SmpteOffsetEvent GetSmpteOffsetEvent(TimeSpan timeSpan)
        {
            return GetSmpteOffsetEvent((byte)timeSpan.Hours, (byte)timeSpan.Minutes, (byte)timeSpan.Seconds,
                (byte)timeSpan.Milliseconds, (byte)timeSpan.Nanoseconds);
        }

        public static SmpteOffsetEvent GetSmpteOffsetEvent(byte hours, byte minutes, byte seconds, byte frames, byte subFrames)
        {
            return new SmpteOffsetEvent(hours, minutes, seconds, frames, subFrames);
        }
    }
}