using Screna.Audio;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SharpAvi.Codecs;
using SharpAvi.Output;
using AviInternalWriter = SharpAvi.Output.AviWriter;
using IAudioEncoder = Screna.Audio.IAudioEncoder;

namespace Screna.Avi
{
    /// <summary>
    /// Writes an AVI file.
    /// </summary>
    public class AviWriter : IVideoFileWriter
    {
        #region Fields
        AviInternalWriter _writer;
        IAviVideoStream _videoStream;
        IAviAudioStream _audioStream;
        byte[] _videoBuffer;
        readonly string _fileName;
        readonly AviCodec _codec;
        readonly object _syncLock = new object();

        /// <summary>
        /// Video Frame Rate.
        /// </summary>
        public int FrameRate { get; private set; }
        
        /// <summary>
        /// Gets whether Audio is recorded.
        /// </summary>
        public bool SupportsAudio => _audioStream != null;
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="AviWriter"/>.
        /// </summary>
        /// <param name="FileName">Output file path.</param>
        /// <param name="Codec">The Avi Codec.</param>
        public AviWriter(string FileName, AviCodec Codec)
        {
            _fileName = FileName;
            _codec = Codec;
        }

        /// <summary>
        /// Initialises the <see cref="IVideoFileWriter"/>. Usually called by an <see cref="IRecorder"/>.
        /// </summary>
        /// <param name="ImageProvider">The Image Provider.</param>
        /// <param name="FrameRate">Video Frame Rate.</param>
        /// <param name="AudioProvider">The Audio Provider.</param>
        public void Init(IImageProvider ImageProvider, int FrameRate, IAudioProvider AudioProvider)
        {
            this.FrameRate = FrameRate;
            _videoBuffer = new byte[ImageProvider.Width * ImageProvider.Height * 4];

            _writer = new AviInternalWriter(_fileName)
            {
                FramesPerSecond = FrameRate,
                EmitIndex1 = true
            };

            CreateVideoStream(ImageProvider.Width, ImageProvider.Height);

            if (AudioProvider != null)
                CreateAudioStream(AudioProvider);
        }

        /// <summary>
        /// Asynchronously writes an Image frame.
        /// </summary>
        /// <param name="Image">The Image frame to write.</param>
        /// <returns>The Task Object.</returns>
        public Task WriteFrameAsync(Bitmap Image)
        {
            return Task.Run(() =>
            {
                var bits = Image.LockBits(new Rectangle(Point.Empty, Image.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                Marshal.Copy(bits.Scan0, _videoBuffer, 0, _videoBuffer.Length);
                Image.UnlockBits(bits);

                Image.Dispose();

                lock (_syncLock)
                    _videoStream.WriteFrameAsync(true, _videoBuffer, 0, _videoBuffer.Length).Wait();
            });
        }

        void CreateVideoStream(int Width, int Height)
        {
            // Select encoder type based on FOURCC of codec
            if (_codec == AviCodec.Uncompressed)
                _videoStream = _writer.AddUncompressedVideoStream(Width, Height);
            else if (_codec == AviCodec.MotionJpeg)
                _videoStream = _writer.AddMotionJpegVideoStream(Width, Height, _codec.Quality);
            else
            {
                _videoStream = _writer.AddMpeg4VideoStream(Width, Height,
                    (double)_writer.FramesPerSecond,
                    // It seems that all tested MPEG-4 VfW codecs ignore the quality affecting parameters passed through VfW API
                    // They only respect the settings from their own configuration dialogs, and Mpeg4VideoEncoder currently has no support for this
                    0,
                    _codec.Quality,
                    // Most of VfW codecs expect single-threaded use, so we wrap this encoder to special wrapper
                    // Thus all calls to the encoder (including its instantiation) will be invoked on a single thread although encoding (and writing) is performed asynchronously
                    _codec.FourCC,
                    true);
            }

            _videoStream.Name = "ScrenaVideo";
        }

        void CreateAudioStream(IAudioProvider AudioProvider)
        {
            _audioStream = _writer.AddEncodingAudioStream(new IAudioProviderWrapper(AudioProvider));

            _audioStream.Name = "ScrenaAudio";
        }

        /// <summary>
        /// Enumerates all available Avi Encoders.
        /// </summary>
        public static IEnumerable<AviCodec> EnumerateEncoders()
        {
            yield return AviCodec.Uncompressed;
            yield return AviCodec.MotionJpeg;
            foreach (var codec in Mpeg4VideoEncoderVcm.GetAvailableCodecs())
                yield return new AviCodec(codec.Codec, codec.Name);
        }

        /// <summary>
        /// Write audio block to Audio Stream.
        /// </summary>
        /// <param name="Buffer">Buffer containing audio data.</param>
        /// <param name="Length">Length of audio data in bytes.</param>
        public void WriteAudio(byte[] Buffer, int Length)
        {
            lock (_syncLock)
                _audioStream?.WriteBlock(Buffer, 0, Length);
        }

        /// <summary>
        /// Frees all resources used by this object.
        /// </summary>
        public void Dispose()
        {
            lock (_syncLock)
                _writer?.Close();
        }
    }
}
