using Microsoft.Extensions.Logging;
using NAudio;
using NAudio.Midi;

namespace LightOnSpotCore.MIDI
{
    public class MidiOutWrapper
    {
        private MidiOut _device;
        private MidiOutCapabilities _deviceInfo;

        private int selectedDevice = 0;

        private ILoggerFactory _factory;
        private ILogger _logger;

        public MidiOutWrapper(string deviceName)
        {
            SetupLogging();
            Start(deviceName);
        }

        ~MidiOutWrapper()
        {
            _logger.LogInformation("Destroy");
        }

        /// <summary>
        /// Create logging factory and logger
        /// </summary>
        private void SetupLogging()
        {
            _factory = LoggerFactory.Create(builder =>
            {
                builder.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.SingleLine = true;
                    options.TimestampFormat = "HH:mm:ss ";
                });
                builder.SetMinimumLevel(LogLevel.Debug);
            });
            _logger = _factory.CreateLogger(nameof(MidiOutWrapper));
        }

        /// <summary>
        /// Start midi output
        /// </summary>
        /// <param name="deviceName"></param>
        /// <exception cref="Exception"></exception>
        public void Start(string deviceName)
        {
            SelectDevice(deviceName, true);
            if (_device == null) throw new Exception("Device was not initialized");
            _logger.LogInformation("Started with device '{0}'", _deviceInfo.ProductName);
        }

        /// <summary>
        /// Stop midi output
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Stop()
        {
            if (_device == null) throw new Exception("Device was not initialized");
            _device.Dispose();
            _logger.LogInformation("Stopped");
        }

        /// <summary>
        /// Set device by name
        /// </summary>
        /// <param name="name"></param>
        public void SelectDevice(string name, bool exact = false)
        {
            var deviceIndex = 0;
            var midiInDevices = new string[MidiOut.NumberOfDevices];

            for (int i = 0; i < midiInDevices.Length; i++)
            {
                var device = MidiOut.DeviceInfo(i);

                if (!exact && device.ProductName.Contains(name) ||
                    exact && device.ProductName == name)
                {
                    deviceIndex = i;
                }
            }

            selectedDevice = deviceIndex;
            ApplyDevice(selectedDevice);
        }

        /// <summary>
        /// Apply device by index
        /// </summary>
        /// <param name="index"></param>
        public bool ApplyDevice(int index)
        {
            MidiOutCapabilities midiInfo;

            try
            {
                midiInfo = MidiOut.DeviceInfo(index);
            }
            catch (MmException e)
            {
                _logger.LogError("Failed to get info of midiOut with index: '{0}': {1}", index, e);
                return false;
            }

            try
            {
                _device = new MidiOut(index);
            }
            catch (MmException e)
            {
                _logger.LogError("Failed to open midiOut: '{0}': {1}", midiInfo.ProductName, e);
                return false;
            }

            _deviceInfo = midiInfo;
            //_logger.LogDebug("Device set to: ({0}) '{1}'", index, midiInfo.ProductName);

            return true;
        }

        /// <summary>
        /// Load and show device list
        /// </summary>
        private void LoadDevices()
        {
            var midiOutDevices = new string[MidiOut.NumberOfDevices];

            for (int i = 0; i < midiOutDevices.Length; i++)
            {
                var device = MidiOut.DeviceInfo(i);
                midiOutDevices[i] = device.ProductName;
            }

            _logger.LogInformation($"Devices: {string.Join(", ", midiOutDevices)}");
        }

        /// <summary>
        /// Send midi sysex message
        /// </summary>
        /// <param name="message"></param>
        public void SendSysex(byte[] message)
        {
            _device.SendBuffer([0xF0, 0x00, 0x21, 0x1D, 0x01, 0x01]);
            _device.SendBuffer(message);
            _device.SendBuffer([0xF7]);
        }

        /// <summary>
        /// Send MIDI signal NoteOn
        /// </summary>
        /// <param name="absoluteTime"></param>
        /// <param name="channel">1-16</param>
        /// <param name="noteNumber">0-63</param>
        /// <param name="velocity">0-127</param>
        /// <param name="duration"></param>
        public void SendNoteOn(int absoluteTime, byte channel, int noteNumber, byte velocity, int duration)
        {
            var noteOnEvent = new NoteOnEvent(absoluteTime, channel, noteNumber, velocity, duration);
            int shortMessage = noteOnEvent.GetAsShortMessage();
            _device.Send(shortMessage);
        }

        /// <summary>
        /// Send midi timecode
        /// </summary>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        /// <param name="frames"></param>
        /// <param name="subFrames"></param>
        public void Send(byte hours, byte minutes, byte seconds, byte frames, byte subFrames)
        {
            var smpteOffsetEvent = new SmpteOffsetEvent(hours, minutes, seconds, frames, subFrames);
            int shortMessage = smpteOffsetEvent.GetAsShortMessage();
            _device.Send(shortMessage);
        }

        /// <summary>
        /// Send midi timecode
        /// </summary>
        /// <param name="timeSpan"></param>
        public void Send(TimeSpan timeSpan)
        {
            Send((byte)timeSpan.Hours, (byte)timeSpan.Minutes, (byte)timeSpan.Seconds, 
                (byte)timeSpan.Milliseconds, (byte)timeSpan.Nanoseconds);
        }
    }
}