using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using LightOnSpotCore;

namespace LightOnSpotApp.ViewModels
{
    public class ConsoleViewModel
    {
        private static UnityDmxWrapper dmxWrapper;
        private static Task task;

        private static LightingDrone[] drones = new LightingDrone[1000];

        public ConsoleViewModel()
        {
            for (int i = 0; i < drones.Length; i++)
            {
                drones[i] = new LightingDrone();
            }

            dmxWrapper = new UnityDmxWrapper();
            dmxWrapper.Start();

            // data test
            task = new Task(Update);
            task.Start();
        }

        private double deltaTime = 0;
        public double DeltaTime { get { return deltaTime; } }

        public int DroneCount { get { return drones.Length; } }

        public int BufferCapacity { get { return dmxWrapper.DmxBuffer.Buffer.Count; } }

        private void Update()
        {
            var capacity = 512 * 23;
            dmxWrapper.DmxBuffer.Buffer.EnsureCapacity(capacity);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            do
            {
                stopwatch.Stop();
                deltaTime = ((float)stopwatch.ElapsedTicks) / 10000000;
                stopwatch.Start();

                UpdateDrones(deltaTime);

                dmxWrapper.Input.ForceBufferUpdate();

                Thread.Sleep(1000 / 60);
            } while (true);
        }

        private static void UpdateDrones(double deltaTime)
        {
            var baseOffset = 2880;

            var speed = .1f;
            var speed2 = 1f;

            dmxWrapper.DmxBuffer.Buffer.EnsureCapacity(baseOffset + (drones.Length * 9));

            //dmxWrapper.DmxBuffer.Buffer.Set(2760, 255); // Enable Skybox Control
            //dmxWrapper.DmxBuffer.Buffer.SetRange(2762, new DmxColor(Color.Red).GetBytes(), );

            for (int i = 0; i < drones.Length; i++)
            {
                var d = drones[i];

                var ind = i % 400;

                var timeOffset = deltaTime * speed + (i);// * Math.PI * 2;

                var timeSin = Math.Sin(timeOffset);
                var timeCos = Math.Cos(timeOffset);

                var radius = i % 2 + 1 * 32;

                var tower = (i % 2) - 1;

                switch (tower)
                {
                    case -1:
                        d.SetColor(Color.Red);
                        break;
                    case 0:
                        d.SetColor(Color.Green);
                        break;
                    case 1:
                        d.SetColor(Color.Blue);
                        break;
                }

                var center = new Vector3(0, 0, 2 + (tower * 5));
                var height = ind * 0.05f * (float)timeSin;
                var pos = new Vector3((float)timeSin * radius, (float)timeCos * radius, 30 + height);

                var mixed = pos + center;

                d.SetPosition(mixed);

                //byte r = d.Color.R >= 255 ? (byte)0 : (byte)(d.Color.R + 1);
                //byte g = d.Color.G >= 255 ? (byte)0 : (byte)(d.Color.R + 1);
                //byte b = d.Color.B >= 255 ? (byte)0 : (byte)(d.Color.R + 1);

                var offsetCos1 = (byte)(255 - (byte)(Math.Sin((timeSin)) * 255));
                d.SetColor(offsetCos1, offsetCos1, offsetCos1);
                //d.SetColor(Color.Red);

                var data = d.GetBytes();
                var dmxOffset = baseOffset + (i * data.Length);


                dmxWrapper.DmxBuffer.Buffer.SetRange(dmxOffset, data);
            }
        }
    }
}
