using Microsoft.EntityFrameworkCore;
using PetFinder.Data;
using PetFinder.Helpers;
using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PetFinderTests
{
    public class CityTests
    {
        private PetFinderDbContextFactory dbContextFactory { get; set; }
        public CityTests()
        {
            dbContextFactory = new PetFinderDbContextFactory();
        }
        public City CreateCity(string name)
        {
            var auxCity = new City();
            auxCity.Name = name;
            auxCity.SerializedName = auxCity.Name.ToUpper().Replace(" ", "");
            return auxCity;
        }

        [Fact]
        public async Task ShouldInsertAsync()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            City city = CreateCity("Tres Arroyos");
            CityService cityService = new CityService(context);

            await cityService.Save(city);
            // Deberia insertarse
            Assert.Equal<City>(city, context.Cities.Find(city.Id));
        }

        [Fact]
        public async Task ShouldNotInsertAsync()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            City city = new City();
            CityService cityService = new CityService(context);

            // No deberia insertarse ya que no tiene nombre
            GenericResult result = await cityService.Save(city);
            // No deberia insertarse ya que existe una ciudad con el mismo nombre
            Assert.False(result.Success);
        }

        [Fact]
        public async Task ShouldNotSaveWithSameName()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            City city = CreateCity("Tres Arroyos");
            City cityRepeated = CreateCity("Tres Arroyos");
            CityService cityService = new CityService(context);

            await cityService.Save(city);
            // No deberia insertarse ya que existe una ciudad con el mismo nombre
            GenericResult result = await cityService.Save(cityRepeated);
            // No deberia insertarse ya que existe una ciudad con el mismo nombre
            Assert.False(result.Success);
        }

        [Fact]
        public void ShouldBeInvalidName()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            CityService cityService = new CityService(context);

            string invalidName = "asdasdasdasdasdasdasdasdasdasdasdasd";
            bool isValid = cityService.IsValidName(invalidName);

            // Deberia ser falso ya que la cadena tiene 36 caracteres, siendo el maximo 35
            Assert.False(isValid);
        }

        [Fact]
        public async Task ShouldUpdateAsync()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            City city = CreateCity("Tres Arroyos");
            CityService cityService = new CityService(context);

            await cityService.Save(city);
            city.Name = "Edited mock";
            city.SerializedName = "EDITEDMOCK";
            await cityService.Save(city);
            int numberOfCities = await context.Cities.CountAsync();

            // Deberia haber una sola ciudad ya que editamos la misma que insertamos
            Assert.Equal<int>(1, numberOfCities);
        }

        [Fact]
        public async Task ShouldDelete()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            City city = CreateCity("Tres Arroyos");
            CityService cityService = new CityService(context);

            await cityService.Save(city);
            await cityService.Delete(city.Id);
            int numberOfCities = await context.Cities.CountAsync();

            // No debería haber ciudades ya que se elimino la unica que se inserto
            Assert.Equal<int>(0, numberOfCities);
        }
    }
}