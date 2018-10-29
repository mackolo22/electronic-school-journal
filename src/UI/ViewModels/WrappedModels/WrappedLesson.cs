using ApplicationCore.Models;
using System.Collections.Generic;

namespace UI.ViewModels.WrappedModels
{
    public class WrappedLesson
    {
        public string Subject { get; set; }
        public IEnumerable<LessonTerm> Terms { get; set; }
    }
}
