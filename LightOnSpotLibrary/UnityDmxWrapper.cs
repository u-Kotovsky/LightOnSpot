using Unity_DMX.Core;

namespace LightOnSpotLibrary
{
    public class UnityDmxWrapper
    {
        private DmxBuffer dmxBuffer;
        private DmxController controller;
        private DmxController outputController;
        private Task task;

        public void Start()
        {
            Console.WriteLine("Starting UnityDmxWrapper..");
            dmxBuffer = new DmxBuffer();
            controller = new DmxController(dmxBuffer);

            string outputIp = "127.0.0.1";
            int outputPort = 6455;

            Console.WriteLine($"Output is '{outputIp}:{outputPort}'");

            outputController = new DmxController(new DmxBuffer());
            outputController.SetRemote("127.0.0.1", 6455);
            outputController.StartArtNet();

            string inputIp = "127.0.0.1";
            int inputPort = 6454;

            Console.WriteLine($"Input is '{inputIp}:{inputPort}'");

            controller.SetRemote("127.0.0.1", 6454);
            controller.SetRedirect(outputController);
            controller.RedirectPackets = true;
            controller.StartArtNet();

            Console.WriteLine("UnityDmxWrapper is now running.");

            task = new Task(Update);
            task.Start();
        }

        private void Update()
        {
            dmxBuffer.Buffer.EnsureCapacity(512);
            do
            {
                byte[] bytes = new byte[512];

                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = (byte)Random.Shared.Next(255);
                }

                dmxBuffer.Buffer.SetRange(0, bytes);
                controller.ForceBufferUpdate();

                Thread.Sleep(200);
            } while (true);
        }
    }
}
