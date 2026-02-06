namespace LightOnSpotCore.DMX.Utilities
{
    public class Parameter
    {
        private string name;
        public string Name { get { return name; } set { name = value; } }

        private string description;
        public string Description { get { return description; } set { description = value; } }

        private DMXChannel dmxChannel;
        public DMXChannel DmxChannel { get { return dmxChannel; } set { dmxChannel = value; }  }

        //private double value;
        //public double Value { get { return value; } set { this.value = value; } }

        private ValueRange<double> valueRange;
        public ValueRange<double> ValueRange { get { return valueRange; } set { valueRange = value; } }

        public Parameter() { }

        public Parameter(string name, string description, DMXChannel dmxChannel, /*double value,*/ ValueRange<double> valueRange)
        {
            Name = name;
            Description = description;
            DmxChannel = dmxChannel;
            //Value = value;
            ValueRange = valueRange;
        }

        public Parameter Clone()
        {
            var parameter = new Parameter
            {
                name = name,
                description = description,
                dmxChannel = dmxChannel,
                //value = value,
                valueRange = valueRange
            };

            return parameter;
        }

        public void SetValue(float value)
        {
            dmxChannel.Value = value;
        }
    }
}
