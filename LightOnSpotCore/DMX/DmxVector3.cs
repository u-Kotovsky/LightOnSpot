using System.Numerics;

namespace LightOnSpotCore
{
    public class DmxVector3
    {
        private Vector3 Max = new Vector3(800, 800, 800);
        private Vector3 Min = new Vector3(-800, -800, -800);

        private DMXChannel x;
        public DMXChannel XChannel { get { return z; } set { z = value; } }
        public float X
        {
            get
            {
                return //Utility.MapRange(x, 0, 1, Min.X, Max.X); // 0 .. 1 to -800 .. 800
                float.Lerp(Min.X, Max.X, x.Value);
            }
            set
            {
                x.Value = //Utility.MapRange(x, Min.X, Max.X, 0, 1); // -800 .. 800 to 0 .. 1
                Utility.InverseLerp(Min.X, Max.X, value);
            }
        }

        private DMXChannel y;
        public DMXChannel YChannel { get { return z; } set { z = value; } }
        public float Y
        {
            get
            {
                return float.Lerp(Min.Y, Max.Y, y.Value);
            }
            set
            {
                y.Value = Utility.InverseLerp(Max.Y, Min.Y, value);
            }
        }

        private DMXChannel z;
        public DMXChannel ZChannel { get { return z; } set { z = value; } }
        public float Z
        {
            get
            {
                return float.Lerp(Min.Z, Max.Z, z.Value);
            }
            set
            {
                z.Value = Utility.InverseLerp(Min.Z, Max.Z, value);
            }
        }

        public DmxVector3() { }
        public DmxVector3(Vector3 min, Vector3 max) 
        {
            Min = min;
            Max = max;
        }

        public void Set(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
            //this.x.Value = x;
            //this.y.Value = y;
            //this.z.Value = z;
        }

        public void Set(Vector3 vector3)
        {
            X = vector3.X;
            Y = vector3.Y;
            Z = vector3.Z;
        }

        public override string ToString()
        {
            return $"DmxVector3: 'x: {x}, y: {y}, z: {z}'";
        }

        public byte[] GetBytes()
        {
            return [
                this.x.Coarse, this.x.Fine,
                this.y.Coarse, this.y.Fine,
                this.z.Coarse, this.z.Fine];
        }
    }
}
