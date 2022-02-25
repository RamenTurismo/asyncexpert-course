using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitExercises.Core
{
    public class AsyncHelpers
    {
        public static async Task<string> GetStringWithRetries(HttpClient client, string url, int maxTries = 3, CancellationToken token = default)
        {
            if (maxTries < 2)
            {
                throw new ArgumentException(nameof(maxTries));
            }
            
            for (int i = 0, waitSeconds = 1; i < maxTries; i++)
            {
                if (i != 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(waitSeconds), token);
                    waitSeconds *= 2;
                }

                ThrowIfCancellationRequested(token);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(new Uri(url), token);
                    
                    if (response == null)
                    {
                        throw new InvalidOperationException();
                    }

                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStringAsync();
                }
                catch (Exception) when (i == maxTries - 1)
                {
                    throw;
                }
                catch
                {
                    // Ignored
                }
            }

            throw new InvalidOperationException();

            // Create a method that will try to get a response from a given `url`, retrying `maxTries` number of times.
            // It should wait one second before the second try, and double the wait time before every successive retry
            // (so pauses before retries will be 1, 2, 4, 8, ... seconds).
            // * `maxTries` must be at least 2
            // * we retry if:
            //    * we get non-successful status code (outside of 200-299 range), or
            //    * HTTP call thrown an exception (like network connectivity or DNS issue)
            // * token should be able to cancel both HTTP call and the retry delay
            // * if all retries fails, the method should throw the exception of the last try
            // HINTS:
            // * `HttpClient.GetStringAsync` does not accept cancellation token (use `GetAsync` instead)
            // * you may use `EnsureSuccessStatusCode()` method
        }


        private static void ThrowIfCancellationRequested(CancellationToken cancellation)
        {
            if (cancellation.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }
        }
    }
}
