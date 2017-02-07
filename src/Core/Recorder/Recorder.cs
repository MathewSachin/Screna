using Screna.Audio;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable 1591

namespace Screna
{
    public class Recorder : IRecorder
    {
        #region Fields
        IAudioProvider _audioProvider;
        IVideoFileWriter _videoWriter;
        IImageProvider _imageProvider;

        int _frameRate;
        
        BlockingCollection<object> _frames = new BlockingCollection<object>();
        
        ManualResetEvent _continueCapturing = new ManualResetEvent(false);

        Task _writeTask, _recordTask;
        #endregion

        public Recorder(IVideoFileWriter VideoWriter, IImageProvider ImageProvider, int FrameRate, IAudioProvider AudioProvider)
        {
            _videoWriter = VideoWriter;
            _imageProvider = ImageProvider;
            _audioProvider = AudioProvider;

            _frameRate = FrameRate;
            
            if (VideoWriter.SupportsAudio && AudioProvider != null)
                AudioProvider.DataAvailable += AudioProvider_DataAvailable;
            else _audioProvider = null;

            _recordTask = Task.Factory.StartNew(DoRecord);
            _writeTask = Task.Factory.StartNew(DoWrite);
        }

        void DoWrite()
        {
            while (!_frames.IsCompleted)
            {
                _frames.TryTake(out var data);

                switch (data)
                {
                    case Bitmap img:
                        _videoWriter.WriteFrame(img);
                        break;

                    case DataAvailableEventArgs args:
                        _videoWriter.WriteAudio(args.Buffer, args.Length);
                        break;
                }   
            }
        }

        void DoRecord()
        {
            var frameInterval = TimeSpan.FromSeconds(1.0 / _frameRate);
            
            while (_continueCapturing.WaitOne() && !_frames.IsAddingCompleted)
            {
                var timestamp = DateTime.Now;

                try { _frames.Add(_imageProvider.Capture()); }
                catch { }
                                
                var timeTillNextFrame = timestamp + frameInterval - DateTime.Now;
                if (timeTillNextFrame < TimeSpan.Zero)
                    timeTillNextFrame = TimeSpan.Zero;

                Thread.Sleep(timeTillNextFrame);
            }
        }

        void AudioProvider_DataAvailable(object sender, DataAvailableEventArgs e)
        {
            try { _frames.Add(e); }
            catch { }
        }
        
        public void Dispose()
        {
            ThrowIfDisposed();

            _audioProvider?.Stop();
            _audioProvider?.Dispose();

            _frames.CompleteAdding();

            _continueCapturing.Set();

            _recordTask.Wait();
            _writeTask.Wait();

            _videoWriter.Dispose();
            _frames.Dispose();
            
            _continueCapturing.Close();

            _disposed = true;
        }

        bool _disposed;

        void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException("this");
        }

        public void Start()
        {
            ThrowIfDisposed();

            _audioProvider?.Start();
            _continueCapturing.Set();
        }

        public void Stop()
        {
            ThrowIfDisposed();

            _continueCapturing.Reset();
            _audioProvider?.Stop();
        }
    }
}
