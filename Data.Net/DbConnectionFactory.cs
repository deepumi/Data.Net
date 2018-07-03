#if NET461 || NET462 || NET47 || NET471 || NET472

using System;
using System.Configuration;
using System.Data.Common;
using System.Data;

namespace Data.Net
{
    internal static class DbConnectionFactory
    {
        internal static IDbConnection Connection => OpenConnection();

        internal static IDbConnection OpenConnection(string connectionStringName = null)
        {
            if(ConfigurationManager.ConnectionStrings.Count == 0) throw new InvalidOperationException("Please specify a connection string in configuration connection string section");

            var defaultConnectionString = connectionStringName == null ? ConfigurationManager.ConnectionStrings[0] : ConfigurationManager.ConnectionStrings[connectionStringName];
        
            if(string.IsNullOrWhiteSpace(defaultConnectionString.ProviderName)) throw new InvalidOperationException("Unable to read provider name. Please configure provider name property in the connection string");  
            
            var factory = DbProviderFactories.GetFactory(defaultConnectionString.ProviderName);

            var connection = factory.CreateConnection();

            if(connection == null)  throw new InvalidOperationException("Unable to create a connection");

            connection.ConnectionString = defaultConnectionString.ConnectionString;

            return connection;
        }
    }
}

#endif