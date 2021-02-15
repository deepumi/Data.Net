namespace Data.Net.Test.PostgreSql
{
    public abstract class StudentBase
    {
        [AutoIncrement]
        [Column(Name = "Id")]
        public int StudentId { get; set; }

        public string Name { get; set; }

        public string Gender { get; set; }

        public int Age { get; set; }
    }
    
    public class StudentInsert : StudentBase
    {
    
    }
}
