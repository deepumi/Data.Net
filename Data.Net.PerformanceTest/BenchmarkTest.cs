using BenchmarkDotNet.Attributes;

namespace Data.Net.PerformanceTest
{
    [RankColumn, MemoryDiagnoser]
    public class BenchmarkTest
    {
        [Benchmark]
        public void DataNet()
        {
            new DataNetPerfTest().QuerySingleTest();
        }

        [Benchmark]
        public void Dapper()
        {
            new DapperTest().QuerySingleTest();
        }

        [Benchmark]
        public void AdoNet()
        {
            new AdoNetPerfApi().ExecuteScalarTest();
        }
    }
}
