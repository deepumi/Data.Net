namespace Data.Net.PerformanceTest
{
    internal abstract class BasePerfTest
    {
        protected internal string ConnectionString = "Data Source=Initial Catalog=DataNet;Integrated Security=True";

        protected internal int MaxLimit = 2;
    }
}