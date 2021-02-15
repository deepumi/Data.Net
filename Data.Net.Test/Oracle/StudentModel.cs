namespace Data.Net.Test.Oracle
{
    public abstract class StudentBase
    {
        [OracleSequence(SequenceName = BaseOracle.SequenceName)]
        public decimal Id { get; set; }

        public string Name { get; set; }

        public string Gender { get; set; }
        
        public decimal Age { get; set; }
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
