using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetFinder.Areas.Identity;
using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    public class PetService : IPetService
    {
        const string ERROR_MISSING_GENDER   = "Debe especificar un género";
        const string ERROR_MISSING_CITY     = "Debe especificar una ciudad";
        const string ERROR_MISSING_TYPE     = "Debe especificar un tipo de animal";
        const string ERROR_INVALID_NAME     = "Asegúrese de insertar un nombre y que sea menor a 20 caracteres";
        const string ERROR_INVALID_PHOTO    = "Ocurrió un error al guardar la foto";
        const string ERROR_INVALID_USER     = "El usuario no puede editar esta mascota";
        const string ERROR_MISSING_PHONE    = "Debe indicar un número de teléfono";

        private readonly PetFinderContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUserService _applicationUserService;

        public PetService(PetFinderContext context)
        {
            _context = context;
        }
        public PetService(PetFinderContext context,
                          IApplicationUserService applicationUserService,
                          UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _applicationUserService = applicationUserService;
        }

        public async Task<bool> CurrUserCanEdit(Pet pet)
        {
            ApplicationUser currUser = await _applicationUserService.GetCurrent();
            if (currUser == null) return false; // No esta logeado

            bool isAdmin = await _userManager.IsInRoleAsync(currUser, ApplicationUserService.ROLE_ADMIN);
            if (isAdmin) return true; //Si es admin puede editar
            if(pet.UserId == currUser.Id) return true; // Si es suya puede editar
            return false; // No puede editar
        }

        public async Task<bool> Delete(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (await CurrUserCanEdit(pet))
            {
                _context.Pets.Remove(pet);
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
            return await _context.Pets.
                Include(p => p.AnimalType).
                Include(p => p.City).
                Include(p => p.Gender).
                FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Pet>> GetAll()
        {
            return await _context.Pets.
                Include(p => p.AnimalType).
                Include(p => p.City).
                Include(p => p.Gender).
                Where(p=> p.Found == 0).
                ToListAsync();
        }

        public async Task<IEnumerable<Pet>> GetAllByUser(string UserId)
        {
            return await _context.Pets.Where(p => p.UserId == UserId).ToListAsync();
        }

        public async Task<bool> Insert(Pet pet)
        {
            _context.Pets.Add(pet);
            return await _context.SaveChangesAsync() > 0;
        }

        public bool IsValidName(string name)
        {
            if (name == null)
            {
                return false;
            }
            else
            {
                bool isValid = name.Length > 0 && name.Length <= 20;
                return isValid;
            }
        }

        public List<string> CheckPet(Pet pet)
        {
            List<string> errorMessages = new List<string>();
            if (pet.GenderId == 0)
                errorMessages.Add(ERROR_MISSING_GENDER);
            if (pet.AnimalTypeId == 0)
                errorMessages.Add(ERROR_MISSING_TYPE);
            if (pet.CityId == 0)
                errorMessages.Add(ERROR_MISSING_CITY);
            if (String.IsNullOrEmpty(pet.PhoneNumber))
                errorMessages.Add(ERROR_MISSING_PHONE);
            if (!IsValidName(pet.Name))
                errorMessages.Add(ERROR_INVALID_NAME);
            if (pet.Photo == null)
                errorMessages.Add(ERROR_INVALID_PHOTO);
            return errorMessages;
        }

        public async Task<bool> Save(Pet pet, List<String> errorMessages)
        {
            errorMessages = CheckPet(pet);
            if (errorMessages.Count > 0) return false;
            ApplicationUser appUser = await _applicationUserService.GetCurrent();
            if (appUser == null)
            {
                errorMessages.Add(ERROR_INVALID_USER);
                return false;
            }
            pet.UserId = appUser.Id;
            if (pet.Id > 0) return await Update(pet, errorMessages);
            return await Insert(pet);
        }

        public async Task<bool> Update(Pet pet, List<string> errorMessages)
        {
            if (!await CurrUserCanEdit(pet))
            {
                errorMessages.Add(ERROR_INVALID_USER);
                return false;
            }
            _context.Entry(pet).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Pet>> GetAllByFilter(params string[] args)
        {
            string city = null, animalType = null, gender = null;
            foreach (string arg in args)
            {
                if(arg == null)
                {
                    // Do nothing
                }
                else if (arg.Contains("ciudad-"))
                {
                    city = arg.Replace("ciudad-", "");
                    if (city.Length == 0) city = null;
                }
                else if (arg.Contains("tipo-"))
                {
                    animalType = arg.Replace("tipo-", "");
                    if (animalType.Length == 0) animalType = null;
                }
                else if (arg.Contains("genero-"))
                {
                    gender = arg.Replace("genero-", "");
                    if (gender.Length == 0) gender = null;
                }
            }
            return await SearchByFilter(city, animalType, gender);
        }

        private async Task<IEnumerable<Pet>> SearchByFilter(string city, string animalType, string gender)
        {
            return await _context.Pets.
                Where(p => p.City.SerializedName == city || city == null).
                Where(p => p.AnimalType.SerializedName == animalType || animalType == null).
                Where(p => p.Gender.SerializedName == gender || gender == null).
                Include(p => p.AnimalType).
                Include(p => p.City).
                Include(p => p.Gender).
                ToListAsync();
        }
    }
}
