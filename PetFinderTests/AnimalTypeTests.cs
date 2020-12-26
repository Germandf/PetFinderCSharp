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
    public class AnimalTypeTests
    {
        private PetFinderDbContextFactory dbContextFactory { get; set; }

        public AnimalTypeTests()
        {
            dbContextFactory = new PetFinderDbContextFactory();
        }

        public AnimalType CreateAnimalType(string name)
        {
            var auxAnimalType = new AnimalType();
            auxAnimalType.Name = name;
            auxAnimalType.SerializedName = auxAnimalType.Name.ToUpper().Replace(" ", "");
            return auxAnimalType;
        }

        [Fact]
        public async Task ShouldNotSaveWithSameName()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            AnimalType animalType = CreateAnimalType("Perro");
            AnimalType animalTypeRepeated = CreateAnimalType("Perro");
            AnimalTypeService animalTypeService = new AnimalTypeService(context);

            await animalTypeService.Save(animalType);
            // No deberia insertarse ya que existe una ciudad con el mismo nombre
            await Assert.ThrowsAsync<AnimalTypeAlreadyExistsException>(async () =>
            {
                await animalTypeService.Save(animalTypeRepeated);
            });
        }

        [Fact]
        public async Task ShouldUpdateAsync()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            AnimalType animalType= CreateAnimalType("Perro Lobo");
            AnimalTypeService animalTypeService = new AnimalTypeService(context);

            await animalTypeService.Save(animalType);
            animalType.Name = "Edited mock";
            animalType.SerializedName = "EDITEDMOCK";
            await animalTypeService.Save(animalType);
            int numberOfAnimalTypes = await context.AnimalTypes.CountAsync();

            // Deberia haber una sola ciudad ya que editamos la misma que insertamos
            Assert.Equal<int>(1, numberOfAnimalTypes);
        }

        [Fact]
        public async Task ShouldAddAsync()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            AnimalType animalType = CreateAnimalType("Perro Lobo");
            AnimalTypeService animalTypeService = new AnimalTypeService(context);

            await animalTypeService.Save(animalType);
            AnimalType animalTypeGato = CreateAnimalType("Gato");
            await animalTypeService.Save(animalTypeGato);

            int numberOfAnimalTypes = context.AnimalTypes.Count();

            // Deberia haber una sola ciudad ya que editamos la misma que insertamos
            Assert.Equal<int>(2, numberOfAnimalTypes);
        }

        [Fact]
        public void ShouldBeInvalidName()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            AnimalTypeService cityService = new AnimalTypeService(context);

            string invalidName = "algo muy largo con muchos caracteres";
            bool isValid = cityService.IsValidName(invalidName);

            // Deberia ser falso ya que la cadena tiene 36 caracteres, siendo el maximo 35
            Assert.False(isValid);
        }

        [Fact]
        public void ShouldBeInvalidNameIlegalCharacters()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            AnimalTypeService cityService = new AnimalTypeService(context);

            string invalidName = "@asdasd 213"; // Solo deberia aceptar letras de la A a la Z
            bool isValid = cityService.IsValidName(invalidName);

            Assert.False(isValid);
        }

        [Fact]
        public void ShouldBeValidName()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            AnimalTypeService cityService = new AnimalTypeService(context);

            string invalidName = "Perro Lobo"; // Solo deberia aceptar numeros de la A a la Z
            bool isValid = cityService.IsValidName(invalidName);

            Assert.True(isValid);
        }
    }
}
