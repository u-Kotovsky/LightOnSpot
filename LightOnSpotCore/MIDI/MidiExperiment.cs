using System.Collections;
using System.Net.Sockets;
using CoreOSC;
using CoreOSC.IO;

namespace LightOnSpotCore.MIDI
{
    public class MidiExperiment
    {
        private static MidiExperiment? instance;
        public static MidiExperiment Instance { get { return instance; } }

        private BitArray _noteLinked = new(64);
        public BitArray NoteLink { get { return _noteLinked; } }

        private BitArray _noteState = new(64);
        public BitArray NoteState { get { return _noteState; } }

        private float[] blending = new float[8];
        public float[] Blending { get { return blending; } set { blending = value; } }

        private MidiInWrapper midiIn;
        private MidiOutWrapper midiOut;

        public MidiExperiment()
        {
            instance = this;
            // Initialize things
            //midiIn = new MidiInWrapper("MIDIIN2 (APC mini mk2)");
            //midiOut = new MidiOutWrapper("MIDIOUT2 (APC mini mk2)");
            midiIn = new MidiInWrapper("APC mini mk2");
            midiOut = new MidiOutWrapper("APC mini mk2");
            //timeCodeIn = new MidiInWrapper();
            timeCodeOut = new MidiOutWrapper("MidiLoop");
            timeCodeGenerator = new MidiTimeCodeGenerator(timeCodeOut);
            //oscSender = new OscSender("127.0.0.1", 7000); // Resolume Input

            Start();
        }

        //private static MidiInWrapper timeCodeIn;
        private static MidiOutWrapper? timeCodeOut;
        private static MidiTimeCodeGenerator? timeCodeGenerator;

        private void ResetFeedback()
        {
            for (byte i = 0; i < _noteLinked.Length; i++)
            {
                if (_noteLinked[i])
                {
                    _noteState[i] = !_noteState[i];
                    SetNoteState(midiOut, i, _noteState[i]);
                }

                //SendNoteOn(0, (byte)((i % 15) + 1), i, 127, 0);
                //SendNoteOn(0, 7, i, (byte)(i % 63), 0);
                //midiOut.SendNoteOn(0, 7, i, 75, 0);
                //_noteToggles[i] = true;
                //SetNoteState(midiOut, i, _noteToggles[i]);
                // channel is a intensity of color?
                // velocity is a color?
            }
        }

        //private static Thread timeCodeThread;
        //private static OscSender oscSender;

        private static void SetNoteState(MidiOutWrapper midiOut, int channel, bool enabled)
        {
            if (enabled)
            {
                midiOut.SendNoteOn(0, 7, channel, 75, 0);
            }
            else
            {
                midiOut.SendNoteOn(0, 7, channel, 90, 0);
            }
        }

        public void Start()
        {
            // Apply defaults
            //midiIn.SelectDevice("MIDIIN");
            //midiOut.SelectDevice("MIDIOUT");
            //midiIn.SelectDevice("APC mini mk2");
            //midiOut.SelectDevice("APC mini mk2");
            //midiIn.ApplyDevice(0);
            //midiIn.ApplyDevice(1);
            //timeCodeIn.SelectDevice("MidiLoop");
            //timeCodeOut.SelectDevice("MidiLoop");

            timeCodeGenerator.OnSmpteOffsetEvent += (@event) =>
            {
                string text = Extensions.GetReadableTime(@event);

                // uuh this is not best way to do it but it works for now...
                var client = new UdpClient("127.0.0.1", 7000);
                var message = new OscMessage(new Address("/composition/layers/20/clips/1/video/source/textgenerator/text/params/lines"), new object[] { text });
                Task.Run(async () =>
                {
                    await client.SendMessageAsync(message);
                });

                //Console.Title = text;
                //oscSender.Send("/composition/layers/1/clips/11/video/source/textgenerator/text/params/lines", text);
            };

            // Apply post-methods
            midiIn.NoteOff += (noteEvent) =>
            {
                if (_noteLinked.Length < noteEvent.NoteNumber) return;
                if (!_noteLinked[noteEvent.NoteNumber]) return;
                SetNoteState(midiOut, noteEvent.NoteNumber, _noteState[noteEvent.NoteNumber]);
                _noteState[noteEvent.NoteNumber] = !_noteState[noteEvent.NoteNumber];
            };

            midiIn.NoteOff += (noteEvent) =>
            {
                if (noteEvent.NoteNumber == 0)
                {
                    ResetFeedback();
                }
            };

            midiIn.ControlChange += (controlChangeEvent) =>
            {
                int index = (int)controlChangeEvent.Controller - 48; // APCmini offset, first slider

                var mapped2 = Extensions.Map(controlChangeEvent.ControllerValue, 0f, 127f, 0f, 1f);
                Blending[index] = mapped2;

                /*if ((int)controlChangeEvent.Controller == 48)
                {
                    // 0 - 127
                    var mapped = Map(controlChangeEvent.ControllerValue, 0, 127, 0, 64);
                    SendNoteOn(0, 7, mapped, 127, 0);

                    var mapped2 = Map(controlChangeEvent.ControllerValue, 0f, 127f, 0f, 1f);
                    Blend1 = mapped2;
                }
                else if ((int)controlChangeEvent.Controller == 49)
                {
                    // 0 - 127
                    //var mapped = Map(controlChangeEvent.ControllerValue, 0, 127, 0, 64);
                    //SendNoteOn(0, 7, mapped, 127, 0);

                    var mapped2 = Map(controlChangeEvent.ControllerValue, 0f, 127f, 0f, 1f);
                    Blend2 = mapped2;
                }*/
            };

            ResetFeedback();

            //timeCodeThread = new Thread(new ThreadStart(timeCodeGenerator.Start));
            //timeCodeThread.Start();
        }

        public void Stop()
        {
            midiIn.Stop();
            midiOut.Stop();
        }
    }
}