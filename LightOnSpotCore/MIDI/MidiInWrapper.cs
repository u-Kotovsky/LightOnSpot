using Microsoft.Extensions.Logging;
using NAudio;
using NAudio.Midi;

namespace LightOnSpotCore.MIDI
{
    public class MidiInWrapper
    {
        private MidiIn _device;
        private MidiInCapabilities _deviceInfo;

        public Action<NoteEvent> NoteOn = delegate { };
        public Action<NoteEvent> NoteOff = delegate { };
        public Action<ControlChangeEvent> ControlChange = delegate { };

        private int selectedDevice = 0;

        private ILoggerFactory _factory;
        private ILogger _logger;

        public MidiInWrapper(string deviceName)
        {
            SetupLogging();
            Start(deviceName);
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
            _logger = _factory.CreateLogger(nameof(MidiInWrapper));
        }

        ~MidiInWrapper()
        {
            _logger.LogInformation("Destroy");
        }

        /// <summary>
        /// Start midi input
        /// </summary>
        /// <param name="deviceName"></param>
        public void Start(string deviceName)
        {
            SelectDevice(deviceName, true);
            _device.MessageReceived += MidiIn_MessageReceived;
            _device.ErrorReceived += MidiIn_ErrorReceived;
            _device.Start();
            _logger.LogInformation("Started '{0}'", _deviceInfo.ProductName);
        }

        /// <summary>
        /// Stop midi input
        /// </summary>
        public void Stop()
        {
            _device.MessageReceived -= MidiIn_MessageReceived;
            _device.ErrorReceived -= MidiIn_ErrorReceived;
            _device.Stop();
            _device.Dispose();
            _logger.LogInformation("Stopped '{0}'", _deviceInfo.ProductName);
        }

        /// <summary>
        /// Set device by name
        /// </summary>
        /// <param name="name"></param>
        public void SelectDevice(string name, bool exact = false)
        {
            var deviceIndex = 0;
            var midiInDevices = new string[MidiIn.NumberOfDevices];

            for (int i = 0; i < midiInDevices.Length; i++)
            {
                var device = MidiIn.DeviceInfo(i);

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
        /// Set device by index
        /// </summary>
        /// <param name="index"></param>
        public bool ApplyDevice(int index)
        {
            MidiInCapabilities midiInfo;

            try
            {
                midiInfo = MidiIn.DeviceInfo(index);
            }
            catch (MmException e)
            {
                _logger.LogError("Failed to get info of midiIn with index: '{0}': {1}", index, e);
                return false;
            }

            try
            {
                _device = new MidiIn(index);
            } 
            catch (MmException e)
            {
                _logger.LogError("Failed to open midiIn: '{0}': {1}", midiInfo.ProductName, e);
                return false;
            }

            _deviceInfo = midiInfo;
            //_logger.LogDebug("Device set to: ({0}) '{1}'", index, midiInfo.ProductName);

            return true;
        }

        /// <summary>
        /// Load devices and show them
        /// </summary>
        private void LoadDevices()
        {
            var midiInDevices = new string[MidiIn.NumberOfDevices];

            for (int i = 0; i < midiInDevices.Length; i++)
            {
                var device = MidiIn.DeviceInfo(i);
                midiInDevices[i] = device.ProductName;
            }

            _logger.LogDebug($"Devices: {string.Join(", ", midiInDevices)}");
        }

        private void MidiIn_ErrorReceived(object? sender, MidiInMessageEventArgs e)
        {
            _logger.LogError(string.Format("Time {0} Message 0x{1:X8} Event {2}", e.Timestamp, e.RawMessage, e.MidiEvent));
        }

        private void MidiIn_MessageReceived(object? sender, MidiInMessageEventArgs e)
        {
            if (e.MidiEvent is NoteEvent noteEvent)
            {
                int noteIndex = noteEvent.NoteNumber;

                switch (e.MidiEvent.CommandCode)
                {
                    case MidiCommandCode.NoteOn:
                        NoteOn?.Invoke(noteEvent);
                        break;

                    case MidiCommandCode.NoteOff:
                        NoteOff?.Invoke(noteEvent);
                        break;
                }
            }
            else if (e.MidiEvent is ControlChangeEvent controlChangeEvent)
            {
                ControlChange?.Invoke(controlChangeEvent);
            }/*
            else if (e.MidiEvent is SmpteOffsetEvent smpteOffsetEvent)
            {
                Console.WriteLine($"SmpteOffsetEvent: {smpteOffsetEvent.Hours}:{smpteOffsetEvent.Minutes}:{smpteOffsetEvent.Seconds} {smpteOffsetEvent.Frames}:{smpteOffsetEvent.SubFrames}");
            }
            else if (e.MidiEvent is PitchWheelChangeEvent pitchWheelChangeEvent)
            {
                // Ignore?
            }*/
            else
            {
                _logger.LogDebug(e.RawMessage, e.MidiEvent.ToString(), e);
            }
        }
    }
}
