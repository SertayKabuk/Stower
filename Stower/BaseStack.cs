using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Stower
{
    public interface IStack<T>
    {
        IEnumerable<T> TakeAll();
    }

    internal class BaseStack<T> : ConcurrentBag<T>, IStack<T>
    {
        public IEnumerable<T> TakeAll()
        {
            while (TryTake(out T item))
            {
                yield return item;
            }
        }
    }
}
