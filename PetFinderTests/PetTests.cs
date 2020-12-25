using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetFinder.Areas.Identity;
using PetFinder.Data;
using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PetFinderTests
{
    public class PetTests
    {
        private PetFinderDbContextFactory dbContextFactory { get; set; }
        public PetTests()
        {
            dbContextFactory = new PetFinderDbContextFactory();
        }
        public Pet CreatePet(string name, int animalTypeId, int cityId, int genderId, DateTime date, string phoneNumber, string photo, string description, string userId, byte found)
        {
            var auxPet = new Pet();
            auxPet.Name = name;
            auxPet.AnimalTypeId = animalTypeId;
            auxPet.CityId = cityId;
            auxPet.GenderId = genderId;
            auxPet.Date = date;
            auxPet.PhoneNumber = phoneNumber;
            auxPet.Photo = photo;
            auxPet.Description = description;
            auxPet.UserId = userId;
            auxPet.Found = found;
            return auxPet;
        }
        public City CreateCity(string name)
        {
            var auxCity = new City();
            auxCity.Name = name;
            return auxCity;
        }
        public AnimalType CreateAnimalType(string name)
        {
            var auxAnimalType = new AnimalType();
            auxAnimalType.Name = name;
            return auxAnimalType;
        }
        public Gender CreateGender(string name)
        {
            var auxGender = new Gender();
            auxGender.Name = name;
            return auxGender;
        }

        [Fact]
        public void ShouldBeMissingDataAsync()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            Pet pet = new Pet();
            PetService petService = new PetService(context);
            List<string> errors = petService.CheckPet(pet);
            //Deberia dar 6 errores
            Assert.Equal<int>(6, errors.Count);
        }

        [Fact]
        public void ShouldBeInvalidName()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            PetService petService = new PetService(context);
            string invalidName = "asdasdasdasdasdasdasd";
            bool isValid = petService.IsValidName(invalidName);
            // Deberia ser falso ya que la cadena tiene 21 caracteres, siendo el maximo 20
            Assert.False(isValid);
        }
    }
}
