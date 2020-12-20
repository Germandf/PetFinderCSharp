using BlazorInputFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    interface IFileService
    {
        Task<string> UploadAsync(IFileListEntry file);
    }
}
