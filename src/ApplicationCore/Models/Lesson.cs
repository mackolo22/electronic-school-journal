using ApplicationCore.Enums;
using ApplicationCore.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
namespace ApplicationCore.Models
{
    public class Lesson
    {
        public Subject Subject { get; set; }
        public string ClassName { get; set; }
        [JsonIgnore]
        public Teacher Teacher { get; set; }
        public long? TeacherId { get; set; }
        public IEnumerable<LessonTerm> Terms { get; set; }
        [JsonIgnore]
        public string TermsToString
        {
            get
            {
                if (Terms != null && Terms.Any())
                {
                    StringBuilder terms = new StringBuilder();
                    foreach (var term in Terms)
                    {
                        terms.AppendLine($"{term.Day.GetDisplayName()}, g. {term.Time}");
                    }

                    return terms.ToString();
                }
                else
                {
                    return "Brak terminów.";
                }
            }
        }

        public string Classroom { get; set; }
    }

    public class LessonTerm
    {
        public Day Day;
        public string Time;
        public int TimeId;
    }
}
