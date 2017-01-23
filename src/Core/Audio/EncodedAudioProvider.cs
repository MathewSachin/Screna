using System;

namespace Screna.Audio
{
    /// <summary>
    /// Wraps an <see cref="IAudioEncoder"/> over an <see cref="IAudioProvider"/>.
    /// </summary>
    public class EncodedAudioProvider : IAudioProvider
    {
        readonly IAudioProvider _audioProvider;
        readonly IAudioEncoder _audioEncoder;
        byte[] _encodedBuffer;

        /// <summary>
        /// Creates a new instance of <see cref="EncodedAudioProvider"/>.
        /// </summary>
        /// <param name="AudioProvider">The <see cref="IAudioProvider"/> to wrap.</param>
        /// <param name="AudioEncoder">The <see cref="IAudioEncoder"/> to use.</param>
        public EncodedAudioProvider(IAudioProvider AudioProvider, IAudioEncoder AudioEncoder)
        {
            if (AudioProvider == null)
                throw new ArgumentNullException(nameof(AudioProvider));

            if (AudioEncoder == null)
                throw new ArgumentNullException(nameof(AudioEncoder));
            
            _audioProvider = AudioProvider;
            _audioEncoder = AudioEncoder;
            
            WaveFormat = AudioEncoder.WaveFormat;

            AudioProvider.RecordingStopped += (Sender, Args) => RecordingStopped?.Invoke(Sender, Args);

            AudioProvider.DataAvailable += AudioProviderOnDataAvailable;
        }

        void AudioProviderOnDataAvailable(object Sender, DataAvailableEventArgs DataAvailableEventArgs)
        {
            _audioEncoder.EnsureBufferIsSufficient(ref _encodedBuffer, DataAvailableEventArgs.Length);

            var encodedLength = _audioEncoder.Encode(DataAvailableEventArgs.Buffer, 0, DataAvailableEventArgs.Length, _encodedBuffer, 0);

            DataAvailable?.Invoke(this, new DataAvailableEventArgs(_encodedBuffer, encodedLength));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _audioProvider.Dispose();
            _audioEncoder.Dispose();
        }

        /// <summary>
        /// Gets the Recording WaveFormat.
        /// </summary>
        public WaveFormat WaveFormat { get; }

        /// <summary>
        /// Start Recording.
        /// </summary>
        public void Start() => _audioProvider.Start();

        /// <summary>
        /// Stop Recording.
        /// </summary>
        public void Stop() => _audioProvider.Stop();

        /// <summary>
        /// Whether this <see cref="IAudioProvider"/> can be synchronized with video.
        /// </summary>
        public bool IsSynchronizable { get; }

        /// <summary>
        /// Indicates recorded data is available.
        /// </summary>
        public event EventHandler<DataAvailableEventArgs> DataAvailable;

        /// <summary>
        /// Indicates that all recorded data has now been received.
        /// </summary>
        public event EventHandler<EndEventArgs> RecordingStopped;
    }
}