namespace Soul.Shop.Infrastructure.Helpers;

public static class AsyncHelper
{
    public static void RunSync(Func<Task> task)
    {
        var oldContext = SynchronizationContext.Current;
        var sync = new ExclusiveSynchronizationContext();
        SynchronizationContext.SetSynchronizationContext(sync);
        sync.Post(async _ =>
        {
            try
            {
                await task();
            }
            catch (Exception e)
            {
                sync.InnerException = e;
                throw;
            }
            finally
            {
                sync.EndMessageLoop();
            }
        }, null);
        sync.BeginMessageLoop();

        SynchronizationContext.SetSynchronizationContext(oldContext);
    }

    public static T RunSync<T>(Func<Task<T>> task)
    {
        var oldContext = SynchronizationContext.Current;
        var sync = new ExclusiveSynchronizationContext();
        SynchronizationContext.SetSynchronizationContext(sync);
        var ret = default(T);
        sync.Post(async _ =>
        {
            try
            {
                ret = await task();
            }
            catch (Exception e)
            {
                sync.InnerException = e;
                throw;
            }
            finally
            {
                sync.EndMessageLoop();
            }
        }, null);
        sync.BeginMessageLoop();
        SynchronizationContext.SetSynchronizationContext(oldContext);
        return ret;
    }

    private class ExclusiveSynchronizationContext : SynchronizationContext
    {
        private bool _done;
        public Exception InnerException { get; set; }
        private readonly AutoResetEvent _workItemsWaiting = new(false);
        private readonly Queue<Tuple<SendOrPostCallback, object>> _items = new();

        public override void Send(SendOrPostCallback d, object? state)
        {
            throw new NotSupportedException("We cannot send to our same thread");
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            lock (_items)
            {
                _items.Enqueue(Tuple.Create(d, state));
            }

            _workItemsWaiting.Set();
        }

        public void EndMessageLoop()
        {
            Post(_ => _done = true, null!);
        }

        public void BeginMessageLoop()
        {
            while (!_done)
            {
                Tuple<SendOrPostCallback, object> task = null!;
                lock (_items)
                {
                    if (_items.Count > 0) task = _items.Dequeue();
                }

                if (task != null)
                {
                    task.Item1(task.Item2);
                    if (InnerException != null) // the method threw an exeption
                        throw new AggregateException("AsyncHelpers.Run method threw an exception.", InnerException);
                }
                else
                {
                    _workItemsWaiting.WaitOne();
                }
            }
        }

        public override SynchronizationContext CreateCopy()
        {
            return this;
        }
    }
}