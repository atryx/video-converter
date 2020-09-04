using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace VideoApp.Web.Services
{
    public interface IFileManagerService
    {
        Task<bool> DeleteTempFile(string fullPath);
        Task<string> SaveTempFile(IFormFile file, string fileName);
        Task<FileStream> GetFile(string fileName);
    }
}
