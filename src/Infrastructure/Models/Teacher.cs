namespace ApplicationCore.Models
{
    public class Teacher : Person
    {
        public Teacher(long id) : base(id)
        {
            PartitionKey = nameof(Teacher);
        }

        public StudentsClass Class { get; set; }
    }
}
