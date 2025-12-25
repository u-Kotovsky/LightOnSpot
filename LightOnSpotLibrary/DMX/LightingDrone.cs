using System.Drawing;
using System.Numerics;
using Unity_DMX.Core;

namespace LightOnSpotCore
{
    public class LightingDrone
    {
        private static Vector3 Max = new Vector3(800, 800, 800);
        private static Vector3 Min = new Vector3(-800, -800, -800);

        private DmxVector3 position = new DmxVector3(Min, Max);
        public DmxVector3 Position { get { return position; } set { position = value; } }

        private DmxColor color = new DmxColor(255, 255, 255);
        public DmxColor Color { get { return color; } set { color = value; } }

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
            //if (x > Max.X) throw new Exception($"x > Max.X ({x} > {Max.X})");
            //if (x < Min.X) throw new Exception($"x < Min.X ({x} < {Min.X})");

            //if (y > Max.Y) throw new Exception($"x > Max.X ({y} > {Max.Y})");
            //if (y < Min.Y) throw new Exception($"x < Min.X ({y} < {Min.Y})");

            //if (z > Max.Z) throw new Exception($"x > Max.X ({z} > {Max.Z})");
            //if (z < Min.Z) throw new Exception($"x < Min.X ({z} < {Min.Z})");

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
