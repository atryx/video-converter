using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;
using VideoApp.Web.Models.ViewModels;

namespace VideoApp.Web.Services
{
    public class FileManagerService : IFileManagerService
    {
        private readonly  IHostEnvironment _hostingEnvironment;

        public FileManagerService(IHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<bool> DeleteTempFile(string fullPath)
        {
            try
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return await Task.FromResult(true);
                }
                return await Task.FromResult(false);
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleteing TEMP file: " + ex.Message);
            }
        }     

        public async Task<string> SaveTempFile(IFormFile file, string fileName)
        {
            var uploads = Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads");
            if (file.Length > 0)
            {
                var filePath = Path.Combine(uploads, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                return filePath;
            }
            else
            {
                throw new Exception("file must not be empty");
            }
        }

        public async Task<OutputFileModel> GetFile(string fileName)
        {
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads", fileName);
            var contentType = GetContentType(fileName);
            var fileStream = ReadFile(filePath);
            var outputFile = new OutputFileModel
            {
                FileName = fileName,
                FileContents = fileStream,
                ContentType = contentType
            };
            return await Task.FromResult(outputFile);
        }

        private byte[] ReadFile(string filePath)
        {
            byte[] buffer;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                int count;                            // actual number of bytes read
                int sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;  // sum is a buffer offset for next reading
            }
            finally
            {
                fileStream.Close();
            }
            return buffer;
        }

        private string GetContentType(string filename)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(filename, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
