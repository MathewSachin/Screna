// Adapted from SharpAvi Screencast Sample by Vasilli Masillov
using Screna.Audio;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace Screna
{
    /// <summary>
    /// Primary implementation of the <see cref="IRecorder"/> interface.
    /// </summary>
    public class Recorder : RecorderBase
    {
        #region Fields
        readonly IAudioProvider _audioProvider;
        readonly IVideoFileWriter _videoEncoder;
        readonly IImageProvider _imageProvider;

        Thread _recordThread;

        readonly ManualResetEvent _stopCapturing = new ManualResetEvent(false),
            _continueCapturing = new ManualResetEvent(false);

        readonly AutoResetEvent _videoFrameWritten = new AutoResetEvent(false),
            _audioBlockWritten = new AutoResetEvent(false);
        #endregion
        
        /// <summary>
        /// Creates a new instance of <see cref="Recorder"/>.
        /// </summary>
        /// <param name="Writer">Video File Writer.</param>
        /// <param name="ImageProvider">Image Provider which provides individual frames.</param>
        /// <param name="FrameRate">Video frame rate.</param>
        /// <param name="AudioProvider">Audio Provider which provides audio data.</param>
        /// <exception cref="ArgumentNullException"><paramref name="Writer"/> or <paramref name="ImageProvider"/> is null. Use <see cref="AudioRecorder"/> if you want to record audio only.</exception>
        public Recorder(IVideoFileWriter Writer, IImageProvider ImageProvider, int FrameRate, IAudioProvider AudioProvider = null)
        {
            if (Writer == null)
                throw new ArgumentNullException(nameof(Writer));

            if (ImageProvider == null)
                throw new ArgumentNullException(nameof(ImageProvider), 
                    AudioProvider == null ? $"Use {nameof(AudioRecorder)} if you want to record audio only" 
                                          : "Argument Null");

            // Init Fields
            _imageProvider = ImageProvider;
            _videoEncoder = Writer;
            _audioProvider = AudioProvider;
            
            Writer.Init(ImageProvider, FrameRate, AudioProvider);

            // Audio Init
            if (_videoEncoder.SupportsAudio
                && AudioProvider != null)
                AudioProvider.DataAvailable += AudioDataAvailable;
            else _audioProvider = null;

            // RecordThread Init
            _recordThread = new Thread(Record)
            {
                Name = "Captura.Record",
                IsBackground = true
            };

            // Not Actually Started, Waits for ContinueThread to be Set
            _recordThread?.Start();
        }

        /// <summary>
        /// Override this method with the code to start recording.
        /// </summary>
        protected override void OnStart()
        {
            if (_recordThread != null)
                _continueCapturing.Set();

            if (_audioProvider == null)
                return;

            _videoFrameWritten.Set();
            _audioBlockWritten.Reset();

            _audioProvider.Start();
        }

        /// <summary>
        /// Override this method with the code to pause recording.
        /// </summary>
        protected override void OnPause()
        {
            if (_recordThread != null)
                _continueCapturing.Reset();

            _audioProvider?.Stop();
        }

        /// <summary>
        /// Override this method with the code to stop recording.
        /// </summary>
        protected override void OnStop()
        {
            // Resume if Paused
            _continueCapturing?.Set();

            // Video
            if (_recordThread != null)
            {
                if (_stopCapturing != null
                    && !_stopCapturing.SafeWaitHandle.IsClosed)
                    _stopCapturing.Set();

                if (!_recordThread.Join(500))
                    _recordThread.Abort();

                _recordThread = null;
            }

            _imageProvider?.Dispose();

            _audioProvider?.Dispose();

            // WaitHandles
            if (_stopCapturing != null
                && !_stopCapturing.SafeWaitHandle.IsClosed)
                _stopCapturing.Dispose();

            if (_continueCapturing != null
                && !_continueCapturing.SafeWaitHandle.IsClosed)
                _continueCapturing.Dispose();

            _videoEncoder?.Dispose();
        }
        
        void Record()
        {
            try
            {
                var frameInterval = TimeSpan.FromSeconds(1 / (double)_videoEncoder.FrameRate);
                Task frameWriteTask = null;
                var timeTillNextFrame = TimeSpan.Zero;

                while (!_stopCapturing.WaitOne(timeTillNextFrame)
                    && _continueCapturing.WaitOne())
                {
                    var timestamp = DateTime.Now;

                    Bitmap frame = null;

                    while (true)
                    {
                        try
                        {
                            frame = _imageProvider.Capture();
                            break;
                        }
                        catch
                        {
                            // Try until we get a frame.
                            
                            if (!_stopCapturing.WaitOne(1))
                                break;
                        }
                    }

                    if (frame == null)
                        continue;

                    // Wait for the previous frame is written
                    if (frameWriteTask != null)
                    {
                        frameWriteTask.Wait();
                        _videoFrameWritten.Set();
                    }

                    if (_audioProvider != null
                        && _audioProvider.IsSynchronizable)
                        if (WaitHandle.WaitAny(new WaitHandle[] { _audioBlockWritten, _stopCapturing }) == 1)
                            break;

                    // Start asynchronous (encoding and) writing of the new frame
                    frameWriteTask = _videoEncoder.WriteFrameAsync(frame);

                    timeTillNextFrame = timestamp + frameInterval - DateTime.Now;
                    if (timeTillNextFrame < TimeSpan.Zero)
                        timeTillNextFrame = TimeSpan.Zero;
                }

                // Wait for the last frame is written
                frameWriteTask?.Wait();
            }
            catch (Exception e)
            {
                Stop();

                RaiseRecordingStopped(e);
            }
        }

        void AudioDataAvailable(object sender, DataAvailableEventArgs e)
        {
            try
            {
                if (_audioProvider.IsSynchronizable)
                {
                    if (WaitHandle.WaitAny(new WaitHandle[] { _videoFrameWritten, _stopCapturing }) != 0)
                        return;

                    _videoEncoder.WriteAudio(e.Buffer, e.Length);

                    _audioBlockWritten.Set();
                }
                else _videoEncoder.WriteAudio(e.Buffer, e.Length);
            }
            catch { }
        }
    }
}
