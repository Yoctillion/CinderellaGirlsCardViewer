using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinderellaGirlsCardViewer
{
    internal class SimpleCache<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, Lazy<Task<TValue>>> _inner = new ConcurrentDictionary<TKey, Lazy<Task<TValue>>>();

        public TValue Get(TKey key, Func<TKey, TValue> factory)
        {
            return this.GetAsync(key, k => Task.Run(() => factory(k))).Result;
        }

        public Task<TValue> GetAsync(TKey key, Func<TKey, Task<TValue>> factory)
        {
            return this._inner.GetOrAdd(key, new Lazy<Task<TValue>>(() => factory(key))).Value;
        }

        //public TValue Update(TKey key, Func<TKey, TValue> factory)
        //{
        //    return this._inner.AddOrUpdate(key, new Lazy<TValue>(() => factory(key)), (o, n) => n).Value;
        //}

        public TValue Remove(TKey key)
        {
            Lazy<Task<TValue>> lazy;
            this._inner.TryRemove(key, out lazy);
            return (lazy != null && lazy.IsValueCreated && lazy.Value.IsCompleted)
                ? lazy.Value.Result
                : default(TValue);
        }

        public void Clear()
        {
            this._inner.Clear();
        }
    }
}
