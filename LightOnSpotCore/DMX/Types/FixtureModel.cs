namespace LightOnSpotCore.DMX.Types
{
    public class FixtureModel
    {
        private List<Parameter> parameters = new List<Parameter>();
        public List<Parameter> Parameters { get { return parameters; } }

        public FixtureModel() { }

        public void AddParameter(Parameter parameter)
        {
            if (parameter == null) throw new Exception("Parameter is null");
            parameters.Add(parameter);
        }

        public void RemoveParameter(Parameter parameter)
        {
            if (parameter == null) throw new Exception("Parameter is null");
            parameters.Remove(parameter);
        }

        public void RemoveParameter(int index)
        {
            parameters.RemoveAt(index);
        }

        public void ChangeParameter(int index, Action<Parameter> change)
        {
            if (parameters.Count == 0) throw new ArgumentOutOfRangeException("There are no parameters");
            var parameter = parameters[index];
            change?.Invoke(parameter);
            parameters[index] = parameter;
        }

        public void ChangeParameter(string name, Action<Parameter> change)
        {
            if (parameters.Count == 0) throw new ArgumentOutOfRangeException("There are no parameters");
            var parameterIndex = 0;

            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].Name == name)
                {
                    parameterIndex = i;
                    break;
                }
            }

            ChangeParameter(parameterIndex, change);
        }

        public FixtureModel Clone()
        {
            var fixture = new FixtureModel();

            foreach (var parameter in parameters)
            {
                fixture.AddParameter(parameter.Clone());
            }

            return fixture;
        }

        public Dictionary<int, byte> GetDmxValues()
        {
            var dict = new Dictionary<int, byte>();

            int i = 0;

            foreach (var parameter in parameters)
            {
                var dmxDataValues = parameter.DmxChannel.GetDmxValues();
                foreach (var data in dmxDataValues)
                {
                    dict.Add(i + data.Key, data.Value);
                }

                i++;
            }

            return dict;
        }
    }
}
