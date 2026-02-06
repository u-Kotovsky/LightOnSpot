using System.Drawing;

namespace LightOnSpotCore.DMX.Types
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
            this.r.DefaultValue = r;
            this.g.DefaultValue = g;
            this.b.DefaultValue = b;

            //this.r.

            this.r.Reset();
            this.g.Reset();
            this.b.Reset();
        }

        public DmxColor(Color color)
        {
            r.Value = color.R;
            g.Value = color.G;
            b.Value = color.B;

            r.Reset();
            g.Reset();
            b.Reset();
        }

        public void Set(byte r, byte g, byte b)
        {
            this.r.Value = r;
            this.g.Value = g;
            this.b.Value = b;
        }

        public void Set(Color color)
        {
            this.r.Value = color.R;
            this.g.Value = color.G;
            this.b.Value = color.B;
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
