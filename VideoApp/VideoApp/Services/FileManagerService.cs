﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

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

        public void CreateVideoDirectory(string directoryName)
        {
            var fullpath =  Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads", directoryName);
            if (!Directory.Exists(fullpath))
            {
                Directory.CreateDirectory(fullpath);
            }

        }

        public async Task<FileStream> GetFile(string fileName)
        {
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads", fileName);
            return await Task.FromResult(new FileStream(filePath, FileMode.Open, FileAccess.Read));            
        }

        public string GetParentDirectory(string filename)
        {
            return Directory.GetParent(filename).Name;
        }
    }
}
