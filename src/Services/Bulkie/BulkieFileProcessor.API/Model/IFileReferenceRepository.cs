using System.Threading.Tasks;

namespace BulkieFileProcessor.API.Model
{
    public interface IFileReferenceRepository
    {
        Task<FileReference> FindOrCreate(string hash);
    }
}
