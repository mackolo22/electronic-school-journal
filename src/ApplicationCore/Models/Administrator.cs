namespace ApplicationCore.Models
{
    public class Administrator : Person
    {
        public Administrator() { }

        public Administrator(long id) : base(id)
        {
            PartitionKey = nameof(Administrator);
        }
    }
}
