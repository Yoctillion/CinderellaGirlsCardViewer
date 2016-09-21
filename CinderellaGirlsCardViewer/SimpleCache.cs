using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CinderellaGirlsCardViewer
{
    internal class SimpleCache<TKey, TValue> : IDisposable
    {
        private readonly Dictionary<TKey, Lazy<TValue>> _inner = new Dictionary<TKey, Lazy<TValue>>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public TValue Get(TKey key, Func<TKey, TValue> factory)
        {
            return this._lock.LockUpgradeableRead(() =>
            {
                Lazy<TValue> lazy;
                if (!this._inner.TryGetValue(key, out lazy))
                {
                    lazy = this.UpdateInternal(key, factory);
                }
                return lazy;
            }).Value;
        }

        public TValue Update(TKey key, Func<TKey, TValue> factory)
        {
            return this.UpdateInternal(key, factory).Value;
        }

        private Lazy<TValue> UpdateInternal(TKey key, Func<TKey, TValue> factory)
        {
            return this._lock.LockWrite(() =>
            {
                var lazy = new Lazy<TValue>(() => factory(key));
                this._inner[key] = lazy;
                return lazy;
            });
        }

        public TValue Remove(TKey key)
        {
            Lazy<TValue> lazy = null;
            this._lock.LockUpgradeableRead(() =>
            {
                if (this._inner.TryGetValue(key, out lazy))
                {
                    this._lock.LockWrite(() => this._inner.Remove(key));
                }
            });

            return (lazy != null && lazy.IsValueCreated)
                ? lazy.Value
                : default(TValue);
        }

        public void Clear()
        {
            this._inner.Clear();
        }

        public void Dispose()
        {
            this._lock.Dispose();
        }
    }
}
