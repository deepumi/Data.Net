namespace Data.Net.Test.SqlLite
{
    public abstract class Person
    {
        [AutoIncrement]
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public string Email { get; set; }
    }
    
    public sealed class PersonInsert : Person
    {
       
    }

    public sealed class PersonUpdate : Person
    {
        
    }
    
    public sealed class PersonDelete : Person
    {
        
    }

    public sealed class PersonQuery : Person
    {
        
    }

    public sealed class PersonReader : Person
    {
        
    }
}