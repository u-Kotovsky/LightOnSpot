using System;
using System.Collections.Generic;
using System.Text;

namespace LightOnSpotConsole
{
    public class LightingDrone
    {
        private (float, float) xLimit = (-800, 800);
        private byte xCoarse;
        public byte XCoarse { get { return xCoarse; } set { xCoarse = value; } }

        private byte xFine;
        public byte XFine { get { return xFine; } set { xFine = value; } }

        public float X
        {
            get
            {
                return Utility.GetValueFromCoarseFine(xCoarse, xFine,
                    xLimit.Item1, xLimit.Item2);
            }
            set
            {
                xCoarse = Utility.GetCoarse(value, xLimit.Item1, xLimit.Item2);
                xFine = Utility.GetFine(value, xLimit.Item1, xLimit.Item2);
            }
        }

        private (float, float) yLimit = (-800, 800);
        private byte yCoarse;
        public byte YCoarse { get { return yCoarse; } set { yCoarse = value; } }

        private byte yFine;
        public byte YFine { get { return yFine; } set { yFine = value; } }

        public float Y
        {
            get
            {
                return Utility.GetValueFromCoarseFine(yCoarse, yFine,
                    yLimit.Item1, yLimit.Item2);
            }
            set
            {
                yCoarse = Utility.GetCoarse(value, yLimit.Item1, yLimit.Item2);
                yFine = Utility.GetFine(value, yLimit.Item1, yLimit.Item2);
            }
        }

        private (float, float) zLimit = (-800, 800);
        private byte zCoarse;
        public byte ZCoarse { get { return zCoarse; } set { zCoarse = value; } }

        private byte zFine;
        public byte ZFine { get { return zFine; } set { zFine = value; } }

        public float Z
        {
            get
            {
                return Utility.GetValueFromCoarseFine(zCoarse, zFine, 
                    zLimit.Item1, zLimit.Item2); 
            }
            set
            {
                zCoarse = Utility.GetCoarse(value, zLimit.Item1, zLimit.Item2);
                zFine = Utility.GetFine(value, zLimit.Item1, zLimit.Item2);
            }
        }


        private byte r = 255;
        public byte R { get { return r; } set { r = value; } }

        private byte g = 255;
        public byte G { get { return g; } set { g = value; } }

        private byte b = 255;
        public byte B { get { return b; } set { b = value; } }
    }
}
