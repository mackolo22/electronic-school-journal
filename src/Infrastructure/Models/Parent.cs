namespace ApplicationCore.Models
{
    public class Parent : Person
    {
        public Parent(long id) : base(id)
        {
            PartitionKey = nameof(Parent);
        }

        public Student Child { get; set; }
    }
}
