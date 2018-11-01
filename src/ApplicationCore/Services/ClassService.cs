using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Exceptions.AzureStorage;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Newtonsoft.Json;

namespace ApplicationCore.Services
{
    public class ClassService : IClassService
    {
        private readonly ITableStorageRepository _repository;
        private readonly IMailingService _mailingService;

        public ClassService(ITableStorageRepository repository, IMailingService mailingService)
        {
            _repository = repository;
            _mailingService = mailingService;
        }

        public async Task<bool> AddNewClassAsync(
            Administrator administrator,
            int classNumber,
            string classLetter,
            Teacher educator,
            IEnumerable<Student> students,
            IEnumerable<Lesson> lessons)
        {
            // TODO: przypisać SerializedLessons każdemu nauczycielowi

            var studentsClass = new StudentsClass(classNumber, classLetter)
            {
                EducatorId = educator?.Id,
                Educator = educator,
                Students = students,
                Lessons = lessons
            };

            List<Teacher> teachersToUpdate = new List<Teacher>();
            foreach (var lesson in studentsClass.Lessons)
            {
                lesson.ClassName = studentsClass.FullName;
                teachersToUpdate.Add(lesson.Teacher);
            }

            teachersToUpdate = teachersToUpdate.Distinct().ToList();
            foreach (var teacher in teachersToUpdate)
            {
                teacher.SerializedLessons = JsonConvert.SerializeObject(teacher.Lessons);
                await _repository.InsertOrReplaceAsync(teacher);
            }

            string serializedLessons = string.Empty;
            if (lessons != null)
            {
                serializedLessons = JsonConvert.SerializeObject(lessons);
                studentsClass.SerializedLessons = serializedLessons;
            }

            if (studentsClass.Educator != null)
            {
                studentsClass.Educator.ClassId = studentsClass.FullName;
                studentsClass.Educator.Class = studentsClass;
            }

            foreach (var student in studentsClass.Students)
            {
                student.ClassId = studentsClass.FullName;
                student.Class = studentsClass;
            }

            try
            {
                await _repository.InsertOrReplaceAsync(studentsClass.Educator);
                await _repository.InsertBatchAsync(studentsClass.Students);
                foreach (var student in studentsClass.Students)
                {
                    await _mailingService.SendEmailWithLoginAndPasswordAsync(student, administrator);

                    if (student.Parent != null)
                    {
                        student.Parent.ChildClassId = studentsClass.FullName;
                        await _repository.InsertOrReplaceAsync(student.Parent);
                        await _mailingService.SendEmailWithLoginAndPasswordAsync(student.Parent, administrator);
                    }
                }

                await _repository.InsertOrReplaceAsync(studentsClass);
            }
            catch (TableException)
            {
                // TODO: wycofanie wszystkich zmian

                return false;
            }

            return true;
        }
    }
}
