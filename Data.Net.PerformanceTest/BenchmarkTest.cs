using BenchmarkDotNet.Attributes;

namespace Data.Net.PerformanceTest
{
    [RankColumn, MemoryDiagnoser]
    public class BenchmarkTest
    {
        [Benchmark]
        public void DataNet()
        {
           //new DataNetPerfTest().QuerySingleTest();
           new InsertMsSqlDataNet().Insert(new User {FirstName = "DataNet", LastName = "DataNet_12"});
        }

        // [Benchmark]
        // public void Dapper()
        // {
        //     new DapperTest().QuerySingleTest();
        // }

        [Benchmark]
        public void AdoNet()
        {
            //new AdoNetPerfApi().ExecuteScalarTest();
            new AdoNetInsertSqlPerfTest().Insert(new User {FirstName = "AdoNet_F", LastName = "AdoNet_12"});
        }
    }
}
