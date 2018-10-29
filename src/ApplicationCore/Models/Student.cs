using ApplicationCore.Enums;
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
        [IgnoreProperty]
        public IList<Attendance> Attendances { get; set; }
        public string SerializedAttendances { get; set; }
        public long? ParentId { get; set; }
        [IgnoreProperty]
        public Parent Parent { get; set; }
    }

    public class Grade
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public string Comment { get; set; }
        public DateTime LastModificationDate { get; set; }
    }

    public class Attendance
    {
        public bool Presence { get; set; }
        public LessonTerm LessonTerm { get; set; }
        public DateTime Date { get; set; }
        public string Subject { get; set; }
    }
}
