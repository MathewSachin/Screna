using System;

namespace Screna.Audio
{
    /// <summary>
    /// An <see cref="IRecorder"/> for recording only Audio.
    /// </summary>
    public class AudioRecorder : IRecorder
    {
        readonly IAudioFileWriter _writer;
        readonly IAudioProvider _audioProvider;
        
        /// <summary>
        /// Creates a new instance of <see cref="AudioRecorder"/>.
        /// </summary>
        /// <param name="Provider">The Audio Source.</param>
        /// <param name="Writer">The <see cref="IAudioFileWriter"/> to write audio to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="Provider"/> or <paramref name="Writer"/> is null.</exception>
        public AudioRecorder(IAudioProvider Provider, IAudioFileWriter Writer)
        {
            _audioProvider = Provider ?? throw new ArgumentNullException(nameof(Provider));
            _writer = Writer ?? throw new ArgumentNullException(nameof(Writer));
            
            _audioProvider.DataAvailable += (s, e) => _writer.Write(e.Buffer, 0, e.Length);
        }

        /// <summary>
        /// Start Recording.
        /// </summary>
        public void Start() => _audioProvider.Start();

        /// <summary>
        /// Stop Recording.
        /// </summary>
        public void Stop()
        {
            _audioProvider?.Stop();
        }

        /// <summary>
        /// Frees all resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            _audioProvider?.Dispose();
            _writer?.Dispose();
        }
    }
}
