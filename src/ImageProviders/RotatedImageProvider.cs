using System;
using System.Drawing;

namespace Screna
{
    /// <summary>
    /// Wraps up another <see cref="IImageProvider"/> to provide Rotated and/or Flipped images.
    /// </summary>
    public class RotatedImageProvider : IImageProvider
    {
        readonly IImageProvider _sourceImageProvider;
        readonly RotateFlipType _rotateFlipType;

        /// <summary>
        /// Creates a new instance of <see cref="RotatedImageProvider"/>.
        /// </summary>
        /// <param name="Source">The source <see cref="IImageProvider"/>.</param>
        /// <param name="RotateFlipType">Rotation and/or Flipping options.</param>
        /// <exception cref="ArgumentNullException"><paramref name="Source"/> is null.</exception>
        public RotatedImageProvider(IImageProvider Source, RotateFlipType RotateFlipType)
        {
            _sourceImageProvider = Source ?? throw new ArgumentNullException(nameof(Source));
            _rotateFlipType = RotateFlipType;

            var flipDimensions = (int)RotateFlipType % 2 != 0;

            Width = flipDimensions ? Source.Height : Source.Width;

            Height = flipDimensions ? Source.Width : Source.Height;
        }
        
        /// <summary>
        /// Capture an image.
        /// </summary>
        public Bitmap Capture()
        {
            var img = _sourceImageProvider.Capture();

            img.RotateFlip(_rotateFlipType);

            return img;
        }

        /// <summary>
        /// Height of Captured image.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Width of Captured image.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// The Source <see cref="IImageProvider"/> is not freed here.
        /// </summary>
        public void Dispose() { }
    }
}
