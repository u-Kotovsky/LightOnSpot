using LightOnSpot.Core.unity.dmx.Core;
using Microsoft.Extensions.Logging;
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

        private ILoggerFactory factory;
        private ILogger logger;

        public UnityDmxWrapper()
        {
            factory = LoggerFactory.Create(builder =>
            {
                builder.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.SingleLine = true;
                    options.TimestampFormat = "HH:mm:ss ";
                });
            });
            logger = factory.CreateLogger(nameof(UnityDmxWrapper));
        }

        public void Start()
        {
            logger.LogInformation("Starting..");

            dmxBuffer = new DmxBuffer();

            // Output
            SimpleSocketAddress outputAddress = new("127.0.0.1", 6455);
            //logger.LogInformation($"Output is '{outputAddress.EndPoint}'");

            outputController = new(dmxBuffer);
            outputController.RemoteAddress = outputAddress;
            outputController.StartArtNet();

            // Input
            SimpleSocketAddress inputAddress = new("127.0.0.1", 6454);
            //logger.LogInformation($"Input is '{inputAddress.EndPoint}'");

            controller = new(dmxBuffer);
            controller.isServer = true;
            controller.RemoteAddress = inputAddress;
            controller.DmxOutput = outputController;
            controller.RedirectPackets = true;
            controller.StartArtNet();

            logger.LogInformation("Started");
        }
    }
}