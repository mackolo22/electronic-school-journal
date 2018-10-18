using Microsoft.WindowsAzure.Storage.Table;

namespace ApplicationCore.Models
{
    public class Parent : Person
    {
        public Parent() { }

        public Parent(long id) : base(id)
        {
            PartitionKey = nameof(Parent);
        }

        public long? ChildId { get; set; }
        [IgnoreProperty]
        public Student Child { get; set; }
        public string ChildClassId { get; set; }
    }
}
