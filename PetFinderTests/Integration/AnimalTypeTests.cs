using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Microsoft.AspNetCore.Mvc;
using PetFinder.Data;
using PetFinder.Models;
using Xunit;

namespace PetFinderTests.Integration
{
    public class AnimalTypeTests
    {
        private readonly IFixture _fixture;

        public AnimalTypeTests()
        {
            var dbContextFactory = new PetFinderDbContextFactory();
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _fixture.Register(dbContextFactory.CreateContext);
        }

        private AnimalType CreateAnimalType(string name)
        {
            var auxAnimalType = new AnimalType {Name = name};
            auxAnimalType.SerializedName = auxAnimalType.Name.ToUpper().Replace(" ", "");
            return auxAnimalType;
        }

        [Fact]
        private async Task ShouldNotSaveWithSameName()
        {
            var animalType = CreateAnimalType("Perro");
            var animalTypeRepeated = CreateAnimalType("Perro");
            var sut = _fixture.Create<CategoryService<AnimalType>>();
            await sut.Save(animalType);
            var result = await sut.Save(animalTypeRepeated);
            Assert.False(result.Success, "No deberia permitir insertar tipos de animales duplicados");
        }

        [Fact]
        private async Task ShouldUpdateAsync()
        {
            var animalType = CreateAnimalType("Perro Lobo");
            var sut = _fixture.Create<CategoryService<AnimalType>>();

            await sut.Save(animalType);
            animalType.Name = "Edited mock";
            await sut.Save(animalType);
            var numberOfAnimalTypes = (await sut.GetAll()).Count();

            // Deberia haber una sola ciudad ya que editamos la misma que insertamos
            Assert.Equal(1, numberOfAnimalTypes);
        }

        [Fact]
        private async Task ShouldAddAsync()
        {
            var sut = _fixture.Create<CategoryService<AnimalType>>();

            var animalType = CreateAnimalType("Perro Lobo");
            var animalTypeGato = CreateAnimalType("Gato");

            await sut.Save(animalType);
            await sut.Save(animalTypeGato);

            var numberOfAnimalTypes = (await sut.GetAll()).Count();
            // Deberia haber una sola ciudad ya que editamos la misma que insertamos
            Assert.Equal(2, numberOfAnimalTypes);
        }


        [Theory]
        [InlineData("Nombre demasiado largo con muchos caracteres")]
        [InlineData("@asdasd 213")]
        private void ShouldBeInvalidName(string invalidName)
        {
            var sut = _fixture.Create<CategoryService<AnimalType>>();

            var isValid = sut.IsValidName(invalidName);
            Assert.False(isValid, "Debería aceptar letras de la A a la Z, como máximo 35 caracteres");
        }
        [Theory]
        [InlineData("Perro lobo")]
        [InlineData("Gato")]
        private void ShouldBeValidName(string name)
        {
            var sut = _fixture.Create<CategoryService<AnimalType>>();
            var isValid = sut.IsValidName(name);
            Assert.True(isValid);
        }
    }
}