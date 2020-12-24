using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetFinder.Areas.Identity;
using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    public class PetService : IPetService
    {
        const string ERROR_MISSING_GENDER = "Debe especificar un genero";
        const string ERROR_MISSING_CITY = "Debe especificar una ciudad";
        const string ERROR_MISSING_TYPE = "Debe especificar un tipo de animal";
        const string ERROR_INVALID_NAME = "Asegúrese de insertar un nombre y que sea menor a 20 caracteres";
        const string ERROR_INVALID_PHOTO = "Ocurrió un error al guardar la foto";
        const string ERROR_INVALID_USER = "El usuario no puede editar está mascota";
        const string ERROR_MISSING_PHONE = "Debe indicar un numéro de telefono";

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

        

        public async Task<bool> currUserCanEdit(Pet pet)
        {
            ApplicationUser currUser = await _applicationUserService.GetCurrent();

            if (currUser == null) {
                return false;
            }

            bool isAdmin = await _userManager.IsInRoleAsync(currUser, ApplicationUserService.ROLE_ADMIN);
            if (isAdmin)
            {
                return true; //Si es admin puede editar cualquier cosa
            }

            if(pet.UserId == currUser.Id)
            {
                return true;
            }

            return false; 
        }

        public async Task<bool> Delete(int id)
        {

            var pet = await _context.Pets.FindAsync(id);
            if (await currUserCanEdit(pet))
            {
                _context.Pets.Remove(pet);
            }
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> SetFound(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (await currUserCanEdit(pet))
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

        public List<string> checkPet(Pet pet)
        {
            List<string> errorMessages = new List<string>();
            if (pet.GenderId == 0)
            {
                errorMessages.Add(ERROR_MISSING_GENDER);
            }

            if (pet.AnimalTypeId == 0)
            {
                errorMessages.Add(ERROR_MISSING_TYPE);
            }

            if (pet.CityId == 0)
            {
                errorMessages.Add(ERROR_MISSING_CITY);
            }

            if (String.IsNullOrEmpty(pet.PhoneNumber))
            {
                errorMessages.Add(ERROR_MISSING_PHONE);
            }

            if (!IsValidName(pet.Name))
            {
                errorMessages.Add(ERROR_INVALID_NAME);
            }

            if (pet.Photo == null)
            {
                errorMessages.Add(ERROR_INVALID_PHOTO);
            }


            return errorMessages;
        }
        public async Task<bool> Save(Pet pet, List<String> errorMessages)
        {
            errorMessages = checkPet(pet);
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
            if (!await currUserCanEdit(pet))
            {
                errorMessages.Add(ERROR_INVALID_USER);
                return false;
            }
            _context.Entry(pet).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
