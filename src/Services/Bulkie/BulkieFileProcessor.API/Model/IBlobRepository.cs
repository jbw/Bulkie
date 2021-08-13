using System.Threading.Tasks;

namespace BulkieFileProcessor.API.Model
{
    public interface IBlobRepository
    {
        Task Add(string name, string filePath);
        Task<bool> Exists(string name);
    }
}