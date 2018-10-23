using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class TimeTableService : ITimeTableService
    {
        private readonly ITableStorageRepository _repository;

        public TimeTableService(ITableStorageRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Lesson>> GetLessonsForGivenClassAsync(string classId)
        {
            string classNumber = classId[0].ToString();
            string classLetter = classId[1].ToString();
            var studentsClass = await _repository.GetAsync<StudentsClass>(classNumber, classLetter);
            if (studentsClass != null)
            {
                var lessons = JsonConvert.DeserializeObject<List<Lesson>>(studentsClass.SerializedLessons);
                return lessons;
            }
            else
            {
                return new List<Lesson>();
            }
        }

        public async Task<List<Lesson>> GetLessonsForGivenTeacherAsync(long? teacherId)
        {
            var teacher = await _repository.GetAsync<Teacher>(nameof(Teacher), teacherId.ToString());
            if (teacher != null)
            {
                var lessons = JsonConvert.DeserializeObject<List<Lesson>>(teacher.SerializedLessons);
                return lessons;
            }
            else
            {
                return null;
            }
        }
    }
}
