using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;

namespace ApplicationCore.Models
{
    public class Student : Person
    {
        public Student() { }

        public Student(long id) : base(id)
        {
            PartitionKey = nameof(Student);
        }

        public string ClassId { get; set; }
        [IgnoreProperty]
        public StudentsClass Class { get; set; }
        [IgnoreProperty]
        public IDictionary<string, List<Grade>> Grades { get; set; }
        public string SerializedGrades { get; set; }
        public long? ParentId { get; set; }
        [IgnoreProperty]
        public Parent Parent { get; set; }
    }

    public class Grade
    {
        int Id { get; set; }
        public double Value { get; set; }
        public string Comment { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
