using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CinderellaGirlsCardViewer
{
    public static class ReaderWriterLockSlimExtensions
    {
        public static T LockRead<T>(this ReaderWriterLockSlim rwLock, Func<T> func)
        {
            rwLock.EnterReadLock();
            var result = func();
            rwLock.ExitReadLock();
            return result;
        }

        public static void LockRead(this ReaderWriterLockSlim rwLock, Action action)
        {
            rwLock.LockRead(Useless);
        }

        public static T LockUpgradeableRead<T>(this ReaderWriterLockSlim rwLock, Func<T> func)
        {
            rwLock.EnterUpgradeableReadLock();
            var result = func();
            rwLock.ExitUpgradeableReadLock();
            return result;
        }

        public static void LockUpgradeableRead(this ReaderWriterLockSlim rwLock, Action action)
        {
            rwLock.LockUpgradeableRead(Useless);
        }

        public static T LockWrite<T>(this ReaderWriterLockSlim rwLock, Func<T> func)
        {
            rwLock.EnterWriteLock();
            var result = func();
            rwLock.ExitWriteLock();
            return result;
        }

        public static void LockWrite(this ReaderWriterLockSlim rwLock, Action action)
        {
            rwLock.LockWrite(Useless);
        }

        private static bool Useless()
        {
            return false;
        }
    }
}
