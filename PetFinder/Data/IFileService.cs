using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BlazorInputFile;
using Microsoft.AspNetCore.Hosting;
using PetFinder.Helpers;

namespace PetFinder.Data
{
    public interface IFileService
    {
        /// <summary>
        ///     Uploads the pet's photo to the server's folder
        /// </summary>
        /// <returns>
        ///     The photo's unique name if it was uploaded successfully or a list of errors in case it was not
        /// </returns>
        Task<GenericResult<string>> UploadPetPhotoAsync(IFileListEntry file);
    }

    public class FileService : IFileService
    {
        public const string EmptyFileError =
            "Debe elegir una imagen";

        public const string InvalidFileType =
            "El tipo de archivo que intenta subir es invalido. Debe ser JPG, JPEG o PNG";

        private static readonly List<string> ImageTypes = new() {"image/jpg", "image/jpeg", "image/png"};
        private static readonly string ImagesPath = "wwwroot/images";

        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<GenericResult<string>> UploadPetPhotoAsync(IFileListEntry fileEntry)
        {
            var result = new GenericResult<string>();
            CreateImagesFolder(); // Creo el directorio donde voy a guardar las imagenes
            if (fileEntry == null)
            {
                result.AddError(EmptyFileError);
                return result;
            }

            var fileType = fileEntry.Type;
            if (ImageTypes.Contains(fileType))
            {
                var fileExtension = fileType.Replace("image/", string.Empty);
                var uniqueFileName = string.Format(@"{0}.{1}", Guid.NewGuid(), fileExtension);
                var path = Path.Combine(_environment.ContentRootPath, "wwwroot/images", uniqueFileName);
                var ms = new MemoryStream();
                await fileEntry.Data.CopyToAsync(ms);
                await using (var file = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    ms.WriteTo(file);
                }

                result.value = uniqueFileName;
            }
            else
            {
                result.AddError(InvalidFileType);
            }

            return result;
        }

        private void CreateImagesFolder()
        {
            var absolutePath =
                _environment.ContentRootPath + "/" +
                ImagesPath; // La ruta absoluta del sitio más la ruta de las imagenes
            var exists = Directory.Exists(absolutePath);
            if (!exists) // Si el directorio no existe lo creo
                Directory.CreateDirectory(absolutePath);
        }
    }
}