namespace ApplicationCore.Models
{
    public class Administrator : User
    {
        public Administrator() { }

        public Administrator(long id) : base(id)
        {
            PartitionKey = nameof(Administrator);
        }
    }
}
