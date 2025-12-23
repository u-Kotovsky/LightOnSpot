using System.Net;
using ArtNetSharp;
using ArtNetSharp.Communication;
using org.dmxc.wkdt.Light.ArtNet;

namespace LightOnSpotLibrary
{
    internal class NodeInputWrapper
    {
        private Task? updateTask;
        private NodeInstance? nodeInstance;
        private IPAddress? broadcastIp;

        public void Start()
        {
            Console.WriteLine("Node Input Example!");

            //Add Logging
            //ArtNet.SetLoggerFectory(YOUR_LOGGER_FACTORY);

            //Set Networkinterfaces
            broadcastIp = new IPAddress([2, 255, 255, 255]);
            ArtNetSharp.ArtNet.Instance.NetworkClients.ToList().ForEach(ncb => ncb.Enabled = IPAddress.Equals(broadcastIp, ncb.BroadcastIpAddress));

            // Create Instance
            nodeInstance = new NodeInstance(ArtNetSharp.ArtNet.Instance);
            nodeInstance.Name = nodeInstance.ShortName = "Node Input Example";

            // Configure Input Ports
            for (byte i = 1; i <= 4; i++)
            {
                var portAddress = new PortAddress((ushort)(i - 1));
                var portConfig = new PortConfig(i, portAddress, false, true)
                { 
                    PortNumber = i, 
                    Type = EPortType.InputToArtNet | EPortType.ArtNet 
                };
                nodeInstance.AddPortConfig(portConfig);
            }

            for (byte i = 11; i <= 14; i++)
            {
                var portAddress = new PortAddress((ushort)(i - 1));
                var portConfig = new PortConfig(i, portAddress, false, true) 
                {
                    PortNumber = i,
                    Type = EPortType.InputToArtNet | EPortType.ArtNet 
                };
                nodeInstance.AddPortConfig(portConfig);
            }

            nodeInstance.AddPortConfig(new PortConfig(15, new PortAddress(6454), false, true));

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
                    data[k] = 255;

                foreach (var port in nodeInstance.PortConfigs)
                    nodeInstance.WriteDMXValues(port.PortAddress, data);
                Console.WriteLine("Sent data " + DateTime.Now.Millisecond);
            } while (true);
        }
    }
}
