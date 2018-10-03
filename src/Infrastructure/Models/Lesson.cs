using ApplicationCore.Enums;
using System;
using System.Collections.Generic;

namespace ApplicationCore.Models
{
    public class Lesson
    {
        public Subject Subject { get; set; }
        public Teacher Teacher { get; set; }
        public IEnumerable<LessonTerm> Terms { get; set; }
    }

    public class LessonTerm
    {
        public Day Day;
        public TimeSpan Time;
    }
}
