namespace LightOnSpotCore
{
    public struct DMXChannel
    {
        private byte coarse;
        public byte Coarse
        {
            get { return coarse; }
            set 
            {
                coarse = value;
                RefreshValue();
            }
        }

        private byte fine;
        public byte Fine
        {
            get { return fine; }
            set
            { 
                fine = value;
                RefreshValue();
            }
        }

        private byte ultra;
        public byte Ultra
        {
            get { return ultra; }
            set 
            { 
                ultra = value;
                RefreshValue();
            }
        }

        private float value; // value is 0 .. 1
        public float Value
        {
            get { return value; }
            set
            {
                this.value = value;
                Coarse = Utility.GetCoarse(value);
                Fine = Utility.GetFine(value);
                Ultra = Utility.GetUltra(value);
            }
        }

        private void RefreshValue()
        {
            value = Utility.GetValueFromCoarseFineUltra(coarse, fine, ultra, 0, 1);
        }

        public DMXChannel() { }

        public DMXChannel(float value) // 0 .. 1
        {
            Value = value;
        }

        public DMXChannel(byte coarse, byte fine = 0, byte ultra = 0) 
        {
            this.coarse = coarse;
            this.fine = fine;
            this.ultra = ultra;
            RefreshValue();
        }

        public static implicit operator byte(DMXChannel c) => c.coarse;
        public static implicit operator float(DMXChannel c) => c.value;
        public static implicit operator double(DMXChannel c) => c.value;

        public static explicit operator DMXChannel(byte c) => new DMXChannel(c, 0, 0);
        public static explicit operator DMXChannel(float c) => new DMXChannel(c);
        public static explicit operator DMXChannel(double c) => new DMXChannel((float)c);

        public override string ToString()
        {
            return $"{Math.Round(value, 2)}";
        }
    }
}