using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using PetFinder.Data;
using PetFinder.Models;
using Xunit;

namespace PetFinderTests.Integration
{
    public class GenderTests
    {
        private readonly IFixture _fixture;

        public GenderTests()
        {
            var dbContextFactory = new PetFinderDbContextFactory();
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _fixture.Register(dbContextFactory.CreateContext);
        }

        public Gender CreateGender(string name)
        {
            var auxGender = new Gender {Name = name};
            return auxGender;
        }

        [Fact]
        public async Task ShouldInsertAsync()
        {
            var sut = _fixture.Create<CategoryService<Gender>>();
            var gender = CreateGender("Masculino");

            await sut.Save(gender);
            // Deberia insertarse
            Assert.Equal(gender, await sut.Get(gender.Id));
        }

        [Fact]
        public async Task ShouldNotInsertAsync()
        {
            var sut = _fixture.Create<CategoryService<Gender>>();
            var gender = new Gender();
            var result = await sut.Save(gender);
            Assert.False(result.Success, "No deberia insertarse ya que no tiene nombre");
        }

        [Fact]
        public async Task ShouldNotSaveWithSameName()
        {
            var sut = _fixture.Create<CategoryService<Gender>>();
            var gender1 = CreateGender("Masculino");
            var gender2 = CreateGender("Masculino");

            await sut.Save(gender1);

            var result = await sut.Save(gender2);

            Assert.False(result.Success, "No deberia insertarse ya que existe un genero con el mismo nombre");
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

        [Fact]
        public async void ShouldBeNotRepeated()
        {
            var sut = _fixture.Create<CategoryService<Gender>>();

            var notRepeatedName = "Masculino";
            var isValid = await sut.IsRepeated(notRepeatedName);

            Assert.False(isValid, "Deberia ser falso ya que no hay otro genero con ese nombre");
        }

        [Fact]
        public async Task ShouldUpdateAsync()
        {
            var sut = _fixture.Create<CategoryService<Gender>>();
            var gender = CreateGender("Masculino");

            await sut.Save(gender);
            gender.Name = "Femenino";
            await sut.Save(gender);
            var numberOfGenders = (await sut.GetAll()).Count();

            // Deberia haber tres generos ya que dos vienen por defecto e insertamos uno solo
            Assert.Equal(3, numberOfGenders);
        }

        [Fact]
        public async Task ShouldDelete()
        {
            var gender = CreateGender("Masculino");
            var sut = _fixture.Create<CategoryService<Gender>>();

            await sut.Save(gender);
            await sut.Delete(gender.Id);
            var numberOfGenders = (await sut.GetAll()).Count();

            // Deberia haber 2 generos que son los que vienen por defecto
            Assert.Equal(2, numberOfGenders);
        }
    }
}