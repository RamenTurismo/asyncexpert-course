using System;
using System.Threading;

namespace Synchronization.Core
{
    /*
     * Implement very simple wrapper around Mutex or Semaphore (remember both implement WaitHandle) to
     * provide a exclusive region created by `using` clause.
     *
     * Created region may be system-wide or not, depending on the constructor parameter.
     *
     * Any try to get a second systemwide scope should throw an `System.InvalidOperationException` with `Unable to get a global lock {name}.`
     */
    public class NamedExclusiveScope : IDisposable
    {
        private static Mutex _globalMutex;
        private static object _lock = new object();

        private readonly Mutex _mutex;

        public NamedExclusiveScope(string name, bool isSystemWide)
        {
            if (isSystemWide)
            {
                if (_globalMutex != null)
                {
                    throw new InvalidOperationException($"Unable to get a global lock {name}.");
                }

                _globalMutex = new Mutex(false, $"{name}Global");
                _mutex = _globalMutex;
            }
            else
            {
                _mutex = new Mutex(false, $"{name}Local");
            }

            lock (_lock)
            {
                _mutex.WaitOne();
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _mutex.Dispose();
        }
    }
}
