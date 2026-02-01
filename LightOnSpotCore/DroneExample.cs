using System.Drawing;
using System.Numerics;
using LightOnSpotCore.DMX.Fixtures;
using LightOnSpotCore.MIDI;
using Unity_DMX.Core;

namespace LightOnSpotCore
{
    public class DroneExample
    {
        private static LightingDrone[] drones = new LightingDrone[1000];
        private static LightingDroneData[] droneData = new LightingDroneData[1000];

        public static void Start()
        {
            for (int i = 0; i < drones.Length; i++)
            {
                drones[i] = new LightingDrone();
                var data = droneData[i] = new LightingDroneData(drones[i]);

                data.StartPosition = new Vector3().RandomSingle();
                data.CurrentPosition = data.StartPosition;
            }
        }

        public static void UpdateDrones(ref DmxData dmxBuffer)
        {
            var baseOffset = 2880;

            var speed = .05f * MidiExperiment.Blending[0];
            var speed2 = .5f * MidiExperiment.Blending[1];

            dmxBuffer.EnsureCapacity(baseOffset + (drones.Length * 9));

            dmxBuffer.Set(2802, 255); // Enable Misc Control
            dmxBuffer.Set(2804, 255); // Enable Huge Drone Map

            for (int i = 0; i < drones.Length; i++)
            {
                var d = drones[i];
                var da = droneData[i];
                var j = i;// % 500;

                Calculate1(i, j, speed, da.StartPosition, out var newPosition1, out var newColor1);
                Calculate2(i, j, speed, da.StartPosition, out var newPosition2, out var newColor2);
                Calculate3(i, j, speed, da.StartPosition, out var newPosition3, out var newColor3);

                var pos = Vector3.Lerp(Vector3.Lerp(newPosition1, newPosition2, MidiExperiment.Blending[2]), newPosition3, MidiExperiment.Blending[3]);

                var col1 = newColor1.Lerp(newColor2, MidiExperiment.Blending[2]);
                var col = col1.Lerp(newColor3, MidiExperiment.Blending[3]);

                d.SetPosition(pos);
                d.SetColor(col);

                var data = d.GetBytes();
                var dmxOffset = baseOffset + (i * data.Length);

                dmxBuffer.SetRange(dmxOffset, data);
            }
        }

        private static void Calculate1(in int i, in int j, in float speed, in Vector3 startPosition, out Vector3 position, out Color color)
        {
            if (i < 500)
            {
                //Circle(ref da, j, speed, 256, 2, 1, new Vector3(0, 0, 15));
                Test(j, startPosition, out position, out color);
            }
            else if (i >= 500)
            {
                Thing1(j, speed, out position, out color);
            }
            else
            {
                position = Vector3.Zero;
                color = Color.Red;
            }

            //Thing1(ref da, i, speed);

            //Circle(ref da, i, speed, 256, 2, 1, new Vector3(0, 0, 15));
            //Test(ref da, j);
        }

        private static void Calculate2(in int i, in int j, in float speed, in Vector3 startPosition, out Vector3 position, out Color color)
        {
            Circle(i, speed, 256, 2, 1, new Vector3(0, 0, 15), startPosition, out position, out color);
            //Test(ref da, j);
        }

        private static void Calculate3(in int i, in int j, in float speed, in Vector3 startPosition, out Vector3 position, out Color color)
        {
            Circle(i, speed, 12, 24, 1, new Vector3(0, 0, 15), startPosition, out position, out color);
            //Test(ref da, j);
        }

        private static void Circle(in int i, in float speed, in int count, in float r, in float heightOffset, in Vector3 offset,
            in Vector3 startPosition, out Vector3 position, out Color color)
        {
            var circleIndex = (i % count) + 1;
            var timeWithSpeed = Time.deltaTime * speed * 16;
            var (timeSin, timeCos) = Math.SinCos(timeWithSpeed + i);
            var timeSin2 = Math.Sin(timeWithSpeed + i);
            var timeAsin = Math.Log(timeWithSpeed + i);
            //var absSin = (timeSin2 + 1) / 2;

            #region Position
            var local = new Vector3((float)timeSin * r, (float)timeCos * r, circleIndex * heightOffset);
            var mixed = local + offset + startPosition;
            position = mixed;
            #endregion

            #region Color
            var col1 = Color.Aqua;
            var col2 = Color.LawnGreen;
            var colOut = col2.Lerp(col1, Math.Abs((float)timeSin2));
            color = colOut;
            #endregion
        }

        private static void Test(in int i, in Vector3 startPosition, out Vector3 position, out Color color)
        {
            var speed = .5;
            var count = 4;
            var heightOffset = 8;
            var r = 12;
            var offset = new Vector3(0, 0, 28);

            var circleIndex = (i % count) + 1;
            var timeWithSpeed = Time.deltaTime * speed;
            var (timeSin, timeCos) = Math.SinCos(timeWithSpeed + i);
            var timeSin2 = Math.Sin(timeWithSpeed + i);
            //var absSin = (timeSin2 + 1) / 2;

            #region Position
            var local = new Vector3((float)timeSin * r, (float)timeCos * r, circleIndex * heightOffset);
            var mixed = startPosition * 14 + offset + local;// local + offset + d.StartPosition;
            position = mixed;
            #endregion

            #region Color
            var col1 = Color.Aqua;
            var col2 = Color.Black;
            var colOut = col2.Lerp(col1, Math.Abs((float)timeSin2));
            color = colOut;
            #endregion
        }

        private static void Thing1(int i, float speed, out Vector3 position, out Color color)
        {
            var radius = 2f;
            var max = 32;
            var j = i % max;
            var circleIndex = i % 12;
            var circleIndex2 = circleIndex + 1;

            var timeWithSpeed = Time.deltaTime * speed;

            var (timeSin, timeCos) = ((float, float))Math.SinCos(timeWithSpeed + i);

            var timeSin2 = (float)Math.Sin(timeWithSpeed * circleIndex2 * 8 + i * 4);
            var timeSin3 = (float)Math.Sin(timeWithSpeed);

            radius *= (float)(1.15 * (Math.Abs(timeSin3) + 1));

            //var tsp = (float)(timeSin + 1) / 2;
            //var tsp2 = (float)(timeSin2 + 1) / 2;

            #region Position
            var center = new Vector3(0, 0, circleIndex * 4);
            var height = (timeSin2 * circleIndex2) + 32;
            var pos = new Vector3(timeSin * radius * circleIndex2, timeCos * radius * circleIndex2, height);

            var mixed = pos + center;// + d.StartPosition;
            position = mixed;
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
            var colOut = col2.Lerp(col1, Math.Abs(timeSin2 * 0.25f));
            color = colOut;
            #endregion
        }

    }
}
