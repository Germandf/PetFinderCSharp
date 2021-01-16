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
    public class CityTests
    {
        private IFixture fixture;

        public CityTests()
        {
            var DbContextFactory = new PetFinderDbContextFactory();
            fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            fixture.Register(DbContextFactory.CreateContext);
        }

        private City CreateCity(string name)
        {
            var auxCity = new City();
            auxCity.Name = name;
            auxCity.SerializedName = auxCity.Name.ToUpper().Replace(" ", "");
            return auxCity;
        }

        [Fact]
        private async Task ShouldInsertAsync()
        {
            var sut = fixture.Create<CityService>();
            var city = CreateCity("Tres Arroyos");

            await sut.Save(city);
            // Deberia insertarse
            Assert.Equal<City>(city, await sut.Get(city.Id));
        }

        [Fact]
        private async Task ShouldNotInsertAsync()
        {
            City city = new City();
            var sut = fixture.Create<CityService>();

            // No deberia insertarse ya que no tiene nombre
            GenericResult result = await sut.Save(city);
            // No deberia insertarse ya que existe una ciudad con el mismo nombre
            Assert.False(result.Success);
        }

        [Fact]
        private async Task ShouldNotSaveWithSameName()
        {
            var sut = fixture.Create<CityService>();
            var city = CreateCity("Tres Arroyos");
            var cityRepeated = CreateCity("Tres Arroyos");

            await sut.Save(city);
            // No deberia insertarse ya que existe una ciudad con el mismo nombre
            GenericResult result = await sut.Save(cityRepeated);
            // No deberia insertarse ya que existe una ciudad con el mismo nombre
            Assert.False(result.Success);
        }

        [Fact]
        private void ShouldBeInvalidName()
        {
            var sut = fixture.Create<CityService>();

            string invalidName = "asdasdasdasdasdasdasdasdasdasdasdasd";
            bool isValid = sut.IsValidName(invalidName);

            // Deberia ser falso ya que la cadena tiene 36 caracteres, siendo el maximo 35
            Assert.False(isValid);
        }

        [Fact]
        private async Task ShouldUpdateAsync()
        {
            var city = CreateCity("Tres Arroyos");
            var sut = fixture.Create<CityService>();

            await sut.Save(city);
            city.Name = "Edited mock";
            city.SerializedName = "EDITEDMOCK";
            await sut.Save(city);
            var numberOfCities = (await sut.GetAll()).Count();

            // Deberia haber una sola ciudad ya que editamos la misma que insertamos
            Assert.Equal<int>(1, numberOfCities);
        }

        [Fact]
        private async Task ShouldDelete()
        {
            var city = CreateCity("Tres Arroyos");
            var sut = fixture.Create<CityService>();

            await sut.Save(city);
            await sut.Delete(city.Id);
            var numberOfCities = (await sut.GetAll()).Count();

            // No debería haber ciudades ya que se elimino la unica que se inserto
            Assert.Equal<int>(0, numberOfCities);
        }
    }
}