using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using VideoApp.Web.Models.ViewModels;

namespace VideoApp.Web.Services
{
    public interface IFileManagerService
    {
        Task<bool> DeleteTempFile(string fullPath);
        Task<string> SaveTempFile(IFormFile file, string fileName);

        Task<OutputFileModel> GetFile(string fileName);
    }
}
