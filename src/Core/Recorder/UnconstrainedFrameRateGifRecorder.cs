using System;
using System.Threading;

namespace Screna
{
    /// <summary>
    /// An <see cref="IRecorder"/> which records to a Gif using Delay for each frame instead of Frame Rate.
    /// </summary>
    public class UnconstrainedFrameRateGifRecorder : IRecorder
    {
        #region Fields
        readonly GifWriter _videoEncoder;
        readonly IImageProvider _imageProvider;

        readonly Thread _recordThread;

        readonly ManualResetEvent _stopCapturing = new ManualResetEvent(false),
            _continueCapturing = new ManualResetEvent(false);
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="UnconstrainedFrameRateGifRecorder"/>.
        /// </summary>
        /// <param name="Encoder">The <see cref="GifWriter"/> to write into.</param>
        /// <param name="ImageProvider">The <see cref="IImageProvider"/> providing the individual frames.</param>
        /// <exception cref="ArgumentNullException"><paramref name="Encoder"/> or <paramref name="ImageProvider"/> is null.</exception>
        public UnconstrainedFrameRateGifRecorder(GifWriter Encoder, IImageProvider ImageProvider)
        {
            // Init Fields
            _imageProvider = ImageProvider ?? throw new ArgumentNullException(nameof(ImageProvider));
            _videoEncoder = Encoder ?? throw new ArgumentNullException(nameof(Encoder));
            
            // GifWriter.Init not needed.

            // RecordThread Init
            _recordThread = new Thread(Record)
            {
                Name = "Captura.Record",
                IsBackground = true
            };


            // Not Actually Started, Waits for _continueCapturing to be Set
            _recordThread?.Start();
        }

        /// <summary>
        /// Start recording.
        /// </summary>
        public void Start() => _continueCapturing.Set();

        /// <summary>
        /// Frees resources.
        /// </summary>
        public void Dispose()
        {
            // Resume if Paused
            _continueCapturing?.Set();

            // Video
            _stopCapturing.Set();
            _recordThread.Join();    
            _imageProvider.Dispose();
                        
            _videoEncoder.Dispose();
        }

        /// <summary>
        /// Override this method with the code to pause recording.
        /// </summary>
        public void Stop() => _continueCapturing.Reset();

        void Record()
        {
            try
            {
                var lastFrameWriteTime = DateTime.MinValue;
                
                while (!_stopCapturing.WaitOne(0) && _continueCapturing.WaitOne())
                {
                    var frame = _imageProvider.Capture();

                    var delay = lastFrameWriteTime == DateTime.MinValue ? 0
                        : (int)(DateTime.Now - lastFrameWriteTime).TotalMilliseconds;

                    lastFrameWriteTime = DateTime.Now;

                    _videoEncoder.WriteFrame(frame, delay);
                }
            }
            catch { Dispose(); }
        }
    }
}
