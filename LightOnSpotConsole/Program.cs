using System.Diagnostics;
using LightOnSpotCore;
using LightOnSpotCore.MIDI;
using Unity_DMX.Core;

namespace LightOnSpotConsole
{
    internal class Program
    {
        private static UnityDmxWrapper dmxWrapper = new UnityDmxWrapper();
        private static MidiExperiment? midiExperiment;

        private static Thread? droneThread;

        private static void Main(string[] args)
        {
            midiExperiment = new MidiExperiment();

            DroneExample.Start();

            dmxWrapper.Start();
            dmxWrapper.Input.OnDmxPacketReceived += OnDmxPacketReceived;

            Console.Title = "yeet";

            // data test
            droneThread = new Thread(Update);
            droneThread.IsBackground = true;
            droneThread.Start();

            Console.ReadLine();
        }

        private static void OnDmxPacketReceived(short universe, DmxData universeBuffer)
        {
            DroneExample.UpdateDrones(ref universeBuffer);

            dmxWrapper.Input.ForceBufferUpdate();
        }

        private static void Update()
        {
            var capacity = 512 * 23;
            dmxWrapper.DmxBuffer.Buffer.EnsureCapacity(capacity);

            Stopwatch stopwatch = new();
            stopwatch.Start();

            while (true)
            {
                stopwatch.Stop();
                Time.deltaTime = ((double)stopwatch.ElapsedTicks) / 10000000;
                stopwatch.Start();

                DroneExample.UpdateDrones(ref dmxWrapper.DmxBuffer.Buffer);

                dmxWrapper.Input.ForceBufferUpdate();

                Thread.Sleep(1000 / 60);
            }
        }
    }
}