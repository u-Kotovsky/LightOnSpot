using LightOnSpotCore.DMX.Utilities;

namespace LightOnSpotCore.DMX.Types
{
    public class DMXChannel
    {
        private SubChannel coarse = new();
        public SubChannel Coarse { get { return coarse; } set { coarse = value; } }

        private SubChannel fine = new();
        public SubChannel Fine { get { return fine; } set { fine = value; } }

        private SubChannel ultra = new();
        public SubChannel Ultra { get { return ultra; } set { ultra = value; } }

        // Actual value
        private float value; // value is 0 .. 1
        public float Value
        {
            get { return value; }
            set
            {
                this.value = value;

                coarse.Value = Utility.GetCoarse(value);

                if (fine.Use)
                {
                    fine.Value = Utility.GetFine(value);

                    if (ultra.Use)
                    {
                        ultra.Value = Utility.GetUltra(value);
                    }
                }
            }
        }

        private float defaultValue;
        public float DefaultValue { get { return defaultValue; } set { defaultValue = value; }  }

        public void Reset()
        {
            Value = DefaultValue;
        }

        private void RefreshValue()
        {
            value = Utility.GetValueFromCoarseFineUltra(coarse, fine, ultra, 0, 1);
        }

        public DMXChannel() { }

        public DMXChannel(int offset, float value, bool useFine = false, bool useUltra = false) // 0 .. 1
        {
            Value = value;

            if (useUltra && !useFine) throw new Exception("You cannot use ultra while fine is not in use.");
        }

        public DMXChannel(int offset, byte coarse, byte fine = 0, byte ultra = 0, bool useFine = false, bool useUltra = false) 
        {
            if (useUltra && !useFine) throw new Exception("You cannot use ultra while fine is not in use.");

            this.coarse.Value = coarse;

            this.fine.Use = useFine;
            this.ultra.Use = useUltra;

            if (useFine)
            {
                this.fine.Value = fine;

                if (useUltra)
                {
                    this.ultra.Value = ultra;
                }
            }

            RefreshValue();
        }

        public static implicit operator byte(DMXChannel c) => c.coarse;
        public static implicit operator float(DMXChannel c) => (float)c.value;
        public static implicit operator double(DMXChannel c) => c.value;

        public static explicit operator DMXChannel(byte c) => new DMXChannel(c, 0, 0);
        public static explicit operator DMXChannel(float c) => new DMXChannel(0, c);
        public static explicit operator DMXChannel(double c) => new DMXChannel(0, (float)c);

        public override string ToString()
        {
            return $"{Math.Round(value, 2)}";
        }

        public Dictionary<int, byte> GetDmxValues()
        {
            var dict = new Dictionary<int, byte>();

            dict[coarse.Offset] = coarse.Value;

            if (fine.Use)
            {
                dict[fine.Offset] = fine.Value;

                if (ultra.Use)
                {
                    dict[ultra.Offset] = ultra.Value;
                }
            }

            return dict;
        }

        public void Set(float value)
        {
            Value = value;
        }

        public void SetCoarse(byte value)
        {
            Coarse.Value = value;
        }

        public void SetFine(byte value)
        {
            Fine.Value = value;
        }

        public void SetUltra(byte value)
        {
            Ultra.Value = value;
        }

        public void SetCoarse(Action<SubChannel> action)
        {
            action?.Invoke(coarse); // ...
        }

        public void SetFine(Action<SubChannel> action)
        {
            action?.Invoke(fine); // ...
        }

        public void SetUltra(Action<SubChannel> action)
        {
            action?.Invoke(ultra); // ...
        }

        public DMXChannel Clone()
        {
            var clone = new DMXChannel();
            clone.Value = Value;
            return clone;
        }
    }
}