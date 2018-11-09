using System;
using System.IO;

namespace LexTalionis.StreamTools
{
    /// <summary>
    /// Расширения для потоков
    /// </summary>
    public static class StreamExtensions
    {
        private const int DefaultBufferSize = 4096;

        /// <summary>
        /// Скопировать из потока в поток
        /// </summary>
        /// <param name="source">откуда</param>
        /// <param name="destination">куда</param>
        /// <returns>переданный объем данных</returns>
        public static long CopyTo(this Stream source, Stream destination)
        {
            return CopyTo(source, destination, DefaultBufferSize, _ => { });
        }

        /// <summary>
        /// Скопировать из потока в поток
        /// </summary>
        /// <param name="source">откуда</param>
        /// <param name="destination">куда</param>
        /// <param name="bufferSize">размер буффера</param>
        /// <returns>переданный объем данных</returns>
        public static long CopyTo(this Stream source, Stream destination, int bufferSize)
        {
            return CopyTo(source, destination, bufferSize, _ => { });
        }

        /// <summary>
        /// Скопировать из потока в поток
        /// </summary>
        /// <param name="source">откуда</param>
        /// <param name="destination">куда</param>
        /// <param name="reportProgress">действие при причтении порции данных</param>
        /// <returns>переданный объем данных</returns>
        public static long CopyTo(this Stream source, Stream destination, Action<long> reportProgress)
        {
            return CopyTo(source, destination, DefaultBufferSize, reportProgress);
        }

        /// <summary>
        /// Скопировать из потока в поток
        /// </summary>
        /// <param name="source">откуда</param>
        /// <param name="destination">куда</param>
        /// <param name="bufferSize">размер буфера</param>
        /// <param name="reportProgress">действие при причтении порции данных</param>
        /// <returns>переданный объем данных</returns>
        public static long CopyTo(this Stream source, Stream destination, int bufferSize, Action<long> reportProgress)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (destination == null) throw new ArgumentNullException("destination");
            if (reportProgress == null) throw new ArgumentNullException("reportProgress");

            var buffer = new byte[bufferSize];

            var transferredBytes = 0L;

            for (var bytesRead = source.Read(buffer, 0, buffer.Length);
                 bytesRead > 0;
                 bytesRead = source.Read(buffer, 0, buffer.Length))
            {
                transferredBytes += bytesRead;
                reportProgress(transferredBytes);

                destination.Write(buffer, 0, bytesRead);
            }

            destination.Flush();
            destination.Seek(0, SeekOrigin.Begin);
            return transferredBytes;
        }
    }
}
