using Microsoft.Extensions.Configuration;

namespace Data.Net.Test
{
    internal static class AppSettings
    {
        internal static readonly IConfiguration Config = LoadConfig();

        private static IConfiguration LoadConfig()
        {
            return new ConfigurationBuilder()
                       .AddJsonFile("appsettings.json", true, true)
                       .Build();
        }
    }
}