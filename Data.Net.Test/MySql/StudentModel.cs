namespace Data.Net.Test.MySql
{
    public abstract class StudentBase
    {
        [AutoIncrement]
        [Column(Name = "Id")]
        public ulong StudentId { get; set; }

        public string Name { get; set; }

        public string Gender { get; set; }

        public int Age { get; set; }
    }
    
    public class StudentInsert : StudentBase
    {
       
    }
    
    public class StudentUpdate : StudentBase
    {
      
    }
    
    public class StudentDelete : StudentBase
    {
      
    }
    
    public class StudentQuery : StudentBase
    {
      
    }
}