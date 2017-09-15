# Data.Net
A simple, light weight data access library for any ADO.Net providers like SQL server,Oracle &amp; MySQL.

Execute a query and return a single value from the first row
-----------------------------------------
```csharp
using (var db = new Database()) //make sure connection string is configured in the config file with <connectionStrings><clear/><add../></connectionStrings>
{
  var firstName = db.ExecuteScalar<string>("SELECT FirstName From Users_Test Where Email = @Email",
                  parameters:new DataParameters{{"Email", "user@domain.com" } });
  Assert.IsTrue(!string.IsNullOrWhiteSpace(firstName));
}
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
Execute a stored procedure and get multiple output parameter with return value.
-----------------------------------------
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
Execute a command with Db transaction.
----------------------------------
```csharp
using (var db = new Database())
{
  try
  {
      db.BeginTransaction(); //create transaction.
    
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


