using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using LightOnSpotCore;

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
            dmxWrapper.DmxBuffer.Buffer.EnsureCapacity(capacity);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            do
            {
                stopwatch.Stop();
                Time.deltaTime = ((double)stopwatch.ElapsedTicks) / 10000000;
                stopwatch.Start();

                UpdateDrones();

                dmxWrapper.Input.ForceBufferUpdate();

                Thread.Sleep(1000 / 60);
            } while (true);
        }

        private static void UpdateDrones()
        {
            var baseOffset = 2880;

            var speed = .05f;
            var speed2 = .5f;

            dmxWrapper.DmxBuffer.Buffer.EnsureCapacity(baseOffset + (drones.Length * 9));

            dmxWrapper.DmxBuffer.Buffer.Set(2802, 255); // Enable Misc Control
            dmxWrapper.DmxBuffer.Buffer.Set(2804, 255); // Enable Huge Drone Map

            for (int i = 0; i < drones.Length; i++)
            {
                var d = drones[i];
                var j = i % 500;

                if (i < 500)
                {
                    Circle(ref d, j, speed, 256, 2, 1, new Vector3(0, 0, 15));
                }

                if (i >= 500)
                {
                    Thing1(ref d, i, speed);
                }

                var data = d.GetBytes();
                var dmxOffset = baseOffset + (i * data.Length);

                dmxWrapper.DmxBuffer.Buffer.SetRange(dmxOffset, data);
            }
        }

        private static void Circle(ref LightingDrone d, int i, 
            float speed, int count, float r, float heightOffset,
            Vector3 offset = new Vector3())
        {
            var circleIndex = (i % count) + 1;
            var timeWithSpeed = Time.deltaTime * speed * 16;
            var (timeSin, timeCos) = Math.SinCos(timeWithSpeed + i);
            var timeSin2 = Math.Sin(timeWithSpeed + i);
            var absSin = (timeSin2 + 1) / 2;

            #region Position
            var local = new Vector3((float)timeSin * r, (float)timeCos * r, circleIndex * heightOffset);
            var mixed = local + offset;
            d.SetPosition(mixed);
            #endregion

            #region Color
            var col1 = Color.Aqua;
            var col2 = Color.Black;
            var colOut = Lerp(col2, col1, Math.Abs((float)timeSin2));
            d.SetColor(colOut);
            #endregion
        }

        private static void Thing1(ref LightingDrone d, int i, float speed)
        {
            var radius = 3f;
            var max = 32;
            var j = i % max;
            var circleIndex = i % 5;
            var circleIndex2 = circleIndex + 1;

            var timeWithSpeed = Time.deltaTime * speed;

            var (timeSin, timeCos) = ((float, float))Math.SinCos(timeWithSpeed + i);

            var timeSin2 = (float)Math.Sin(timeWithSpeed * circleIndex2 * 8 + i * 4);
            var timeSin3 = (float)Math.Sin(timeWithSpeed);

            radius *= (float)(1.15 * (Math.Abs(timeSin3) + 1));

            var tsp = (float)(timeSin + 1) / 2;
            var tsp2 = (float)(timeSin2 + 1) / 2;

            #region Position
            var center = new Vector3(0, 0, circleIndex * 8);
            var height = (timeSin2 * circleIndex2) + 32;
            var pos = new Vector3(timeSin * radius * circleIndex2, timeCos * radius * circleIndex2, height);

            var mixed = pos + center;

            d.SetPosition(mixed);
            #endregion

            #region Color
            /*
            var b = (int)(tsp * 255);
            var c1 = Color.FromArgb(b, 255, 255);
            var c2 = Color.FromArgb(255, b, 255);
            var c3 = Color.FromArgb(255, 255, b);
            d.SetColor(ShortMath.GetT(circleIndex, c1, c2, c3));
            */

            var col1 = Color.Aqua;
            var col2 = Color.Black;
            var colOut = Lerp(col2, col1, Math.Abs(timeSin2 * 0.25f));
            d.SetColor(colOut);
            #endregion
        }

        private static Color Lerp(Color min, Color max, float amount)
        {
            amount = Math.Clamp(amount, 0, 1);
            int r = (int)float.Lerp(min.R, max.R, amount);
            int g = (int)float.Lerp(min.G, max.G, amount);
            int b = (int)float.Lerp(min.B, max.B, amount);
            int a = (int)float.Lerp(min.A, max.A, amount);
            Color color = Color.FromArgb(a, r, g, b);
            return color;
        }
    }
}