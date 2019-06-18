# Data.Net
A simple, lightweight data access library for ADO.Net providers like Microsoft SQL Server, Oracle Database, MySQL and PostgreSQL.

## No Dependencies
Since Data.Net is written in .NetStandard 2.0 no third party libraries are required. 

## Supporting platforms
Data.Net support multiple frameworks and platforms.

*  Net Core 2.0 / ASP.NET Core 2.2 / ASP.NET Core 2.1 / ASP.NET Core 2.0.
*  Net Standard 2.0,
*  NetFramework 4.8.0
*  NetFramework 4.6.1 and above

## Nuget
Install [Data.Net](https://www.nuget.org/packages/Data.Net/) via [Nuget](https://www.nuget.org/packages/Data.Net/)
```csharp
PM> Install-Package Data.Net
```

Execute a query and return a single value from the first row
------------------------------------------------------------
```csharp
using (var db = new Database()) //make sure connection string is configured in the config file with <connectionStrings><clear/><add../></connectionStrings>
{
  var firstName = db.ExecuteScalar<string>("SELECT FirstName From Users_Test Where Email = @Email",
                  parameters:new DataParameters{{"Email", "user@domain.com" } });
  Assert.IsTrue(!string.IsNullOrWhiteSpace(firstName));
}
```
Async support
-----------------------------------------
Starting Data.Net version 2.0 support Asynchronous APIs.
```csharp
var result = await db.ExecuteNonQueryAsync(string sql, CommandType commandType = CommandType.Text, DataParameters parameters = null, CancellationToken token = default(CancellationToken));

var scalar = await db.ExecuteScalarAsync<T>(string sql, CommandType commandType = CommandType.Text, DataParameters parameters = null, CancellationToken token = default(CancellationToken));

var reader = await db.ExecuteReaderAsync(string sql, CommandType commandType = CommandType.Text,
            DataParameters parameters = null, CommandBehavior behavior = CommandBehavior.CloseConnection, CancellationToken token = default(CancellationToken));

var firstName = await db.QuerySingleAsync<string>("select firstname from users where email = '@Email', CommandType commandType = CommandType.Text, DataParameters parameters = new DataParameters {{"@Email","email@domain.com"}}, CommandBehavior behavior = CommandBehavior.CloseConnection, CancellationToken token = default(CancellationToken));
```
Execute a query and convert to POCO object
-----------------------------------------
```csharp
public class User
{
    public string FirstName { get; set; } //make sure property name is same as table column name, otherwise use alias in sql query.
    public string LastName { get; set; }
    public string Email { get; set; }
}

using(var db = new Database(new OracleConnection("connectionString")))
{
    List<User> q = db.Query<User>("SELECT EmailAddress as Email,LastName FROM Users_Test");
    Assert.IsTrue(q.Count > 0);
}
```
Execute a stored procedure and get multiple output parameter and return value.
-----------------------------------------------------------------------------
```csharp
 var parameters = new DataParameters
 {
    { "@Email", "user1@domain.com" },
    { "@RecordCount"}, //Default parameter direction is 'output'
    { "@AnotherOutParameter"},
    { "@ReturnVal",ParameterDirection.ReturnValue}
 };

  var q = db.Query<User>("PROC_NAME", CommandType.StoredProcedure, parameters);
  
  Assert.IsTrue(q.Count > 0);
  Assert.IsTrue(dataParameter.Value<int>("@RecordCount") > 0);
  Assert.IsTrue(dataParameter.Value<int>("@AnotherOutParameter") > 0);
  Assert.IsTrue(dataParameter.Value<int>("@ReturnVal") > 0);

```
Execute a command with DB transaction.
-------------------------------------
```csharp
using (var db = new Database(new SqlConnection(""),true)) //use transaction. Check overloads for IsolationLevel specific.
{
  try
  {
      db.ExecuteNonQuery("sql", parameters:new DataParameters{{"Email", "user@domain.com" } });
      db.ExecuteNonQuery("sql", parameters:new DataParameters{{"Email", "user@domain.com" } });
      
      db.CommitTransaction(); //commit transaction.
  }
  catch (Exception)
  {
      db.RollbackTransaction(); // rollback if any failure. 
  }
}
```

Execute multiple query using a single DB connection instance
------------------------------------------------------------
```csharp

using(var db = new Database(new SqlConnection("")))
{
  var reader1 = db.ExecuteReader("sql",CommandType.Text,parameters,CommandBehavior.Default);
  var reader2 = db.ExecuteReader("sql",CommandType.Text,parameters); //default behaviour is close connection
}

```
Create DB provider specific parameter object
--------------------------------------------
Data.Net support multiple overloads so that you can create DB specific parameter object like below.

```csharp
public void Add(IDbDataParameter parameter) {}  //For any Ado.Net specific provider
public void Add(string name, object value) {}
public void Add(string name, ParameterDirection direction = ParameterDirection.Output,DbType dbType = DbType.Int32, int size = 0) {}

```
Example usage

```csharp
var p = new DataParameter();
p.Add(new OracleParameter("name","value"));
p.Add("name","value");
p.Add("name",ParameterDirection.Output,DbType.Int32);
```

Please check out more samples in the [Unit test app](https://github.com/deepumi/Data.Net/tree/master/Data.Net.Test).
