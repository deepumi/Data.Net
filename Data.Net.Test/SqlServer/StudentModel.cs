namespace Data.Net.Test.SqlServer
{
    public abstract class StudentBase
    {
        [AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Gender { get; set; }
        
        public int Age { get; set; }
    }
    
    public class StudentInsert : StudentBase
    {
        
    }
    
    public class StudentQuery : StudentBase
    {
     
    }
    
    public class StudentUpdate : StudentBase
    {
        
    }
    
    public class StudentDelete : StudentBase
    {
        
    }
}
