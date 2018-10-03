using Microsoft.WindowsAzure.Storage.Table;

namespace ApplicationCore.Models
{
    public class Teacher : Person
    {
        public Teacher(long id) : base(id)
        {
            PartitionKey = nameof(Teacher);
        }

        public string ClassId { get; set; }
        [IgnoreProperty]
        public StudentsClass Class { get; set; }
    }
}
