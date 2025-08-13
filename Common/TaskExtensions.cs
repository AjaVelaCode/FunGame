namespace FunGame.Common
{
    public static class TaskExtensions
    {
        public static async Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource();
            var completedTask = await Task.WhenAny(task, Task.Delay(timeout, cts.Token));
            if (completedTask != task) throw new TimeoutException("The operation has timed out.");
            await cts.CancelAsync();
            return await task;
        }
    }
}
