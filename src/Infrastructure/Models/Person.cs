using Microsoft.WindowsAzure.Storage.Table;

namespace ApplicationCore.Models
{
    public class Person : TableEntity
    {

        public Person(long id)
        {
            Id = id;
            RowKey = id.ToString();
        }

        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get => $"{FirstName} {LastName}"; }
        public User User { get; set; }
    }
}
