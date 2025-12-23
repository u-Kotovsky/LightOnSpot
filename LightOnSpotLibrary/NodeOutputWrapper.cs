using ArtNetSharp;
using ArtNetSharp.Communication;
using org.dmxc.wkdt.Light.ArtNet;

namespace LightOnSpotLibrary
{
    internal class NodeOutputWrapper
    {
        private Task? updateTask;
        private NodeInstance? nodeInstance;

        public void Start()
        {
            Console.WriteLine("Node Output Exampler!");

            //Add Logging
            //ArtNet.SetLoggerFectory(YOUR_LOGGER_FACTORY);

            //Set Networkinterfaces
            //var broadcastIp = new IPAddress(new byte[] { 2, 255, 255, 255 });
            //ArtNet.Instance.NetworkClients.ToList().ForEach(ncb => ncb.Enabled = IPAddress.Equals(broadcastIp, ncb.BroadcastIpAddress));

            // Create Instance
            nodeInstance = new NodeInstance(ArtNetSharp.ArtNet.Instance);
            nodeInstance.Name = nodeInstance.ShortName = "Node Output Example";

            // Configure Output Ports
            for (byte i = 1; i <= 32; i++)
            {
                var portAddress = new PortAddress((ushort)(i - 1));
                var goodOutput = new GoodOutput(outputStyle: GoodOutput.EOutputStyle.Continuous, isBeingOutputAsDMX: true);
                var portConfig = new PortConfig(i, portAddress, true, false)
                { 
                    PortNumber = i,
                    Type = EPortType.OutputFromArtNet,
                    GoodOutput = goodOutput
                };
                nodeInstance.AddPortConfig(portConfig);
            }

            // Listen for new Data
            nodeInstance.DMXReceived += (sender, e) =>
            {
                if (!(sender is NodeInstance ni))
                    return;

                // Can be called from anywere anytime without listen to the Event!!!
                var data = ni.GetReceivedDMX(e);

                Console.WriteLine($"Received date for {e}: {data.Length} bytes");
            };

            // Add Instance
            ArtNetSharp.ArtNet.Instance.AddInstance(nodeInstance);

            updateTask = new Task(Update);
            updateTask.Start();
        }

        private async void Update()
        {
            // Genrerate some DMX-Data
            byte[] data = new byte[512];
            do
            {
                await Task.Delay(200);
                for (short k = 0; k < 512; k++)
                    data[k]++;

                foreach (var port in nodeInstance.PortConfigs)
                    nodeInstance.WriteDMXValues(port.PortAddress, data);
                Console.WriteLine("Sent data " + DateTime.Now.Millisecond);
            } while (true);
        }
    }
}
