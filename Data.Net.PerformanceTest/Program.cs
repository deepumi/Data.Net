using System;
using BenchmarkDotNet.Running;

namespace Data.Net.PerformanceTest
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<BenchmarkTest>();

            Console.WriteLine("DONE");
            Console.ReadKey();
        }
    }
}