namespace Data.Net.PerformanceTest
{
    internal abstract class BasePerfTest
    {
        protected internal string ConnectionString = "Data Source=darsh;Initial Catalog=Bot;Integrated Security=True";

        protected internal int MaxLimit = 2;
    }
}