// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using CS13;

//Console.WriteLine("Hello, World!");


var summary = BenchmarkRunner.Run<BenchmarkTest>();

//int runCount = 10000000;
////var logger = new ConsoleLogger(true);
//var logger = new ConsoleLogger(false);
//var fibonacciRunner = new FibonacciRunner(logger, runCount);
//var testRunner = new TestRunner(logger, fibonacciRunner);
//testRunner.RunTests(10);

//var newLock = new NewLock();
//var benchmark = new LockBenchmark();
//benchmark.UsingOldLock();