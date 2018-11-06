using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

namespace ApplicationCore.Models
{
    public class Teacher : User
    {
        public Teacher() { }

        public Teacher(long id) : base(id)
        {
            PartitionKey = nameof(Teacher);
        }

        public string ClassId { get; set; }
        [IgnoreProperty]
        public StudentsClass Class { get; set; }
        [IgnoreProperty]
        public List<Lesson> Lessons { get; set; }
        public string SerializedLessons { get; set; }
    }
}
