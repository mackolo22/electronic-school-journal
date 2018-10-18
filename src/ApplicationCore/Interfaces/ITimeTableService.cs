using ApplicationCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface ITimeTableService
    {
        Task<List<Lesson>> GetLessonsForGivenClassAsync(string classId);
        Task<List<Lesson>> GetLessonsForGivenTeacherAsync(long? teacherId);
    }
}
