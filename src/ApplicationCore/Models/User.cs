using Microsoft.WindowsAzure.Storage.Table;

namespace ApplicationCore.Models
{
    public class User : TableEntity
    {
        public User() { }

        public User(long id)
        {
            Id = id;
            RowKey = id.ToString();
        }

        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [IgnoreProperty]
        public string FullName { get => $"{FirstName} {LastName}"; }
        public string Email { get; set; }
        public string Login { get; set; }
        [IgnoreProperty]
        public string Password { get; set; }
        public string HashedPassword { get; set; }
    }
}
