namespace LightOnSpotCore.DMX.Utilities
{
    public class SubChannel
    {
        private int offset;
        public int Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        private bool use;
        public bool Use { get { return use; } set { use = value; } }

        private byte value;
        public byte Value
        {
            get { return value; }
            set
            {
                this.value = value;
                OnValueChange?.Invoke(value);
            }
        }

        public Action<byte> OnValueChange = delegate { };

        public SubChannel() { }

        public SubChannel(int offset, byte value, bool use)
        {
            this.offset = offset;
            this.use = use;
            this.value = value;
        }

        public static implicit operator byte(SubChannel subChannel) => subChannel.Value;
        //public static implicit operator SubChannel(byte value) => value; // ...
        public static SubChannel operator +(SubChannel left, SubChannel right)
        {
            left.Value += right.Value;
            return left;
        }
        public static SubChannel operator +(SubChannel left, byte right)
        {
            left.Value += right;
            return left;
        }
    }
}
