using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace WebVella.BlazorTrace;
/// <summary>
/// Provides utility methods for compressing and decompressing strings,
/// particularly useful for JSON strings to reduce their size for storage or transmission.
/// </summary>
public static class CompressionUtility
{
    /// <summary>
    /// Compresses a given string using GZip compression and returns the compressed data as a Base64 string.
    /// This is useful for reducing the size of JSON strings before storing or sending them.
    /// </summary>
    /// <param name="jsonString">The original string (e.g., a JSON string) to compress.</param>
    /// <returns>A Base64 encoded string representing the compressed data.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the input string is null.</exception>
    public static string CompressString(this string jsonString)
    {
        if (jsonString == null)
        {
            throw new ArgumentNullException(nameof(jsonString), "The string to compress cannot be null.");
        }

        // Convert the string to a byte array using UTF-8 encoding
        byte[] originalBytes = Encoding.UTF8.GetBytes(jsonString);

        using (MemoryStream memoryStream = new MemoryStream())
        {
            // Create a GZipStream to compress the data and write it to the memory stream
            using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gzipStream.Write(originalBytes, 0, originalBytes.Length);
            } // The GZipStream is closed here, which flushes its buffer to the underlying MemoryStream

            // Get the compressed byte array from the memory stream
            byte[] compressedBytes = memoryStream.ToArray();

            // Convert the compressed byte array to a Base64 string for easy storage/transmission
            return Convert.ToBase64String(compressedBytes);
        }
    }

    /// <summary>
    /// Decompresses a Base64 encoded string that was previously compressed using GZip,
    /// and returns the original string (e.g., the original JSON string).
    /// </summary>
    /// <param name="compressedString">The Base64 encoded compressed string to decompress.</param>
    /// <returns>The original decompressed string.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the input string is null.</exception>
    /// <exception cref="FormatException">Thrown if the input string is not a valid Base64 string.</exception>
    public static string DecompressString(this string compressedString)
    {
        if (compressedString == null)
        {
            throw new ArgumentNullException(nameof(compressedString), "The string to decompress cannot be null.");
        }

        // Convert the Base64 string back to a byte array
        byte[] compressedBytes = Convert.FromBase64String(compressedString);

        using (MemoryStream memoryStream = new MemoryStream(compressedBytes))
        {
            // Create a GZipStream to decompress the data from the memory stream
            using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                // Create a new MemoryStream to hold the decompressed data
                using (MemoryStream decompressedStream = new MemoryStream())
                {
                    // Copy the decompressed bytes from the GZipStream to the decompressedStream
                    gzipStream.CopyTo(decompressedStream);

                    // Get the decompressed byte array
                    byte[] decompressedBytes = decompressedStream.ToArray();

                    // Convert the decompressed byte array back to a string using UTF-8 encoding
                    return Encoding.UTF8.GetString(decompressedBytes);
                }
            }
        }
    }
}
