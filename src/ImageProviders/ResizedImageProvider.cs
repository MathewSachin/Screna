using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Screna
{
    /// <summary>
    /// Wraps up another <see cref="IImageProvider"/> to provide images resied to required dimensions.
    /// </summary>
    public class ResizedImageProvider : IImageProvider
    {
        readonly float _resizeWidth, _resizeHeight;

        readonly IImageProvider _imageSource;
        readonly Color _backgroundColor;

        /// <summary>
        /// Creates a new instance of <see cref="ResizedImageProvider"/>.
        /// </summary>
        /// <param name="ImageSource">The Source <see cref="IImageProvider"/>.</param>
        /// <param name="TargetWidth">Target Width.</param>
        /// <param name="TargetHeight">Target Height.</param>
        /// <param name="BackgroundColor">Background Color to fill any left space.</param>
        public ResizedImageProvider(IImageProvider ImageSource, int TargetWidth, int TargetHeight, Color BackgroundColor)
        {
            if (ImageSource == null)
                throw new ArgumentNullException(nameof(ImageSource));

            _imageSource = ImageSource;
            _backgroundColor = BackgroundColor;

            Height = TargetHeight;
            Width = TargetWidth;

            int originalWidth = ImageSource.Width,
                originalHeight = ImageSource.Height;

            var ratio = Math.Min((float)TargetWidth / originalWidth, (float)TargetHeight / originalHeight);

            _resizeWidth = originalWidth * ratio;
            _resizeHeight = originalHeight * ratio;
        }

        /// <summary>
        /// Capture an image.
        /// </summary>
        public Bitmap Capture()
        {
            var bmp = _imageSource.Capture();

            var resizedBmp = new Bitmap(Width, Height);

            using (var g = Graphics.FromImage(resizedBmp))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;

                if (_backgroundColor != Color.Transparent)
                    g.FillRectangle(new SolidBrush(_backgroundColor), 0, 0, Width, Height);

                g.DrawImage(bmp, 0, 0, _resizeWidth, _resizeHeight);
            }

            return resizedBmp;
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