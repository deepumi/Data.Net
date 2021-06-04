namespace Data.Net.Test.Oracle
{
    public class StudentInsert
    {
        [AutoIncrement(SequenceName = OracleSequenceHelper.SequenceName)]
        public long Id { get; set; }

        public string Name { get; set; }

        public string Gender { get; set; }
        
        public decimal Age { get; set; }
    }
}