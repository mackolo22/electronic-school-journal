using ApplicationCore.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ApplicationCore.Models
{
    public class Lesson
    {
        public Subject Subject { get; set; }
        public string ClassName { get; set; }
        [JsonIgnore]
        public Teacher Teacher { get; set; }
        public long? TeacherId { get; set; }
        public string Classroom { get; set; }
        //public List<LessonTerm> Terms { get; set; }
        public LessonTerm Term { get; set; }
        //[JsonIgnore]
        //public string TermsToString
        //{
        //    get
        //    {
        //        if (Terms != null && Terms.Any())
        //        {
        //            StringBuilder terms = new StringBuilder();
        //            foreach (var term in Terms)
        //            {
        //                terms.AppendLine($"{term.Day.GetDisplayName()}, g. {term.Time}");
        //            }

        //            return terms.ToString();
        //        }
        //        else
        //        {
        //            return "Brak terminów.";
        //        }
        //    }
        //}
    }

    public class LessonTerm
    {
        public Day Day { get; set; }
        public string Time { get; set; }
        public int LessonNumber { get; set; }

        public override string ToString()
        {
            string lesson = $"Lekcja {LessonNumber + 1} ({Time})";
            return lesson;
        }
    }
}
