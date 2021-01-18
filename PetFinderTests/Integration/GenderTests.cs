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
            var sut = _fixture.Create<GenderService>();
            var gender = CreateGender("Masculino");

            await sut.Save(gender);
            // Deberia insertarse
            Assert.Equal(gender, await sut.Get(gender.Id));
        }

        [Fact]
        public async Task ShouldNotInsertAsync()
        {
            var sut = _fixture.Create<GenderService>();
            var gender = new Gender();

            // No deberia insertarse ya que no tiene nombre
            var result = await sut.Save(gender);

            Assert.False(result.Success);
        }

        [Fact]
        public async Task ShouldNotSaveWithSameName()
        {
            var sut = _fixture.Create<GenderService>();
            var gender1 = CreateGender("Masculino");
            var gender2 = CreateGender("Masculino");

            await sut.Save(gender1);
            // No deberia insertarse ya que existe un genero con el mismo nombre

            var result = await sut.Save(gender2);

            Assert.False(result.Success);
        }

        [Fact]
        public void ShouldBeInvalidName()
        {
            var sut = _fixture.Create<GenderService>();

            var invalidName = "asdasdasdasdasdasdasd";
            var isValid = sut.IsValidName(invalidName);

            // Deberia ser falso ya que la cadena tiene 21 caracteres, siendo el maximo 20
            Assert.False(isValid);
        }

        [Fact]
        public async void ShouldBeNotRepeated()
        {
            var sut = _fixture.Create<GenderService>();

            var notRepeatedName = "Masculino";
            var isValid = await sut.IsRepeated(notRepeatedName);

            // Deberia ser falso ya que no hay otro genero con ese nombre
            Assert.False(isValid);
        }

        [Fact]
        public async Task ShouldUpdateAsync()
        {
            var sut = _fixture.Create<GenderService>();
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
            var sut = _fixture.Create<GenderService>();

            await sut.Save(gender);
            await sut.Delete(gender.Id);
            var numberOfGenders = (await sut.GetAll()).Count();

            // Deberia haber 2 generos que son los que vienen por defecto
            Assert.Equal(2, numberOfGenders);
        }
    }
}