using System;
using System.Runtime.CompilerServices;

namespace AwaitableExercises.Core
{
    public static class BoolExtensions
    {
        public static BoolAwaiter GetAwaiter(this bool b)
        {
            return new BoolAwaiter(b);
        }
    }

    public class BoolAwaiter : INotifyCompletion
    {
        private readonly bool _origin;

        public BoolAwaiter(bool origin)
        {
            _origin = origin;
        }

        public bool IsCompleted => true;

        public void OnCompleted(Action continuation)
        {
            continuation();
        }

        public bool GetResult() => _origin;
    }
}
