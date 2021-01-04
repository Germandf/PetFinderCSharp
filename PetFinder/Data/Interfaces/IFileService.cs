using BlazorInputFile;
using PetFinder.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    public interface IFileService
    {
        /// <summary>
        /// Uploads the pet's photo to the server's folder
        /// </summary>
        /// <returns>
        /// The photo's unique name if it was uploaded successfully or a list of errors in case it was not
        /// </returns>
        Task<GenericResult<string>> UploadPetPhotoAsync(IFileListEntry file);
    }
}
