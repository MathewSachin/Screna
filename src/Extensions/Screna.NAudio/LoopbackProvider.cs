using System;
using System.Collections.Generic;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using Screna.Audio;
using WaveFormat = Screna.Audio.WaveFormat;

namespace Screna.NAudio
{
    /// <summary>
    /// Provides Audio from Wasapi Loopback.
    /// </summary>
    public class LoopbackProvider : IAudioProvider
    {
        readonly WasapiOut _silenceOut;
        readonly WasapiLoopbackCapture _capture;
        static readonly MMDeviceEnumerator DeviceEnumerator = new MMDeviceEnumerator();

        /// <summary>
        /// Gets the Default Wasapi Loopback Device.
        /// </summary>
        public static MMDevice DefaultDevice => DeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

        /// <summary>
        /// Enumerates Wasapi Loopback Devices.
        /// </summary>
        public static IEnumerable<MMDevice> EnumerateDevices() => DeviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

        /// <summary>
        /// Create a new instance of <see cref="LoopbackProvider"/> using <see cref="DefaultDevice"/>.
        /// </summary>
        /// <param name="IncludeSilence">Whether to record silence?... default = true</param>
        public LoopbackProvider(bool IncludeSilence = true)
            : this(DefaultDevice, IncludeSilence) { }

        /// <summary>
        /// Create a new instance of <see cref="LoopbackProvider"/>.
        /// </summary>
        /// <param name="Device"><see cref="MMDevice"/> to use.</param>
        /// <param name="IncludeSilence">Whether to record silence?... default = true</param>
        public LoopbackProvider(MMDevice Device, bool IncludeSilence = true)
        {
            _capture = new WasapiLoopbackCapture(Device);

            _capture.DataAvailable += (Sender, Args) => DataAvailable?.Invoke(this, new DataAvailableEventArgs(Args.Buffer, Args.BytesRecorded));
            _capture.RecordingStopped += (Sender, Args) => RecordingStopped?.Invoke(this, new EndEventArgs(Args.Exception));

            var mixFormat = _capture.WaveFormat;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(mixFormat.SampleRate, mixFormat.Channels);

            if (!IncludeSilence)
                return;

            _silenceOut = new WasapiOut(Device, AudioClientShareMode.Shared, false, 100);
            _silenceOut.Init(new SilenceProvider(new global::NAudio.Wave.WaveFormat()));
        }

        /// <summary>
        /// Gets the Wasapi MixFormat used for Loopback.
        /// </summary>
        public WaveFormat WaveFormat { get; }

        /// <summary>
        /// Start Recording.
        /// </summary>
        public void Start()
        {
            _silenceOut.Play();
            _capture.StartRecording();
        }

        /// <summary>
        /// Stop Recording.
        /// </summary>
        public void Stop()
        {
            _capture.StopRecording();
            _silenceOut.Stop();
        }
        
        /// <summary>
        /// Indicates recorded data is available.
        /// </summary>
        public event EventHandler<DataAvailableEventArgs> DataAvailable;

        /// <summary>
        /// Indicates that all recorded data has now been received.
        /// </summary>
        public event EventHandler<EndEventArgs> RecordingStopped;

        /// <summary>
        /// Frees up the resources used by this instant.
        /// </summary>
        public void Dispose()
        {
            _capture?.Dispose();
            _silenceOut?.Dispose();
        }
    }
}
