using ApplicationCore.Models;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IClassesRepository
    {
        Task<StudentsClass> GetAsync(string partitionKey, string rowKey);
        Task InsertOrReplaceAsync(StudentsClass studentsClass);
    }
}
