using System;
using System.Drawing;

namespace Screna
{
    /// <summary>
    /// Wraps up another <see cref="IImageProvider"/> to provide Cropped images.
    /// </summary>
    public class CroppedImageProvider : IImageProvider
    {
        readonly IImageProvider _sourceImageProvider;
        readonly Rectangle _cropRectangle;

        /// <summary>
        /// Creates a new instance of <see cref="CroppedImageProvider"/>.
        /// </summary>
        /// <param name="Source">The Source <see cref="IImageProvider"/>.</param>
        /// <param name="CropRectangle">The <see cref="Rectangle"/> used to determine the region to crop.</param>
        /// <exception cref="ArgumentNullException"><paramref name="Source"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="CropRectangle"/> extends boutside the images returned by <paramref name="Source"/>.</exception>
        public CroppedImageProvider(IImageProvider Source, Rectangle CropRectangle)
        {
            if (Source == null)
                throw new ArgumentNullException(nameof(Source));
            
            if (!new Rectangle(0, 0, Source.Width, Source.Height).Contains(CropRectangle))
                throw new ArgumentException("The Rectangle extends outside the image.");
            
            _sourceImageProvider = Source;
            _cropRectangle = CropRectangle;

            Width = CropRectangle.Width;
            Height = CropRectangle.Height;
        }

        /// <summary>
        /// The Source <see cref="IImageProvider"/> is not freed here.
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Capture an image.
        /// </summary>
        public Bitmap Capture()
        {
            var bmp = _sourceImageProvider.Capture();

            return bmp.Clone(_cropRectangle, bmp.PixelFormat);
        }

        /// <summary>
        /// Height of Captured image.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Width of Captured image.
        /// </summary>
        public int Width { get; }
    }
}