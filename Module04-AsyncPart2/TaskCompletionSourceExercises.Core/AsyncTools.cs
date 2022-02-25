using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TaskCompletionSourceExercises.Core
{
    public class AsyncTools
    {
        public static Task<string> RunProgramAsync(string path, string args = "")
        {
            var tcs = new TaskCompletionSource<string>(TaskContinuationOptions.RunContinuationsAsynchronously);

            Process process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo(path, args)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            process.Exited += (sender, eventArgs) =>
            {
                Process senderProcess = (Process)sender;
                string output = senderProcess?.StandardOutput.ReadToEnd();
                string error = senderProcess?.StandardError.ReadToEnd();
                senderProcess?.Dispose();

                if (!string.IsNullOrEmpty(error))
                {
                    tcs.SetException(new Exception(error));
                }
                else
                {
                    tcs.SetResult(output);
                }
            };

            process.Start();

            return tcs.Task;
        }
    }
}
