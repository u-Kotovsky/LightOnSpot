using System.Drawing;
using System.Numerics;
using LightOnSpotCore.DMX.Types;
using Unity_DMX.Core;

namespace LightOnSpotCore.DMX.Fixtures
{
    public class LightingDrone
    {
        private static Vector3 Max = new(800, 800, 800);
        private static Vector3 Min = new(-800, -800, -800);

        private DmxVector3 position = new(Min, Max);
        public DmxVector3 Position { get { return position; } set { position = value; } }

        private DmxColor color = new(255, 255, 255);
        public DmxColor Color { get { return color; } set { color = value; } }

        public LightingDrone()
        {
            position.SetQuality(true);
        }

        public byte[] GetBytes()
        {
            byte[] positionBytes = position.GetBytes();
            byte[] colorBytes = color.GetBytes();
            byte[] bytes = new byte[9];

            bytes.BlockCopy(positionBytes, 0, 0, positionBytes.Length);
            bytes.BlockCopy(colorBytes, 0, positionBytes.Length, colorBytes.Length);

            return bytes;
        }

        public void SetPosition(float x, float y, float z)
        {
            position.Set(x, y, z);
        }

        public void SetPosition(Vector3 position)
        {
            SetPosition(position.X, position.Y, position.Z);
        }

        public void SetColor(byte r, byte g, byte b)
        {
            color.Set(r, g, b);
        }

        public void SetColor(Color color)
        {
            this.color.Set(color);
        }
    }
}
