using ApplicationCore.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ApplicationCore.Models
{
    public class Lesson
    {
        public Subject Subject { get; set; }
        [JsonIgnore]
        public Teacher Teacher { get; set; }
        public long? TeacherId { get; set; }
        public IEnumerable<LessonTerm> Terms { get; set; }
        public string Classroom { get; set; }
    }

    public class LessonTerm
    {
        public Day Day;
        public string Time;
    }
}
