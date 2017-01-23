using System;
using System.Linq;
using System.Runtime.InteropServices;
using ManagedBass;
using ManagedBass.Wasapi;
using Screna.Audio;
using WaveFormat = Screna.Audio.WaveFormat;

namespace Screna.Bass
{
    /// <summary>
    /// Provides Audio from Wasapi Loopback.
    /// </summary>
    public class LoopbackProvider : IAudioProvider
    {
        readonly Silence _silencePlayer;
        readonly int _deviceIndex;
        readonly WasapiProcedure _proc;
        
        /// <summary>
        /// Create a new instance of <see cref="LoopbackProvider"/> using <see cref="WasapiLoopbackDevice.Default"/>.
        /// </summary>
        /// <param name="IncludeSilence">Whether to record silence?... default = true</param>
        public LoopbackProvider(bool IncludeSilence = true) 
            : this(WasapiLoopbackDevice.Default, IncludeSilence) { }

        /// <summary>
        /// Create a new instance of <see cref="LoopbackProvider"/>.
        /// </summary>
        /// <param name="Device"><see cref="WasapiLoopbackDevice"/> to use.</param>
        /// <param name="IncludeSilence">Whether to record silence?... default = true</param>
        public LoopbackProvider(WasapiLoopbackDevice Device, bool IncludeSilence = true)
        {
            _deviceIndex = Device.DeviceIndex;
            _proc = Procedure;
            
            if (IncludeSilence)
                _silencePlayer = new Silence(PlaybackDevice.Devices.First(Dev => Dev.Info.Driver == Device.Info.ID));

            BassWasapi.Init(_deviceIndex, Procedure: _proc);
            BassWasapi.CurrentDevice = Device.DeviceIndex;

            var info = BassWasapi.Info;

            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(info.Frequency, info.Channels);
        }
                
        /// <summary>
        /// Gets the Wasapi MixFormat used for Loopback.
        /// </summary>
        public WaveFormat WaveFormat { get; }

        /// <summary>
        /// Indicates recorded data is available.
        /// </summary>
        public event EventHandler<DataAvailableEventArgs> DataAvailable;

        /// <summary>
        /// Indicates that all recorded data has now been received.
        /// </summary>
        public event EventHandler<EndEventArgs> RecordingStopped;

        byte[] _buffer;

        int Procedure(IntPtr Buffer, int Length, IntPtr User)
        {
            if (_buffer == null || _buffer.Length < Length)
                _buffer = new byte[Length];

            Marshal.Copy(Buffer, _buffer, 0, Length);

            DataAvailable?.Invoke(this, new DataAvailableEventArgs(_buffer, Length));

            return Length;
        }

        /// <summary>
        /// Frees up the resources used by this instant.
        /// </summary>
        public void Dispose()
        {
            BassWasapi.CurrentDevice = _deviceIndex;
            BassWasapi.Free();
            _silencePlayer?.Dispose();
        }
        
        /// <summary>
        /// Start Recording.
        /// </summary>
        public void Start()
        {
            _silencePlayer?.Play();

            BassWasapi.CurrentDevice = _deviceIndex;

            if (!BassWasapi.Start())
                _silencePlayer?.Stop();
        }

        /// <summary>
        /// Stop Recording.
        /// </summary>
        public void Stop()
        {
            _silencePlayer?.Stop();

            BassWasapi.CurrentDevice = _deviceIndex;

            BassWasapi.Stop();

            RecordingStopped?.Invoke(this, new EndEventArgs(null));
        }
    }
}