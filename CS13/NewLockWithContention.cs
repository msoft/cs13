using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS13
{
    public class BenchmarkTest
    {
        private readonly ILogger logger = new ConsoleLogger(false);
        private int runCount = 10000000;
        private int threadCount = 10;

        [Benchmark]
        public void UsingOldLock()
        {
            var runner = new TestRunner(logger, new RunnerWithOldLock(logger, runCount));
            runner.RunTests(threadCount);
        }

        [Benchmark]
        public void UsingLockEnterScope()
        {
            var runner = new TestRunner(logger, new RunnerUsingLockEnterScope(logger, runCount));
            runner.RunTests(threadCount);
        }

        [Benchmark]
        public void UsingLockEnter()
        {
            var runner = new TestRunner(logger, new RunnerUsingLockEnter(logger, runCount));
            runner.RunTests(threadCount);
        }

        [Benchmark]
        public void UsingLockTryEnter()
        {
            var runner = new TestRunner(logger, new RunnerUsingLockTryEnter(logger, runCount));
            runner.RunTests(threadCount);
        }


        //| Method              | Mean    | Error    | StdDev   |
        //|-------------------- |--------:|---------:|---------:|
        //| UsingOldLock        | 1.785 s | 0.0361 s | 0.1041 s |
        //| UsingLockEnterScope | 1.540 s | 0.0216 s | 0.0192 s |
        //| UsingLockEnter      | 1.576 s | 0.0312 s | 0.0334 s |
        //| UsingLockTryEnter   | 1.453 s | 0.0278 s | 0.0352 s |
    }

    internal class TestRunner
    {
        private readonly IRunner runner;
        private readonly ILogger logger;

        public TestRunner(ILogger logger, IRunner runner)
        {
            this.logger = logger;
            this.runner = runner;
        }

        public void RunTests(int taskCount)
        {
            var tasks = new Task[taskCount];
            for (int i = 0; i < taskCount; i++)
            {
                logger.Log($"Launching task #{i}");
                Task task = new Task(() => RunJob(i));
                tasks[i] = task;
                task.Start();
            }

            Task.WaitAll(tasks);
        }

        private void RunJob(int taskId)
        {
            bool canRun = true;
            while (canRun)
            {
                logger.Log($"Task #{taskId} job starts...");
                canRun = runner.Run();
                logger.Log($"Task #{taskId} job ends (canRun: {canRun}).");
            }
        }
    }

    internal interface IRunner
    {
        bool Run();
    }

    internal class RunnerWithOldLock: IRunner
    {
        private readonly FibonacciRunner runner;
        private object oldLock = new object();

        public RunnerWithOldLock(ILogger logger, int runCount)
        {
            runner = new FibonacciRunner(logger, runCount);
        }

        public bool Run()
        {
            lock (oldLock)
            {
                return runner.Run();
            }
        }
    }

    internal class RunnerUsingLockEnterScope : IRunner
    {
        private readonly FibonacciRunner runner;
        private readonly Lock lockObject = new();

        public RunnerUsingLockEnterScope(ILogger logger, int runCount)
        {
            runner = new FibonacciRunner(logger, runCount);
        }

        public bool Run()
        {
            using (lockObject.EnterScope())
            {
                return runner.Run();
            }
        }
    }

    internal class RunnerUsingLockEnter : IRunner
    {
        private readonly FibonacciRunner runner;
        private readonly Lock lockObject = new();

        public RunnerUsingLockEnter(ILogger logger, int runCount)
        {
            runner = new FibonacciRunner(logger, runCount);
        }

        public bool Run()
        {
            lockObject.Enter();
            try
            {
                return runner.Run();
            }
            finally
            {
                lockObject.Exit();
            }
        }
    }

    internal class RunnerUsingLockTryEnter : IRunner
    {
        private readonly FibonacciRunner runner;
        private readonly Lock lockObject = new();

        public RunnerUsingLockTryEnter(ILogger logger, int runCount)
        {
            runner = new FibonacciRunner(logger, runCount);
        }

        public bool Run()
        {
            if (lockObject.TryEnter())
            {
                try
                {
                    return runner.Run();
                }
                finally
                {
                    lockObject.Exit();
                }
            }

            return false;
        }
    }

    internal class FibonacciRunner: IRunner
    {
        private readonly int runCount;
        private readonly List<long> fibonacciRunResults;
        //private readonly long[] fibonacciRunResults;
        private readonly Random random = new();
        private int runIndex = 0;
        private readonly ILogger logger;

        public FibonacciRunner(ILogger logger, int runCount)
        {
            this.logger = logger;
            this.runCount = runCount;
            fibonacciRunResults = new List<long>();
        }

        public bool Run()
        {
            if (runIndex >= runCount)
                return false;

            int n = random.Next(1, 100);
            long result = RunFibonacciSequence(n);
            logger.Log($"Fibonacci({n}) = {result} for run #{runIndex}");
            //if (runIndex < runCount)
            //{
                //fibonacciRunResults[runIndex] = result;
                fibonacciRunResults.Add(result);
                runIndex++;
            //}
                
            return runIndex < runCount;
        }

        private static long RunFibonacciSequence(int n)
        {
            long n1 = 0;
            long n2 = 1;
            for (int i = 0; i < n; i++)
            {
                long oldN2 = n2;
                n2 = n1 + n2;
                n1 = oldN2;
            }
            return n1;
        }
    }

    internal class NewLockWithContention
    {

    }
}
