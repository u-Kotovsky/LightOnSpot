using System.Net;
using ArtNetSharp;
using ArtNetSharp.Communication;
using org.dmxc.wkdt.Light.ArtNet;

namespace LightOnSpotLibrary
{
    internal class ControllerWrapper
    {
        private Task? updateTask;
        private ControllerInstance? controllerInstance;
        private IPAddress? broadcastIp;

        public void Start()
        {
            Console.WriteLine("Controller Example!");

            //Add Logging
            //ArtNet.SetLoggerFectory(YOUR_LOGGER_FACTORY);

            //Set Networkinterfaces
            broadcastIp = new IPAddress([2, 255, 255, 255]);
            //broadcastIp = new IPAddress([255, 255, 255, 255]);
            //broadcastIp = new IPAddress([0, 0, 0, 0]);
            //broadcastIp = new IPAddress([127, 0, 0, 1]);
            ArtNetSharp.ArtNet.Instance.NetworkClients.ToList().ForEach(ncb => ncb.Enabled = IPAddress.Equals(broadcastIp, ncb.BroadcastIpAddress));

            // Create Instance
            controllerInstance = new ControllerInstance(ArtNetSharp.ArtNet.Instance);
            controllerInstance.Name = controllerInstance.ShortName = "Controller Example";

            // Configure Ports
            for (byte i = 1; i <= 32; i++)
                controllerInstance.AddPortConfig(new PortConfig(i, new PortAddress((ushort)(i - 1)), false, true) { PortNumber = i, Type = EPortType.InputToArtNet | EPortType.ArtNet });

            // Add Instance
            ArtNetSharp.ArtNet.Instance.AddInstance(controllerInstance);

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
                for (short k = 0; k < 512; k++) // 512 channels
                    data[k]++;

                for (ushort i = 0; i < 32; i++) // 32 universes
                    controllerInstance?.WriteDMXValues(i, data);

                Console.WriteLine("Sent data " + DateTime.Now.Millisecond);
            } while (true);
        }
    }
}
