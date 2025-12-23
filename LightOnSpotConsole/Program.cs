using System.Diagnostics;
using LightOnSpotCore;
using Unity_DMX.Core;

namespace LightOnSpotConsole
{
    internal class Program
    {
        private static UnityDmxWrapper dmxWrapper;
        private static Task task;

        private static LightingDrone[] drones = new LightingDrone[1000];

        static void Main(string[] args)
        {
            for (int i = 0; i < drones.Length; i++)
            {
                drones[i] = new LightingDrone();
            }

            Console.WriteLine("Running ArtNet");

            dmxWrapper = new UnityDmxWrapper();
            dmxWrapper.Start();

            Console.WriteLine("After start");

            // data test
            task = new Task(Update);
            task.Start();

            Console.ReadLine();
        }

        private static void Update()
        {
            var capacity = 512 * 23;
            dmxWrapper.Buffer.Buffer.EnsureCapacity(capacity);

            float deltaTime = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            do
            {
                stopwatch.Stop();
                var time = stopwatch.ElapsedMilliseconds;
                stopwatch.Start();

                for (int i = 0; i < drones.Length; i++)
                {
                    var d = drones[i];

                    var timeOffset = (time + i * 3) / 1000;

                    var timeSin = Math.Sin(timeOffset);
                    var timeCos = Math.Cos(timeOffset);

                    var multiplier = 24;

                    d.X = (float)timeSin* multiplier;
                    d.Y = (float)timeCos* multiplier;
                    d.Z = 2;

                    var data = new byte[] {
                        d.XCoarse,
                        d.XFine,
                        d.YCoarse,
                        d.YFine,
                        d.ZCoarse,
                        d.ZFine,
                        d.R,
                        d.G,
                        d.B
                    };

                    var dmxOffset = 2880 + (i * data.Length);

                    dmxWrapper.Buffer.Buffer.EnsureCapacity(dmxOffset + data.Length);
                    dmxWrapper.Buffer.Buffer.SetRange(dmxOffset, data);
                }
                dmxWrapper.Input.ForceBufferUpdate();
                Thread.Sleep(1000 / 30);

            } while (true);

        }
    }
}