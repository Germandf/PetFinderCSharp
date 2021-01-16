using Microsoft.EntityFrameworkCore;
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
    public class GenderTests
    {
        private IFixture fixture;

        public GenderTests()
        {
            var dbContextFactory = new PetFinderDbContextFactory();
            fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            fixture.Register(dbContextFactory.CreateContext);
        }
        public Gender CreateGender(string name)
        {
            var auxGender = new Gender();
            auxGender.Name = name;
            return auxGender;
        }

        [Fact]
        public async Task ShouldInsertAsync()
        {
            var sut = fixture.Create<GenderService>();
            Gender gender = CreateGender("Masculino");

            await sut.Save(gender);
            // Deberia insertarse
            Assert.Equal<Gender>(gender, await sut.Get(gender.Id));
        }

        [Fact]
        public async Task ShouldNotInsertAsync()
        {
            var sut = fixture.Create<GenderService>();
            Gender gender = new Gender();

            // No deberia insertarse ya que no tiene nombre
            var result = await sut.Save(gender);

            Assert.False(result.Success);
        }

        [Fact]
        public async Task ShouldNotSaveWithSameName()
        {
            var sut = fixture.Create<GenderService>();
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
            var sut = fixture.Create<GenderService>();

            string invalidName = "asdasdasdasdasdasdasd";
            bool isValid = sut.IsValidName(invalidName);

            // Deberia ser falso ya que la cadena tiene 21 caracteres, siendo el maximo 20
            Assert.False(isValid);
        }

        [Fact]
        public async void ShouldBeNotRepeated()
        {
            var sut = fixture.Create<GenderService>();

            string notRepeatedName = "Masculino";
            bool isValid = await sut.IsRepeated(notRepeatedName);

            // Deberia ser falso ya que no hay otro genero con ese nombre
            Assert.False(isValid);
        }

        [Fact]
        public async Task ShouldUpdateAsync()
        {
            var sut = fixture.Create<GenderService>();
            var gender = CreateGender("Masculino");

            await sut.Save(gender);
            gender.Name = "Femenino";
            await sut.Save(gender);
            int numberOfGenders = (await sut.GetAll()).Count();

            // Deberia haber tres generos ya que dos vienen por defecto e insertamos uno solo
            Assert.Equal<int>(3, numberOfGenders);
        }

        [Fact]
        public async Task ShouldDelete()
        {
            var gender = CreateGender("Masculino");
            var sut = fixture.Create<GenderService>();

            await sut.Save(gender);
            await sut.Delete(gender.Id);
            int numberOfGenders = (await sut.GetAll()).Count();

            // Deberia haber 2 generos que son los que vienen por defecto
            Assert.Equal<int>(2, numberOfGenders);
        }
    }
}
