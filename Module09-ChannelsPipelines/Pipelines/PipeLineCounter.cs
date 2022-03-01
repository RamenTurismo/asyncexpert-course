using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pipelines
{
    public class PipeLineCounter
    {
        public async Task<int> CountLines(Uri uri)
        {
            using HttpClient client = new HttpClient();
            await using Stream stream = await client.GetStreamAsync(uri);

            // Calculate how many lines (end of line characters `\n`) are in the network stream
            // To practice, use a pattern where you have the Pipe, Writer and Reader tasks
            // Read about SequenceReader<T>, https://docs.microsoft.com/en-us/dotnet/api/system.buffers.sequencereader-1?view=netcore-3.1
            // This struct h has a method that can be very useful for this scenario :)

            // Good luck and have fun with pipelines!
            var pipe = new Pipe();

            Task write = WriteAsync(pipe.Writer, stream);
            Task<int> read = ReadAsync(pipe.Reader);

            await Task.WhenAll(write, read);

            return read.Result;
        }

        private static async Task WriteAsync(PipeWriter writer, Stream stream)
        {
            while (true)
            {
                Memory<byte> buffer = writer.GetMemory();
                int position = await stream.ReadAsync(buffer);
                writer.Advance(position);

                if (position == 0)
                {
                    break;
                }

                await writer.FlushAsync();
            }

            await writer.CompleteAsync();
        }

        private static async Task<int> ReadAsync(PipeReader reader)
        {
            var count = 0;

            while (true)
            {
                ReadResult result = await reader.ReadAsync();
                ReadOnlySequence<byte> buffer = result.Buffer;
                count += CountEndOfLines(buffer, count);

                reader.AdvanceTo(buffer.End);

                if (result.IsCompleted)
                {
                    break;
                }
            }

            return count;
        }

        private static int CountEndOfLines(in ReadOnlySequence<byte> buffer, int count)
        {
            var reader = new SequenceReader<byte>(buffer);

            while (reader.TryAdvanceTo((byte)'\n'))
            {
                count++;
            }

            return count;
        }
    }
}
