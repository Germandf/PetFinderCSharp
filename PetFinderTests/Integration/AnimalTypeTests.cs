using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
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
            var sut = _fixture.Create<AnimalTypeService>();
            await sut.Save(animalType);
            var result = await sut.Save(animalTypeRepeated);
            // No deberia insertarse ya que existe una ciudad con el mismo nombre
            Assert.False(result.Success);
        }

        [Fact]
        private async Task ShouldUpdateAsync()
        {
            var animalType = CreateAnimalType("Perro Lobo");
            var sut = _fixture.Create<AnimalTypeService>();

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
            var sut = _fixture.Create<AnimalTypeService>();

            var animalType = CreateAnimalType("Perro Lobo");
            var animalTypeGato = CreateAnimalType("Gato");

            await sut.Save(animalType);
            await sut.Save(animalTypeGato);

            var numberOfAnimalTypes = (await sut.GetAll()).Count();
            // Deberia haber una sola ciudad ya que editamos la misma que insertamos
            Assert.Equal(2, numberOfAnimalTypes);
        }

        [Fact]
        private void ShouldBeInvalidName()
        {
            var sut = _fixture.Create<AnimalTypeService>();

            var invalidName = "algo muy largo con muchos caracteres";
            var isValid = sut.IsValidName(invalidName);

            // Deberia ser falso ya que la cadena tiene 36 caracteres, siendo el maximo 35
            Assert.False(isValid);
        }

        [Fact]
        private void ShouldBeInvalidNameIlegalCharacters()
        {
            var sut = _fixture.Create<AnimalTypeService>();

            var invalidName = "@asdasd 213"; // Solo deberia aceptar letras de la A a la Z
            var isValid = sut.IsValidName(invalidName);

            Assert.False(isValid);
        }

        [Fact]
        private void ShouldBeValidName()
        {
            var sut = _fixture.Create<AnimalTypeService>();

            var invalidName = "Perro Lobo"; // Solo deberia aceptar numeros de la A a la Z
            var isValid = sut.IsValidName(invalidName);

            Assert.True(isValid);
        }
    }
}