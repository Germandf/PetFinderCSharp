using Microsoft.EntityFrameworkCore;
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
        public Pet CreatePet(string name, int animalTypeId, int cityId, int genderId, DateTime date, string phoneNumber, string photo, string description, int userId, byte found)
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
        public async Task ShouldInsertAsync()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            Pet pet = CreatePet("Toto", 1, 1, 1, new DateTime(2015, 12, 25), "2983458324", "photo_url", "description", 1, 0);
            City city = CreateCity("Tres Arroyos");
            AnimalType animalType = CreateAnimalType("Gato");
            PetService petService = new PetService(context);
            CityService cityService = new CityService(context);
            AnimalTypeService animalTypeService = new AnimalTypeService(context);

            await cityService.Save(city);
            await animalTypeService.Save(animalType);
            await petService.Save(pet);
            // Deberia insertarse
            Assert.Equal<Pet>(pet, context.Pets.Find(pet.Id));
        }

        [Fact]
        public async Task ShouldBeMissingDataAsync()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            Pet pet = new Pet();
            PetService petService = new PetService(context);

            // No deberia insertarse ya que faltan datos
            await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await petService.Save(pet);
            });
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

        [Fact]
        public async Task ShouldUpdateAsync()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            Pet pet = CreatePet("Toto", 1, 1, 1, new DateTime(2015, 12, 25), "2983458324", "photo_url", "description", 1, 0);
            City city = CreateCity("Tres Arroyos");
            AnimalType animalType = CreateAnimalType("Gato");
            PetService petService = new PetService(context);
            CityService cityService = new CityService(context);
            AnimalTypeService animalTypeService = new AnimalTypeService(context);

            await cityService.Save(city);
            await animalTypeService.Save(animalType);
            await petService.Save(pet);
            pet.Name = "Yasi";
            await petService.Save(pet);
            int numberOfPets = await context.Pets.CountAsync();

            // Deberia haber una sola mascota ya que editamos la misma que insertamos
            Assert.Equal<int>(1, numberOfPets);
        }

        [Fact]
        public async Task ShouldDelete()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            Pet pet = CreatePet("Toto", 1, 1, 1, new DateTime(2015, 12, 25), "2983458324", "photo_url", "description", 1, 0);
            City city = CreateCity("Tres Arroyos");
            AnimalType animalType = CreateAnimalType("Gato");
            PetService petService = new PetService(context);
            CityService cityService = new CityService(context);
            AnimalTypeService animalTypeService = new AnimalTypeService(context);

            await cityService.Save(city);
            await animalTypeService.Save(animalType);
            await petService.Save(pet);
            await petService.Delete(pet.Id);
            int numberOfPets = await context.Pets.CountAsync();

            // No debería haber mascotas ya que se elimino la unica que se inserto
            Assert.Equal<int>(0, numberOfPets);
        }
    }
}
