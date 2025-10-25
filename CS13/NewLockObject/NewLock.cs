using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS13.NewLockObject
{
    internal class NewLock
    {
        private readonly Lock _lockObj = new();

        public void Modify()
        {
            lock (_lockObj)
            {
                // Critical section associated with _lockObj
                Console.WriteLine("Inside lock 1");
            }

            using (_lockObj.EnterScope())
            {
                // Critical section associated with _lockObj
                Console.WriteLine("Inside lock 2");
            }

            _lockObj.Enter();
            try
            {
                // Critical section associated with _lockObj
                Console.WriteLine("Inside lock 3");
            }
            finally { _lockObj.Exit(); }

            if (_lockObj.TryEnter())
            {
                try
                {
                    // Critical section associated with _lockObj
                    Console.WriteLine("Inside lock 4");
                }
                finally { _lockObj.Exit(); }
            }
        }
    }

    public class LockBenchmark
    {
        private readonly object oldLockObject = new object();
        private readonly Lock lockObject = new();
        private long counter;

        private static long RunFibonacciSequence(int n)
        {
            long n1 = 0;
            long n2 = 1;
            for (int i = 0; i < n; i++)
            {
                long oldN2 = n2;
                n2 = n2 + n1;
                n1 = oldN2;
            }

            return n2;
        }

        [Benchmark]
        public void UsingOldLock()
        {
            lock(oldLockObject)
            {
                counter = RunFibonacciSequence(100);
            }
        }

        [Benchmark]
        public void UsingLock()
        {
            lock (lockObject)
            {
                counter = RunFibonacciSequence(100);
            }
        }

        [Benchmark]
        public void UsingLockEnterScope()
        {
            using (lockObject.EnterScope())
            {
                counter = RunFibonacciSequence(100);
            }
        }

        [Benchmark]
        public void UsingLockEnter()
        {
            lockObject.Enter();
            try
            {
                counter = RunFibonacciSequence(100);
            }
            finally 
            { 
                lockObject.Exit(); 
            }
        }

        [Benchmark]
        public void UsingLockTryEnter()
        {
            if (lockObject.TryEnter())
            {
                try
                {
                    counter = RunFibonacciSequence(100);
                }
                finally 
                { 
                    lockObject.Exit(); 
                }
            }

            //for (short spinIndex = 0; ;)
            //{

            //}
        }
    }
}
