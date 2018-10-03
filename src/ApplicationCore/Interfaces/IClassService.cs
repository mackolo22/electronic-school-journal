using ApplicationCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IClassService
    {
        Task<bool> AddNewClassAsync(int classNumber, string classLetter, Teacher educator, IEnumerable<Student> students, IEnumerable<Lesson> lessons);
    }
}
