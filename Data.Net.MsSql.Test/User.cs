namespace Data.Net.MsSql.Test
{
    [TableName("Users_Test")]
    public class User
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
    }

    public class Address
    {
        public HelloEnum Active { get; set; }

        public string Name { get; set; }
    }

    public enum HelloEnum
    {
        Success = 1,
        Failed = 0,
        Something = 2
    }
}