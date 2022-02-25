using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TaskCombinatorsExercises.Core
{
    public static class HttpClientExtensions
    {
        /*
         Write cancellable async method with timeout handling, that concurrently tries to get data from
         provided urls (first wins and its response is returned, rest is __cancelled__).
         
         Tips:
         * consider using HttpClient.GetAsync (as it is cancellable)
         * consider using Task.WhenAny
         * you may use urls like for testing https://postman-echo.com/delay/3
         * you should have problem with tasks cancellation -
            - how to merge tokens of operations (timeouts) with the provided token? 
            - Tip: you can link tokens with the help of CancellationTokenSource.CreateLinkedTokenSource(token)
         */
        public static async Task<string> ConcurrentDownloadAsync(this HttpClient httpClient,
            string[] urls, int millisecondsTimeout, CancellationToken token)
        {
            using CancellationTokenSource tcs = new CancellationTokenSource(millisecondsTimeout);
            token.Register(state => ((CancellationTokenSource)state).Cancel(), tcs);

            using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(tcs.Token, token);

            string result = null;

            foreach (string url in urls)
            {
                linked.Token.ThrowIfCancellationRequested();

                using HttpResponseMessage response = await httpClient.GetAsync(url, linked.Token);

                if (response is null)
                {
                    continue;
                }

                result = await response.Content.ReadAsStringAsync();
            }

            return result;
        }
    }
}
