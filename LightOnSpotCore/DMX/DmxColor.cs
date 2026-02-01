using System.Drawing;

namespace LightOnSpotCore
{
    public class DmxColor
    {
        private DMXChannel r = (DMXChannel)255;
        public DMXChannel R { get { return r; } set { r = value; } }

        private DMXChannel g = (DMXChannel)255;
        public DMXChannel G { get { return g; } set { g = value; } }

        private DMXChannel b = (DMXChannel)255;
        public DMXChannel B { get { return b; } set { b = value; } }

        public DmxColor() { }

        public DmxColor(byte r, byte g, byte b)
        {
            this.r.Coarse = r;
            this.g.Coarse = g;
            this.b.Coarse = b;
        }

        public DmxColor(Color color)
        {
            r.Coarse = color.R;
            g.Coarse = color.G;
            b.Coarse = color.B;
        }

        public void Set(byte r, byte g, byte b)
        {
            this.r.Coarse = r;
            this.g.Coarse = g;
            this.b.Coarse = b;
        }

        public void Set(Color color)
        {
            this.r.Coarse = color.R;
            this.g.Coarse = color.G;
            this.b.Coarse = color.B;
        }

        public byte[] GetBytes()
        {
            return [r.Coarse, g.Coarse, b.Coarse];
        }

        /*public static byte[] GetBytes(this Color color)
        {
            return [color.R, color.G, color.B];
        }*/
    }
}
