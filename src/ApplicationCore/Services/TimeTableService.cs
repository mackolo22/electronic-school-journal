using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class TimeTableService : ITimeTableService
    {
        private readonly IClassesRepository _classesRepository;
        private readonly IUsersRepository _usersRepository;

        public TimeTableService(IClassesRepository classesRepository, IUsersRepository usersRepository)
        {
            _classesRepository = classesRepository;
            _usersRepository = usersRepository;
        }

        public async Task<List<Lesson>> GetLessonsForGivenClassAsync(string classId)
        {
            string classNumber = classId[0].ToString();
            string classLetter = classId[1].ToString();
            var studentsClass = await _classesRepository.GetAsync(classNumber, classLetter);
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
            var user = await _usersRepository.GetAsync(nameof(Teacher), teacherId.ToString());
            if (user != null)
            {
                Teacher teacher = user as Teacher;
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
