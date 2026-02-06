using System.Drawing;
using System.Numerics;
using LightOnSpotCore.DMX.Fixtures;
using LightOnSpotCore.DMX.Types;
using LightOnSpotCore.MIDI;
using Unity_DMX.Core;

namespace LightOnSpotCore
{
    public class DroneExample
    {
        private static LightingDrone[] drones = new LightingDrone[1000];
        private static LightingDroneData[] droneData = new LightingDroneData[1000];

        private static FixtureModel _droneFixture;

        //private static FixtureModel[] _fixturesPatch = new FixtureModel[1000];

        public static void Start()
        {
            /*if (_droneFixture == null)
            {
                _droneFixture = new FixtureModel();

                var posTemplate = new DMXChannel(0, 127, true);
                posTemplate.SetFine(f => f.Offset = 1);

                var colTemplate = new DMXChannel(0, 255, true);
                colTemplate.SetFine(f => f.Offset = 1);

                _droneFixture.AddParameter(new Parameter("X", "", posTemplate.Clone(), new ValueRange<double>(-800, 800)));
                _droneFixture.AddParameter(new Parameter("Y", "", posTemplate.Clone(), new ValueRange<double>(-800, 800)));
                _droneFixture.AddParameter(new Parameter("Z", "", posTemplate.Clone(), new ValueRange<double>(-800, 800)));
                _droneFixture.AddParameter(new Parameter("R", "", colTemplate.Clone(), new ValueRange<double>(0, 1)));
                _droneFixture.AddParameter(new Parameter("G", "", colTemplate.Clone(), new ValueRange<double>(0, 1)));
                _droneFixture.AddParameter(new Parameter("B", "", colTemplate.Clone(), new ValueRange<double>(0, 1)));
            }

            for (int i = 0; i < _fixturesPatch.Length; i++)
            {
                _fixturesPatch[i] = _droneFixture.Clone();
            }*/

            // if we try to set parameter: // Y:
            // _droneFixture.ChangeParameter(1, p => p.SetValue(21)); // might be a bit too heavy ....

            // Start definition of fixture template
            // to make new dmx channel that represents offset, raw float value 
            // var posTemplate = new DMXChannel(0, 127, true);  // new dmx channel (offset, value, use fine)
            // posTemplate.SetFine(f => f.Offset = 1);          // set fine offset

            // add new parameter:
            // _droneFixture.AddParameter(new Parameter("X", "",
            //      posTemplate.Clone(), new ValueRange<double>(-800, 800)));
            // End definition of fixture template

            // to change parameter value later in code on fixture instances
            // that are mapped to specific drones in world
            //_droneFixture.ChangeParameter("X", p => p.SetValue(450));

            // is this too weird way of doing it?

            for (int i = 0; i < drones.Length; i++)
            {
                drones[i] = new LightingDrone();
                var data = droneData[i] = new LightingDroneData(drones[i]);

                data.StartPosition = new Vector3().RandomSingle();
                data.CurrentPosition = data.StartPosition;
            }

            MidiExperiment.Instance.NoteLink.Set(1, true);
        }

        public static void UpdateDrones(ref DmxData dmxBuffer)
        {
            var baseOffset = 2880;

            var blend0 = MidiExperiment.Instance.Blending[0];
            var blend1 = MidiExperiment.Instance.Blending[1];
            var blend2 = MidiExperiment.Instance.Blending[2];
            var blend3 = MidiExperiment.Instance.Blending[3];

            var toggle0 = MidiExperiment.Instance.NoteState[1];

            var speed = .05f * blend0;
            var speed2 = .5f * blend1;

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

                var pos1 = Vector3.Lerp(newPosition1, newPosition2, blend2);
                var pos = Vector3.Lerp(pos1, newPosition3, blend3);

                var col1 = newColor1.Lerp(newColor2, blend2);
                var col = col1.Lerp(newColor3, blend3);

                d.SetPosition(pos);
                d.SetColor(toggle0 ? col : Color.Black);

                /*d.ChangeParameter("X", p => p.SetValue(pos.X));
                d.ChangeParameter("Y", p => p.SetValue(pos.Y));
                d.ChangeParameter("Z", p => p.SetValue(pos.Z));

                d.ChangeParameter("R", p => p.SetValue(col.R));
                d.ChangeParameter("G", p => p.SetValue(col.G));
                d.ChangeParameter("B", p => p.SetValue(col.B));*/

                //var dmxData = d.GetDmxValues();

                var data = d.GetBytes();
                //var dmxOffset = baseOffset + (i * 9);
                var dmxOffset = baseOffset + (i * data.Length);

                /*foreach (var keyValue in dmxData)
                {
                    dmxBuffer.Set(dmxOffset + keyValue.Key, keyValue.Value);
                }*/

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
