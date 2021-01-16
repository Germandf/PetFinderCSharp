using Microsoft.EntityFrameworkCore;
using PetFinder.Data;
using PetFinder.Helpers;
using PetFinder.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Xunit;


namespace PetFinderTests
{
    public class AnimalTypeTests
    {
        private IFixture fixture;

        public AnimalTypeTests()
        {
            var DbContextFactory = new PetFinderDbContextFactory();
            fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            fixture.Register(DbContextFactory.CreateContext);
        }

        private AnimalType CreateAnimalType(string name)
        {
            var auxAnimalType = new AnimalType();
            auxAnimalType.Name = name;
            auxAnimalType.SerializedName = auxAnimalType.Name.ToUpper().Replace(" ", "");
            return auxAnimalType;
        }

        [Fact]
        private async Task ShouldNotSaveWithSameName()
        {
            var animalType = CreateAnimalType("Perro");
            var animalTypeRepeated = CreateAnimalType("Perro");
            var sut = fixture.Create<AnimalTypeService>();
            await sut.Save(animalType);
            GenericResult result = await sut.Save(animalTypeRepeated);
            // No deberia insertarse ya que existe una ciudad con el mismo nombre
            Assert.False(result.Success);
        }

        [Fact]
        private async Task ShouldUpdateAsync()
        {
            AnimalType animalType= CreateAnimalType("Perro Lobo");
            var sut = fixture.Create<AnimalTypeService>();

            await sut.Save(animalType);
            animalType.Name = "Edited mock";
            await sut.Save(animalType);
            var numberOfAnimalTypes = (await sut.GetAll()).Count();

            // Deberia haber una sola ciudad ya que editamos la misma que insertamos
            Assert.Equal<int>(1, numberOfAnimalTypes);
        }

        [Fact]
        private async Task ShouldAddAsync()
        {

            var sut = fixture.Create<AnimalTypeService>();

            var animalType = CreateAnimalType("Perro Lobo");
            var animalTypeGato = CreateAnimalType("Gato");

            await sut.Save(animalType);
            await sut.Save(animalTypeGato);

            var numberOfAnimalTypes = (await sut.GetAll()).Count();
            // Deberia haber una sola ciudad ya que editamos la misma que insertamos
            Assert.Equal<int>(2, numberOfAnimalTypes);
        }

        [Fact]
        private void ShouldBeInvalidName()
        {
            var sut = fixture.Create<AnimalTypeService>();

            string invalidName = "algo muy largo con muchos caracteres";
            bool isValid = sut.IsValidName(invalidName);

            // Deberia ser falso ya que la cadena tiene 36 caracteres, siendo el maximo 35
            Assert.False(isValid);
        }

        [Fact]
        private void ShouldBeInvalidNameIlegalCharacters()
        {
            var sut = fixture.Create<AnimalTypeService>();

            string invalidName = "@asdasd 213"; // Solo deberia aceptar letras de la A a la Z
            bool isValid = sut.IsValidName(invalidName);

            Assert.False(isValid);
        }

        [Fact]
        private void ShouldBeValidName()
        {
            var sut = fixture.Create<AnimalTypeService>();

            string invalidName = "Perro Lobo"; // Solo deberia aceptar numeros de la A a la Z
            bool isValid = sut.IsValidName(invalidName);

            Assert.True(isValid);
        }
    }
}
