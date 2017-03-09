using System;

namespace Screna.Audio
{
    /// <summary>
    /// Encodes Audio.
    /// </summary>
    public interface IAudioEncoder : IDisposable
    {
        /// <summary>
        /// Wave Format including Specific Data.
        /// </summary>
        WaveFormat WaveFormat { get; }

        /// <summary>
        /// Gets if RIFF header is needed when writing to a file.
        /// </summary>
        bool RequiresRiffHeader { get; }

        /// <summary>
        /// Gets the maximum number of bytes in encoded data for a given number of source bytes.
        /// </summary>
        /// <param name="SourceCount">Number of source bytes. Specify <c>0</c> for a flush buffer size.</param>
        /// <seealso cref="Encode"/>
        /// <seealso cref="Flush"/>
        int GetMaxEncodedLength(int SourceCount);

        /// <summary>
        /// Encodes block of audio data.
        /// </summary>
        /// <param name="Source">Buffer with audio data.</param>
        /// <param name="SourceOffset">Offset to start reading <paramref name="Source"/>.</param>
        /// <param name="SourceCount">Number of bytes to read from <paramref name="Source"/>.</param>
        /// <param name="Destination">Buffer for encoded audio data.</param>
        /// <param name="DestinationOffset">Offset to start writing to <paramref name="Destination"/>.</param>
        /// <returns>The number of bytes written to <paramref name="Destination"/>.</returns>
        /// <seealso cref="GetMaxEncodedLength"/>
        int Encode(byte[] Source, int SourceOffset, int SourceCount, byte[] Destination, int DestinationOffset);

        /// <summary>
        /// Flushes internal encoder buffers if any.
        /// </summary>
        /// <param name="Destination">Buffer for encoded audio data.</param>
        /// <param name="DestinationOffset">Offset to start writing to <paramref name="Destination"/>.</param>
        /// <returns>The number of bytes written to <paramref name="Destination"/>.</returns>
        /// <seealso cref="GetMaxEncodedLength"/>
        int Flush(byte[] Destination, int DestinationOffset);

        /// <summary>
        /// Ensures that the buffer is big enough to hold the result of encoding <paramref name="SourceCount"/> bytes.
        /// </summary>
        void EnsureBufferIsSufficient(ref byte[] Buffer, int SourceCount);
    }
}
