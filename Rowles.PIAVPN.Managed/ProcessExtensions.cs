using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Rowles.PIA.Managed
{
    // https://stackoverflow.com/questions/470256/process-waitforexit-asynchronously
    public static class ProcessExtensions
    {
        public static Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default)
        {
            if (process.HasExited) return Task.CompletedTask;

            var tcs = new TaskCompletionSource<object>();
            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => tcs.TrySetResult(null);
            if (cancellationToken != default)
                cancellationToken.Register(() => tcs.SetCanceled());

            return process.HasExited ? Task.CompletedTask : tcs.Task;
        }

    }
}
