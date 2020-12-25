﻿using BlazorInputFile;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    public class FileService : IFileService
    {
        public static readonly List<string> ImageTypes = new List<string> { "image/jpg", "image/jpeg", "image/png" };
        private readonly IWebHostEnvironment _environment;
        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public async Task<string> UploadAsync(IFileListEntry fileEntry)
        {

            if(fileEntry == null)
            {
                return null;
            }
            string fileType = fileEntry.Type;

            if (ImageTypes.Contains(fileType))
            {
                string fileExtension = fileType.Replace("image/", string.Empty);
                var myUniqueFileName = string.Format(@"{0}.{1}", Guid.NewGuid(), fileExtension);

                var path = Path.Combine(_environment.ContentRootPath, "wwwroot/images", myUniqueFileName);
                var ms = new MemoryStream();
                await fileEntry.Data.CopyToAsync(ms);
                using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    ms.WriteTo(file);
                }
                return myUniqueFileName;
            }

            return null;
        }
    }
}