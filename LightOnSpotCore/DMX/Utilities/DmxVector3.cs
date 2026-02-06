using System.Numerics;

namespace LightOnSpotCore.DMX.Utilities
{
    public class DmxVector3
    {
        private Vector3 Max = new(800, 800, 800);
        private Vector3 Min = new(-800, -800, -800);

        private DMXChannel x = new();
        public DMXChannel XChannel { get { return z; } set { z = value; } }
        public float X
        {
            get
            {
                return float.Lerp(Min.X, Max.X, x.Value);
            }
            set
            {
                x.Value = Utility.InverseLerp(Min.X, Max.X, value);
            }
        }

        private DMXChannel y = new();
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

        private DMXChannel z = new();
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

        public void SetQuality(bool useFine = false, bool useUtlra = false)
        {
            x.SetFine(fine => fine.Use = useFine);
            x.SetUltra(ultra => ultra.Use = useUtlra);

            y.SetFine(fine => fine.Use = useFine);
            y.SetUltra(ultra => ultra.Use = useUtlra);

            z.SetFine(fine => fine.Use = useFine);
            z.SetUltra(ultra => ultra.Use = useUtlra);
        }

        public void Set(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
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
                x.Coarse, x.Fine,
                y.Coarse, y.Fine,
                z.Coarse, z.Fine];
        }
    }
}
