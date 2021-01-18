using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BlazorInputFile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetFinder.Areas.Identity;
using PetFinder.Helpers;
using PetFinder.Models;
using PetFinder.ViewModels;
using Serilog;

namespace PetFinder.Data
{
    public interface IPetService
    {
        /// <summary>
        ///     Gets all Pets with their referenced City, AnimalType and Gender
        /// </summary>
        /// <returns>
        ///     An IEnumerable of type Pet that each one includes City, AnimalType and Gender
        /// </returns>
        Task<IEnumerable<Pet>> GetAll();

        /// <summary>
        ///     Gets a specific Pet with its referenced City, AnimalType and Gender by its Id
        /// </summary>
        /// <returns>
        ///     A Pet object that includes City, AnimalType and Gender
        /// </returns>
        Task<Pet> Get(int id);

        /// <summary>
        ///     Inserts a Pet
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Insert(Pet pet);

        /// <summary>
        ///     Updates a Pet
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it was successfull or not
        /// </returns>
        Task<GenericResult> Update(Pet pet);

        /// <summary>
        ///     Deletes a Pet
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Delete(int id);

        /// <summary>
        ///     Inserts or Updates a Pet depending on the case, it also takes care of uploading the photo if necessary
        /// </summary>
        /// <returns>
        ///     A GenericResult that indicates if it was successfull or not, if not, it will contain the error/s
        /// </returns>
        Task<GenericResult> Save(Pet pet, IFileListEntry photo);

        /// <summary>
        ///     Transforms the PetViewModel into a Pet and calls Save with the Pet parameter
        /// </summary>
        /// <returns>
        ///     A GenericResult that indicates if it was successfull or not, if not, it will contain the error/s
        /// </returns>
        Task<GenericResult> Save(PetViewModel petViewModel, IFileListEntry photo);

        /// <summary>
        ///     Looks for all Pets that contains the same UserId attribute as the one inserted
        /// </summary>
        /// <returns>
        ///     An IEnumerable of type Pet that each one includes City, AnimalType and Gender
        /// </returns>
        Task<IEnumerable<Pet>> GetAllByUser(string UserId);

        /// <summary>
        ///     Looks for all Pets that matches with the filters inserted, these can be single or multiple
        /// </summary>
        /// <returns>
        ///     An IEnumerable of type Pet that each one includes City, AnimalType and Gender
        /// </returns>
        Task<IEnumerable<Pet>> GetAllByFilter(params string[] filters);

        /// <summary>
        ///     Checks if the current user in session has permissions on this Pet
        /// </summary>
        /// <returns>
        ///     A bool that indicates if he has permissions or not
        /// </returns>
        Task<bool> CurrUserCanEdit(Pet pet);

        /// <summary>
        ///     Sets the Found attribute of the Pet object as true
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> SetFound(int id);
    }

    public class PetService : IPetService
    {
        public const string ErrorMissingGender = "Debe especificar un género";
        public const string ErrorMissingCity = "Debe especificar una ciudad";
        public const string ErrorMissingType = "Debe especificar un tipo de animal";
        public const string ErrorInvalidName = "Asegúrese de insertar un nombre y que sea menor a 20 caracteres";
        public const string ErrorInvalidPhoto = "Debe elegir un tipo de imagen valida";
        public const string ErrorInvalidUser = "El usuario no puede editar esta mascota";
        public const string ErrorMissingPhone = "Debe indicar un número de teléfono";
        public const string ErrorSaving = "Ocurrió un error al guardar el usuario";

        private readonly IApplicationUserService _applicationUserService;
        private readonly PetFinderContext _context;
        private readonly IFileService _fileService;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public PetService(PetFinderContext context)
        {
            _context = context;
        }

        public PetService(PetFinderContext context,
            ILogger logger,
            IApplicationUserService applicationUserService,
            UserManager<ApplicationUser> userManager,
            IFileService fileService)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _applicationUserService = applicationUserService;
            _fileService = fileService;
        }

        public async Task<bool> CurrUserCanEdit(Pet pet)
        {
            var currUser = await _applicationUserService.GetCurrent();
            if (currUser == null) return false; // No esta logeado
            var isAdmin = await _userManager.IsInRoleAsync(currUser, ApplicationUserService.ROLE_ADMIN);
            if (isAdmin) return true; //Si es admin puede editar
            if (pet.UserId == currUser.Id) return true; // Si es suya puede editar
            return false; // No puede editar
        }

