using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

namespace ApplicationCore.Models
{
    public class Student : Person
    {
        public Student(long id) : base(id)
        {
            PartitionKey = nameof(Student);
        }

        public string ClassId { get; set; }
        [IgnoreProperty]
        public StudentsClass Class { get; set; }
        [IgnoreProperty]
        public IDictionary<Lesson, double> Grades { get; set; }
        public string SerializedGrades { get; set; }
        public long? ParentId { get; set; }
        [IgnoreProperty]
        public Parent Parent { get; set; }
    }
}
