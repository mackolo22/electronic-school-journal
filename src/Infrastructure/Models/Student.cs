using System.Collections.Generic;

namespace ApplicationCore.Models
{
    public class Student : Person
    {
        public Student(long id) : base(id)
        {
            PartitionKey = nameof(Student);
        }

        public StudentsClass Class { get; set; }
        public IDictionary<Lesson, double> Grades { get; set; }
        public Parent Parent { get; set; }
    }
}
