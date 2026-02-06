using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Windows.Controls;
using LightOnSpotApp.Pages;
using LightOnSpotApp.Services;
using LightOnSpotCore;
using LightOnSpotCore.DMX.Fixtures;
using LightOnSpotCore.DMX.Types;

namespace LightOnSpotApp.ViewModels
{
    public class MainWindowViewModel
    {
        private static UnityDmxWrapper dmxWrapper;
        private static Task task;

        private static LightingDrone[] drones = new LightingDrone[1000];

        public MainWindowViewModel(Frame mainFrame)
        {
            MainFrameService.Instance.MainFrame = mainFrame;
            MainFrameService.Instance.MainFrame.Navigate(new StartPage());

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

        public double DeltaTime { get { return Time.deltaTime; } }

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
                Time.deltaTime = ((float)stopwatch.ElapsedTicks) / 10000000;
                stopwatch.Start();

                //UpdateClouds();
                UpdateSkybox();
                UpdateDrones();

                dmxWrapper.Input.ForceBufferUpdate();

                Thread.Sleep(1000 / 60);
            } while (true);
        }

        private static DmxColor black = new DmxColor(Color.Black);
        private static DmxColor red = new DmxColor(Color.Red);
        private static DmxColor green = new DmxColor(Color.Green);
        private static DmxColor blue = new DmxColor(Color.Blue);
        private static DmxColor yellow = new DmxColor(Color.Yellow);

        private static void UpdateSkybox()
        {
            dmxWrapper.DmxBuffer.Buffer.Set(2760, 255); // Enable Skybox Control
            dmxWrapper.DmxBuffer.Buffer.SetRange(2762, black.GetBytes()); // Top Color
            dmxWrapper.DmxBuffer.Buffer.SetRange(2765, black.GetBytes()); // Horizon Color
            dmxWrapper.DmxBuffer.Buffer.SetRange(2768, black.GetBytes()); // Bottom Color
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


                var timeOffset = Time.deltaTime * speed + (i);// * Math.PI * 2;

                var timeSin = (float)Math.Sin(timeOffset);
                var timeCos = (float)Math.Cos(timeOffset);

                var radius = i % 255;

                var height = i % 256;

                d.SetPosition(timeSin, timeCos, height);


                /*var ind = i % 500;

                var timeOffset = Time.deltaTime * speed + (i);// * Math.PI * 2;

                var timeSin = Math.Sin(timeOffset);
                var timeCos = Math.Cos(timeOffset);

                var radius = i % 3 + 1 * 4;

                var tower = (i % 32) - 1;

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

                var center = new Vector3(0, 0, 2 + (tower * 2));
                var height = ind * 0.05f * (float)timeCos + (float)timeSin;
                var pos = new Vector3((float)timeSin * radius, (float)timeCos * radius, 5 + height);

                var mixed = pos + center;

                d.SetPosition(mixed);

                //var offsetCos1 = (byte)(255 - (byte)(Math.Sin((timeSin)) * 255));
                // d.SetColor(offsetCos1, offsetCos1, offsetCos1);
                d.SetColor(Color.Red);*/

                /*var index = i % 4;

                var timeOffset = Time.deltaTime * speed + (i * Math.PI / 70);// * Math.PI * 2;
                var timeSin = Math.Sin(timeOffset);
                var timeCos = Math.Cos(timeOffset);
                var timeCbrt = Math.Cbrt(timeOffset);
                var timeSqrt = Math.Sqrt(timeOffset);
                var timeAcos = Math.Acos(timeOffset);
                var timeTan = Math.Tan(timeOffset);
                var timeExp = Math.Exp(timeOffset);
                var timeSinh = Math.Sinh(timeOffset);
                var timeTanh = Math.Tanh(timeOffset);
                var timeLog = Math.Log(timeOffset);
                var timeReciprocalEstimate = Math.ReciprocalEstimate(timeOffset);

                var mult = 16;
                var mult2 = .15f;

                var num = (float)timeSin * mult;
                var num2 = 2 + (i % 256) * mult2;
                var num3 = 2 + (i % 256) * (float)timeCos * mult2;

                switch (index)
                {
                    case 0:
                        d.SetPosition(num, num3, num2);
                        d.SetColor(Color.Red);
                        break;
                    case 1:
                        d.SetPosition(-num, num3, num2);
                        d.SetColor(Color.Green);
                        break;
                    case 2:
                        d.SetPosition(num3, num, num2);
                        d.SetColor(Color.Yellow);
                        break;
                    case 3:
                        d.SetPosition(num3, -num, num2);
                        d.SetColor(Color.Aqua);
                        break;
                }*/

                var data = d.GetBytes();
                var dmxOffset = baseOffset + (i * data.Length);

                dmxWrapper.DmxBuffer.Buffer.SetRange(dmxOffset, data);
            }
        }

        //private static DmxColor cloudsLight = new DmxColor(Color.White);
        //private static DmxColor cloudsDark = new DmxColor(Color.Black);
        private static DmxColor cloudsLight = new DmxColor((22 / 100) * 255, (30 / 100) * 255, (56 / 100) * 255);
        private static DmxColor cloudsDark = new DmxColor((3 / 100) * 255, (4 / 100) * 255, (7 / 100) * 255);

        private static void UpdateClouds()
        {
            dmxWrapper.DmxBuffer.Buffer.Set(2850, 255); // Enable Clouds Control
            dmxWrapper.DmxBuffer.Buffer.SetRange(2852, cloudsLight.GetBytes()); // Light Color
            dmxWrapper.DmxBuffer.Buffer.SetRange(2855, cloudsDark.GetBytes()); // Dark Color
            dmxWrapper.DmxBuffer.Buffer.Set(2858, 0); // Video map intensity
            dmxWrapper.DmxBuffer.Buffer.Set(2859, 0); // Depth map intensity
            dmxWrapper.DmxBuffer.Buffer.Set(2860, 32); // Depth offset
        }
    }
}
