using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

namespace ApplicationCore.Models
{
    public class StudentsClass : TableEntity
    {
        public StudentsClass() { }

        public StudentsClass(int number, string letter)
        {
            Number = number;
            PartitionKey = number.ToString();
            Letter = letter;
            RowKey = letter.ToString();
            Id = $"{number}{letter}";
            FullName = Id;
        }

        public string Id { get; set; }
        [IgnoreProperty]
        public int Number { get; set; }
        [IgnoreProperty]
        public string Letter { get; set; }
        public string FullName { get; set; }
        public long? EducatorId { get; set; }
        [IgnoreProperty]
        public Teacher Educator { get; set; }
        public IEnumerable<long> StudentsIds { get; set; }
        [IgnoreProperty]
        public IEnumerable<Student> Students { get; set; }
        [IgnoreProperty]
        public List<Lesson> Lessons { get; set; }
        public string SerializedLessons { get; set; }
    }
}
