namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        #region Public Methods

        public static Task StartDelayed(this TaskFactory factory, int dueTime, Action continuationAction)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            if (dueTime < 0)
                throw new ArgumentNullException(nameof(dueTime));

            if (continuationAction == null)
                throw new ArgumentNullException(nameof(continuationAction));

            if (factory.CancellationToken.IsCancellationRequested)
                return new Task(() => { }, factory.CancellationToken);

            var completionSource = new TaskCompletionSource<object>(factory.CreationOptions);
            var cancellationTokenRegistration = default(CancellationTokenRegistration);
            var timer = new Timer(t =>
            {
                cancellationTokenRegistration.Dispose();
                ((Timer)t).Dispose();
                completionSource.TrySetResult(null);
            });

            if (factory.CancellationToken.CanBeCanceled)
            {
                cancellationTokenRegistration = factory.CancellationToken.Register(() =>
                {
                    timer.Dispose();
                    completionSource.TrySetCanceled();
                });
            }

            try
            {
                timer.Change(dueTime, Timeout.Infinite);
            }
            catch { }

            return completionSource.Task.ContinueWith(
                _ => continuationAction(),
                factory.CancellationToken,
                TaskContinuationOptions.OnlyOnRanToCompletion,
                factory.Scheduler ?? TaskScheduler.Current);
        }

        #endregion
    }
}
