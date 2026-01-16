using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Threading.Tasks;

namespace H4G_Project.Services
{
    public class FileService
    {
        public async Task<string> SaveFileLocally(IFormFile file)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/" + fileName;
        }
    }
}