using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ITGlobal.MarkDocs.Impl
{
    internal sealed class DebounceAction<T> : IDisposable
    {
        private const int DEBOUNCE_INTERVAL_MS = 250;

        private readonly Timer _timer;
        private readonly object _syncRoot = new object();
        private bool _triggered;

        private readonly SemaphoreSlim _running = new SemaphoreSlim(1);

        private readonly Action<T> _callback;
        private readonly IMarkDocsLog _log;

        private readonly HashSet<T> _pendingRequests = new HashSet<T>();

        public DebounceAction(IMarkDocsLog log, Action<T> callback)
        {
            _log = log;
            _callback = callback;

            _timer = new Timer(OnTimer, null, -1, -1);
        }

        public void Trigger(T arg)
        {
            lock (_syncRoot)
            {
                if (!_pendingRequests.Add(arg))
                {
                    return;
                }

                if (_triggered)
                {
                    return;
                }

                _triggered = true;
            }

            _timer.Change(DEBOUNCE_INTERVAL_MS, -1);
        }

        public void Dispose()
        {
            _timer.Dispose();
            _running.Dispose();
        }

        private void OnTimer(object _)
        {
            lock (_syncRoot)
            {
                _triggered = false;
            }

            _running.Wait();
            try
            {
                while (true)
                {
                    T arg;
                    lock (_syncRoot)
                    {
                        if (_pendingRequests.Count == 0)
                        {
                            return;
                        }

                        arg = _pendingRequests.First();
                        _pendingRequests.Remove(arg);
                    }

                    try
                    {
                        _callback(arg);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex, $"Unable to handle debounce action {arg}");
                    }
                }
            }
            finally
            {
                _running.Release();
            }
        }
    }
}