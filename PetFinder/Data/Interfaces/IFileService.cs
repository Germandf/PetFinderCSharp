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
        Task<GenericResult<string>> UploadAsync(IFileListEntry file);
    }
}
