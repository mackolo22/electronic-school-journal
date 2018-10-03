using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Exceptions.AzureStorage;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;

namespace ApplicationCore.Services
{
    public class ClassService : IClassService
    {
        private readonly ITableStorageRepository _repository;

        public ClassService(ITableStorageRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> AddNewClassAsync(
            int classNumber, 
            string classLetter, 
            Teacher educator, 
            IEnumerable<Student> students, 
            IEnumerable<Lesson> lessons)
        {
            var studentsClass = new StudentsClass(classNumber, classLetter)
            {
                EducatorId = educator?.Id,
                Educator = educator,
                Students = students,
                Lessons = lessons
            };

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
                await _repository.InsertAsync(studentsClass.Educator);
                await _repository.InsertBatchAsync(studentsClass.Students);
                foreach (var student in studentsClass.Students)
                {
                    if (student.Parent != null)
                    {
                        await _repository.InsertAsync(student.Parent);
                    }
                }

                await _repository.InsertAsync(studentsClass);
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
