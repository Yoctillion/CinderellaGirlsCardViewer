using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CinderellaGirlsCardViewer.Annotations;

namespace CinderellaGirlsCardViewer
{
    internal class SimpleTaskManager
    {
        private readonly object _taskCreatingLock = new object();
        private readonly object _cancelTokenLock = new object();
        private CancellationTokenSource _lastCancelToken;

        public void Do<T>(SpecialTask<T> task)
        {
            this.DoInternal(token =>
            {
                var res = task.BeforeCancelling();

                if (this.IsTokenCancelled(token))
                {
                    task.IfCancelled?.Invoke(res);
                }
                else
                {
                    task.IfNotCancelled?.Invoke(res);
                }

                task.Finally?.Invoke(res);
            });
        }

        public void Do(SpecialTask task)
        {
            this.DoInternal(token =>
            {
                task.BeforeCancel();

                if (this.IsTokenCancelled(token))
                {
                    task.IfCancelled?.Invoke();
                }
                else
                {
                    task.IfNotCancelled?.Invoke();
                }

                task.Finally?.Invoke();
            });
        }

        private void DoInternal(Action<CancellationToken> action)
        {
            lock (this._taskCreatingLock)
            {
                if (this._lastCancelToken != null)
                {
                    lock (this._cancelTokenLock)
                    {
                        if (this._lastCancelToken != null)
                        {
                            this._lastCancelToken.Cancel();
                            this._lastCancelToken.Dispose();
                        }
                    }
                }

                this._lastCancelToken = new CancellationTokenSource();
                var token = this._lastCancelToken.Token;

                Task.Run(() => action(token), token);
            }
        }

        private bool IsTokenCancelled(CancellationToken token)
        {
            lock (this._cancelTokenLock)
            {
                var isCancelled = token.IsCancellationRequested;

                if (!isCancelled)
                {
                    this._lastCancelToken.Dispose();
                    this._lastCancelToken = null;
                }

                return isCancelled;
            }
        }
    }

    internal struct SpecialTask<T>
    {
        [NotNull]
        public Func<T> BeforeCancelling { get; set; }
        public Action<T> IfCancelled { get; set; }
        public Action<T> IfNotCancelled { get; set; }
        public Action<T> Finally { get; set; }
    }

    internal struct SpecialTask
    {
        [NotNull]
        public Action BeforeCancel { get; set; }
        public Action IfCancelled { get; set; }
        public Action IfNotCancelled { get; set; }
        public Action Finally { get; set; }
    }
}
