using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

namespace ApplicationCore.Models
{
    public class StudentsClass : TableEntity
    {
        public StudentsClass(byte number, char letter)
        {
            Number = number;
            PartitionKey = number.ToString();
            Letter = letter;
            RowKey = letter.ToString();
            FullName = $"{number}{letter}";
        }

        public byte Number { get; set; }
        public char Letter { get; set; }
        public string FullName { get; set; }
        public Teacher Educator { get; set; }
        public IEnumerable<Student> Students { get; set; }
        public IEnumerable<Lesson> Lessons { get; set; }
    }
}
