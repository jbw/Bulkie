using Bulkie.API.Infrastructure;
using BulkieFileProcessor.API.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BulkieFileProcessor.API.Infrastructure
{
    public class FileReferenceRepository : IFileReferenceRepository
    {
        private readonly FileReferenceContext _fileReferenceContext;

        public FileReferenceRepository(FileReferenceContext fileReferenceContext)
        {
            _fileReferenceContext = fileReferenceContext;
        }

        public async Task<FileReference> FindOrCreate(string hash)
        {
            var fileReference = await _fileReferenceContext.FileReferences.FirstOrDefaultAsync(x => x.FileHash.Equals(hash));

            if (fileReference != null)
            {
                return fileReference;
            }

            var newFileReference = new FileReference
            {
                Id = Guid.NewGuid(),
                FileHash = hash
            };

            await _fileReferenceContext.AddAsync(fileReference);
            await _fileReferenceContext.SaveChangesAsync();

            return newFileReference;

        }
    }
}
