using ApplicationCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IClassesRepository
    {
        Task<StudentsClass> GetAsync(string partitionKey, string rowKey);
        Task<IEnumerable<StudentsClass>> GetAllAsync();
        Task InsertOrReplaceAsync(StudentsClass studentsClass);
    }
}
