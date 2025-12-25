using LightOnSpot.Core.unity.dmx.Core;
using Unity_DMX.Core;

namespace LightOnSpotCore
{
    public class UnityDmxWrapper
    {
        internal DmxBuffer dmxBuffer; // Shared DMX512 buffer
        public DmxBuffer DmxBuffer { get { return dmxBuffer; } }

        internal DmxController controller;
        public DmxController Input { get { return controller; } }

        internal DmxController outputController;
        public DmxController Output { get { return controller; } }

        public void Start()
        {
            Console.WriteLine("Starting UnityDmxWrapper..");

            // Output
            SimpleSocketAddress outputAddress = new SimpleSocketAddress("127.0.0.1", 6455);
            Console.WriteLine($"Output is '{outputAddress.EndPoint}'");

            outputController = new DmxController(new DmxBuffer());
            outputController.RemoteAddress = outputAddress;
            outputController.StartArtNet();

            // Input
            SimpleSocketAddress inputAddress = new SimpleSocketAddress("127.0.0.1", 6454);
            Console.WriteLine($"Input is '{inputAddress.EndPoint}'");

            dmxBuffer = new DmxBuffer();
            controller = new DmxController(dmxBuffer);
            controller.RemoteAddress = inputAddress;
            controller.DmxOutput = outputController;
            controller.RedirectPackets = true;
            controller.StartArtNet();

            Console.WriteLine("UnityDmxWrapper is now running.");
        }
    }
}