        public async Task<bool> Delete(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (await CurrUserCanEdit(pet))
            {
                _context.Pets.Remove(pet);
                _logger.Warning("Pet {name} deleted, Id: {id}", pet.Name, pet.Id);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SetFound(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (await CurrUserCanEdit(pet))
            {
                pet.Found = 1;
                _context.Entry(pet).State = EntityState.Modified;
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Pet> Get(int id)
        {
            var pet = await _context.Pets.Include(p => p.AnimalType).Include(p => p.City).Include(p => p.Gender)
                .FirstOrDefaultAsync(i => i.Id == id);
            _context.Entry(pet).State = EntityState.Detached;
            return pet;
        }

        public async Task<IEnumerable<Pet>> GetAll()
        {
            return await _context.Pets.Include(p => p.AnimalType).Include(p => p.City).Include(p => p.Gender)
                .Where(p => p.Found == 0).ToListAsync();
        }

        public async Task<IEnumerable<Pet>> GetAllByUser(string UserId)
        {
            return await _context.Pets.Include(p => p.AnimalType).Include(p => p.City).Include(p => p.Gender)
                .Where(p => p.Found == 0 && p.UserId == UserId).ToListAsync();
        }

        public async Task<bool> Insert(Pet pet)
        {
            _context.Pets.Add(pet);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<GenericResult> Save(PetViewModel petViewModel, IFileListEntry photo)
        {
            var pet = petViewModel.ConvertToPet();
            return await Save(pet, photo);
        }

        public async Task<GenericResult> Save(Pet pet, IFileListEntry photo)
        {
            var result = new GenericResult();
            var appUser = await _applicationUserService.GetCurrent();
            if (appUser == null)
            {
                result.AddError(ErrorInvalidUser);
                return result;
            }

            pet.UserId = appUser.Id;
            if (photo != null) // Puede ser que la imagen sea nula porque estemos editando y no cambiamos la foto
            {
                var resultImage = await _fileService.UploadPetPhotoAsync(photo);
                if (resultImage.Success)
                    pet.Photo = resultImage.value; // Si la imagen se subio bien le asignamos la url a la mascota
                else result.AddRange(resultImage.Errors);
            }

            result.Errors.AddRange(CheckPet(pet)); // Si devuelve errores los agrego para mostrarlos
            if (result.Success) // Si no hay errores guardo o actualizo
            {
                if (pet.Id > 0) // Si estamos editando
                    result.AddRange((await Update(pet)).Errors);
                else if (!await Insert(pet)) // Si estamos guardando
                    result.AddError(ErrorSaving);
            }

            return result;
        }

        public async Task<GenericResult> Update(Pet pet)
        {
            var result = new GenericResult();
            if (await CurrUserCanEdit(pet))
            {
                _context.Entry(pet).State = EntityState.Modified;
                if(pet.Photo == null)
                    _context.Entry(pet).Property(x => x.Photo).IsModified = false;
                if (await _context.SaveChangesAsync() > 0)
                    return result;
                result.AddError(ErrorSaving);
            }
            else
            {
                result.AddError(ErrorInvalidUser);
            }

            return result;
        }

        public async Task<IEnumerable<Pet>> GetAllByFilter(params string[] filters)
        {
            string city = null, animalType = null, gender = null, search = null;
            foreach (var filter in filters)
                if (filter == null)
                {
                    // Do nothing
                }
                else if (filter.Contains("ciudad-"))
                {
                    city = filter.Replace("ciudad-", "").ToUpper();
                    if (city.Length == 0) city = null;
                }
                else if (filter.Contains("tipo-"))
                {
                    animalType = filter.Replace("tipo-", "").ToUpper();
                    if (animalType.Length == 0) animalType = null;
                }
                else if (filter.Contains("genero-"))
                {
                    gender = filter.Replace("genero-", "").ToUpper();
                    if (gender.Length == 0) gender = null;
                }
                else if (filter.Contains("contiene-"))
                {
                    search = filter.Replace("contiene-", "");
                    search = HttpUtility.UrlDecode(search);
                    if (search.Length == 0) search = null;
                }

            return await SearchByFilter(city, animalType, gender, search);
        }

        public bool IsValidName(string name)
        {
            if (name == null)
            {
                return false;
            }

            var isValid = name.Length > 0 && name.Length <= 20;
            return isValid;
        }

        public List<string> CheckPet(Pet pet)
        {
            var errorMessages = new List<string>();
            if (pet.GenderId == 0)
                errorMessages.Add(ErrorMissingGender);
            if (pet.AnimalTypeId == 0)
                errorMessages.Add(ErrorMissingType);
            if (pet.CityId == 0)
                errorMessages.Add(ErrorMissingCity);
            if (string.IsNullOrEmpty(pet.PhoneNumber))
                errorMessages.Add(ErrorMissingPhone);
            if (!IsValidName(pet.Name))
                errorMessages.Add(ErrorInvalidName);
            if (pet.Photo == null && pet.Id == 0)
                errorMessages.Add(ErrorInvalidPhoto);
            return errorMessages;
        }

        private async Task<IEnumerable<Pet>> SearchByFilter(string city, string animalType, string gender,
            string search)
        {
            return await _context.Pets.Where(p => p.City.SerializedName == city || city == null)
                .Where(p => p.AnimalType.SerializedName == animalType || animalType == null)
                .Where(p => p.Gender.SerializedName == gender || gender == null).Where(p =>
                    EF.Functions.FreeText(p.Name, search) || EF.Functions.FreeText(p.Description, search)
                                                          || search == null
                ).Include(p => p.AnimalType).Include(p => p.City).Include(p => p.Gender).ToListAsync();
        }
    }
}