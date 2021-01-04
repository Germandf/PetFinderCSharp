using BlazorInputFile;
using Microsoft.AspNetCore.Hosting;
using PetFinder.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    public class FileService : IFileService
    {
        private static readonly List<string> ImageTypes = new List<string> { "image/jpg", "image/jpeg", "image/png" };
        private static readonly string ImagesPath = "wwwroot/images";

        const string EMPTY_FILE_ERROR = "Debe elegir una imagen";
        const string INVALID_FILE_TYPE = "El tipo de archivo que intenta subir es invalido. Debe ser JPG, JPEG o PNG";

        private readonly IWebHostEnvironment _environment;
        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        private void CreateImagesFolder()
        {
            string absolutePath = _environment.ContentRootPath + "/" + ImagesPath; // La ruta absoluta del sitio más la ruta de las imagenes
            bool exists = Directory.Exists(absolutePath);
            if (!exists) // Si el directorio no existe lo creo
                Directory.CreateDirectory(absolutePath);
        }

        public async Task<GenericResult<string>> UploadPetPhotoAsync(IFileListEntry fileEntry)
        {
            GenericResult<string> result = new GenericResult<string>();
            CreateImagesFolder(); // Creo el directorio donde voy a guardar las imagenes
            if (fileEntry == null)
            {
                result.AddError(EMPTY_FILE_ERROR);
                return result;
            }
            string fileType = fileEntry.Type;
            if (ImageTypes.Contains(fileType))
            {
                string fileExtension = fileType.Replace("image/", string.Empty);
                var uniqueFileName = string.Format(@"{0}.{1}", Guid.NewGuid(), fileExtension);

                var path = Path.Combine(_environment.ContentRootPath, "wwwroot/images", uniqueFileName);
                var ms = new MemoryStream();
                await fileEntry.Data.CopyToAsync(ms);
                using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    ms.WriteTo(file);
                }
                result.value = uniqueFileName;
            }
            else result.AddError(INVALID_FILE_TYPE);
            return result;
        }
    }
}
