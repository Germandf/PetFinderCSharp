using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using PetFinder.Data;
using PetFinder.Models;
using Xunit;

namespace PetFinderTests.Integration
{
    public class CityTests
    {
        private readonly IFixture _fixture;

        public CityTests()
        {
            var dbContextFactory = new PetFinderDbContextFactory();
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _fixture.Register(dbContextFactory.CreateContext);
        }

        private City CreateCity(string name)
        {
            var auxCity = new City {Name = name};
            auxCity.SerializedName = auxCity.Name.ToUpper().Replace(" ", "");
            return auxCity;
        }

        [Fact]
        private async Task ShouldInsertAsync()
        {
            var sut = _fixture.Create<CityService>();
            var city = CreateCity("Tres Arroyos");

            await sut.Save(city);
            // Deberia insertarse
            Assert.Equal(city, await sut.Get(city.Id));
        }

        [Fact]
        private async Task ShouldNotInsertAsync()
        {
            var city = new City();
            var sut = _fixture.Create<CityService>();
            var result = await sut.Save(city);
            Assert.False(result.Success, "No debería insertarse ya que no se especifico un nombre");
        }

        [Fact]
        private async Task ShouldNotSaveWithSameName()
        {
            var sut = _fixture.Create<CityService>();
            var city = CreateCity("Tres Arroyos");
            var cityRepeated = CreateCity("Tres Arroyos");

            await sut.Save(city);
            var result = await sut.Save(cityRepeated);
            Assert.False(result.Success, "No deberia insertarse. Ya existe una ciudad con el mismo nombre.");
        }


        [Theory]
        [InlineData("Nombre demasiado largo con muchisimos caracteres")]
        private void ShouldBeInvalidName(string invalidName)
        {
            var sut = _fixture.Create<CityService>();

            var isValid = sut.IsValidName(invalidName);

            // Deberia ser falso ya que la cadena tiene 36 caracteres, siendo el maximo 35
            Assert.False(isValid, "Debería aceptar letras de la A a la Z, como máximo 35 caracteres");
        }

        [Fact]
        private async Task ShouldUpdateAsync()
        {
            var city = CreateCity("Tres Arroyos");
            var sut = _fixture.Create<CityService>();

            await sut.Save(city);
            city.Name = "Edited mock";
            city.SerializedName = "EDITEDMOCK";
            await sut.Save(city);
            var numberOfCities = (await sut.GetAll()).Count();

            // Deberia haber una sola ciudad ya que editamos la misma que insertamos
            Assert.Equal(1, numberOfCities);
        }

        [Fact]
        private async Task ShouldDelete()
        {
            var city = CreateCity("Tres Arroyos");
            var sut = _fixture.Create<CityService>();

            await sut.Save(city);
            await sut.Delete(city.Id);
            var numberOfCities = (await sut.GetAll()).Count();

            // No debería haber ciudades ya que se elimino la unica que se inserto
            Assert.Equal(0, numberOfCities);
        }
    }
}