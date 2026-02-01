using System.Diagnostics;
using NAudio.Midi;

namespace LightOnSpotCore.MIDI
{
    internal class MidiTimeCodeGenerator
    {
        private MidiOutWrapper midiOut;
        private Stopwatch stopwatch;
        private long lastUpdateFrameTime;
        private long time;
        private long deltaTime;

        public MidiTimeCodeGenerator(MidiOutWrapper midiOut)
        {
            this.midiOut = midiOut;
            stopwatch = new Stopwatch();
        }

        private bool isTimerRunning = false;
        public bool IsTimerRunning
        {
            get { return isTimerRunning; }
        }

        public Action<SmpteOffsetEvent> OnSmpteOffsetEvent = delegate { };

        public void Start()
        {
            if (isTimerRunning)
            {
                throw new Exception("Tried to start active timer.");
            }

            isTimerRunning = true;

            Update();
        }

        public void Stop()
        {
            if (!isTimerRunning)
            {
                throw new Exception("Tried to stop inactive timer.");
            }

            isTimerRunning = false;
        }

        private const int UpdateRate = 30; // 30 frames per second  so, 1 / 30 = millisecond or smth

        private SmpteOffsetEvent _smpteOffsetEvent;

        private void Update()
        {
            stopwatch.Start();

            lastUpdateFrameTime = 0;

            while (isTimerRunning)
            {
                deltaTime = stopwatch.ElapsedMilliseconds - time;
                time = stopwatch.ElapsedMilliseconds;

                if (lastUpdateFrameTime + (UpdateRate * 0.01) <= stopwatch.ElapsedMilliseconds)
                {
                    lastUpdateFrameTime = time;

                    _smpteOffsetEvent = Extensions.GetSmpteOffsetEvent(stopwatch.Elapsed);

                    // Send new timecode
                    midiOut.Send(stopwatch.Elapsed);
                    OnSmpteOffsetEvent?.Invoke(_smpteOffsetEvent);
                }
            }
        }
    }
}
