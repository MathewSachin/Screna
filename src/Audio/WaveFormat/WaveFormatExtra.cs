using System.IO;

namespace Screna.Audio
{
    /// <summary>
    /// Represents a Wave file format with Extra data.
    /// </summary>
    public class WaveFormatExtra : WaveFormat
    {
        readonly byte[] _extraData;

        /// <summary>
        /// Creates a new instance of <see cref="WaveFormatExtra"/>.
        /// </summary>
        public WaveFormatExtra(int SampleRate, int BitsPerSample, int Channels, byte[] ExtraData)
            : base(SampleRate, BitsPerSample, Channels)
        {
            _extraData = ExtraData;
            ExtraSize = ExtraData.Length;
        }

        /// <summary>
        /// Writes this object to a stream
        /// </summary>
        /// <param name="Writer">the output stream</param>
        public override void Serialize(BinaryWriter Writer)
        {
            base.Serialize(Writer);

            Writer.Write(_extraData);
        }
    }
}
