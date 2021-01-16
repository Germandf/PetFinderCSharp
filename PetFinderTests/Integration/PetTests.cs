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
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Xunit;

namespace PetFinderTests
{
    public class PetTests
    {
        private IFixture fixture;
        public PetTests()
        {
            var dbContextFactory = new PetFinderDbContextFactory();
            fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            fixture.Register(dbContextFactory.CreateContext);
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
            var sut = fixture.Create<PetService>();
            var pet = new Pet();

            List<string> errors = sut.CheckPet(pet);
            //Deberia dar 6 errores
            Assert.Equal<int>(6, errors.Count);
        }

        [Fact]
        public void ShouldBeInvalidName()
        {
            var sut = fixture.Create<PetService>();
            string invalidName = "asdasdasdasdasdasdasd";
            bool isValid = sut.IsValidName(invalidName);
            // Deberia ser falso ya que la cadena tiene 21 caracteres, siendo el maximo 20
            Assert.False(isValid);
        }
    }
}
