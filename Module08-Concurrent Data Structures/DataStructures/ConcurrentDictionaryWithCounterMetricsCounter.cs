using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DataStructures
{
    public class ConcurrentDictionaryWithCounterMetricsCounter : IMetricsCounter
    {
        private readonly ConcurrentDictionary<string, AtomicCounter> _counters = new ConcurrentDictionary<string, AtomicCounter>();

        // Implement this class using ConcurrentDictionary and the provided AtomicCounter class.
        // AtomicCounter should be created only once per key, then its Increment method should be used.

        public IEnumerator<KeyValuePair<string, int>> GetEnumerator() => _counters.Select(e => KeyValuePair.Create(e.Key, e.Value.Count)).GetEnumerator();

        public void Increment(string key)
        {
            _counters.AddOrUpdate(key, 
                _ =>
                {
                    AtomicCounter newInstance = new AtomicCounter();
                    newInstance.Increment();

                    return newInstance;
                }, 
                (_, counter) =>
                {
                    counter.Increment();

                    return counter;
                });
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public class AtomicCounter
        {
            int value;

            public void Increment() => Interlocked.Increment(ref value);

            public int Count => Volatile.Read(ref value);
        }
    }
}