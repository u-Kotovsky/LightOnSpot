using System;
using System.Collections.Generic;
using System.Text;

namespace LightOnSpotConsole
{
    public static class Utility
    {

        /// <summary>
        /// Map a value with min/max ranges
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromMin"></param>
        /// <param name="fromMax"></param>
        /// <param name="toMin"></param>
        /// <param name="toMax"></param>
        /// <returns></returns>
        public static float MapRange(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            // Clamp input to source range
            value = Math.Clamp(value, fromMin, fromMax);

            // Map from source range to 0-1
            float normalized = (value - fromMin) / (fromMax - fromMin);

            // Map from 0-1 to target range
            return toMin + (normalized * (toMax - toMin));
        }

        /// <summary>
        /// Calculates coarse from value
        /// </summary>
        /// <param name="input"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static byte GetCoarse(float input, float minValue, float maxValue)
        {
            // 1) normalize
            double normalized = InverseLerp(minValue, maxValue, input);
            // 2) scale
            uint value = (uint)(normalized * ushort.MaxValue);
            // 3) get upper byte
            double coarse = value >> 8;
            // 4) return byte value
            return (byte)coarse;
        }

        /// <summary>
        /// Calculates fine from value
        /// </summary>
        /// <param name="input"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static byte GetFine(float input, float minValue, float maxValue)
        {
            // 1) normalize
            double normalized = InverseLerp(minValue, maxValue, input);
            // 2) scale
            uint value = (uint)(normalized * ushort.MaxValue);
            // 3) get upper byte
            double fine = value & 0xFF;
            // 4) return byte value
            return (byte)fine;
        }

        public static float InverseLerp(float from, float to, float value)
        {
            return (value - from) / (to - from);
        }

        /// <summary>
        /// Calculates coarse from value
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte GetCoarse(float input) // input = 0 .. 1
        {
            // 1) scale
            uint value = (uint)(input * ushort.MaxValue);
            // 2) get upper byte
            double coarse = value >> 8;
            // 3) return byte value
            return (byte)coarse;
        }

        /// <summary>
        /// Calculates fine from value
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte GetFine(float input) // input = 0 .. 1
        {
            // 1) scale
            uint value = (uint)(input * ushort.MaxValue);
            // 2) get upper byte
            double fine = value & 0xFF;
            // 3) return byte value
            return (byte)fine;
        }

        /// <summary>
        /// Calculates value from coarse, fine with value remap
        /// </summary>
        /// <param name="coarse"></param>
        /// <param name="fine"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static float GetValueFromCoarseFine(byte coarse, byte fine, float minValue, float maxValue)
        {
            float normalized = GetNormalizedValueFromCoarseFine(coarse, fine);
            return float.Lerp(minValue, maxValue, normalized);
        }

        /// <summary>
        /// Calculates normalized value from coarse, fine
        /// </summary>
        /// <param name="coarse"></param>
        /// <param name="fine"></param>
        /// <returns></returns>
        public static float GetNormalizedValueFromCoarseFine(byte coarse, byte fine)
        {
            uint combinedValue = ((uint)coarse << 8) | fine;
            return combinedValue / (float)ushort.MaxValue;
        }
    }
}
