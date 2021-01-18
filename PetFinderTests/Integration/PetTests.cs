using AutoFixture;
using AutoFixture.AutoNSubstitute;
using PetFinder.Data;
using PetFinder.Models;
using Xunit;

namespace PetFinderTests.Integration
{
    public class PetTests
    {
        private readonly IFixture _fixture;

        public PetTests()
        {
            var dbContextFactory = new PetFinderDbContextFactory();
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _fixture.Register(dbContextFactory.CreateContext);
        }

        [Fact]
        public void ShouldBeMissingDataAsync()
        {
            var sut = _fixture.Create<PetService>();
            var pet = new Pet();

            var errors = sut.CheckPet(pet);
            //Deberia dar 6 errores
            Assert.Equal(6, errors.Count);
        }

        [Fact]
        public void ShouldBeInvalidName()
        {
            var sut = _fixture.Create<PetService>();
            var invalidName = "asdasdasdasdasdasdasd";
            var isValid = sut.IsValidName(invalidName);
            Assert.False(isValid, "Solo se admiten nombres de 20 caracteres como máximo");
        }
    }
}