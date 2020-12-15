using Microsoft.EntityFrameworkCore;
using PetFinder.Data;
using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PetFinderTests
{
    public class GenderTests
    {
        private PetFinderDbContextFactory dbContextFactory { get; set; }
        public GenderTests()
        {
            dbContextFactory = new PetFinderDbContextFactory();
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
            PetFinderContext context = dbContextFactory.CreateContext();
            Gender gender = CreateGender("Masculino");
            GenderService genderService = new GenderService(context);

            await genderService.Save(gender);
            // Deberia insertarse
            Assert.Equal<Gender>(gender, context.Genders.Find(gender.Id));
        }

        [Fact]
        public async Task ShouldNotInsertAsync()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            Gender gender = new Gender();
            GenderService genderService = new GenderService(context);

            // No deberia insertarse ya que no tiene nombre
            await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await genderService.Save(gender);
            });
        }

        [Fact]
        public async Task ShouldNotSaveWithSameName()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            Gender gender1 = CreateGender("Masculino");
            Gender gender2 = CreateGender("Masculino");
            GenderService genderService = new GenderService(context);

            await genderService.Save(gender1);
            // No deberia insertarse ya que existe un genero con el mismo nombre
            await Assert.ThrowsAsync<GenderAlreadyExistsException>(async () =>
            {
                await genderService.Save(gender2);
            });
        }

        [Fact]
        public void ShouldBeInvalidName()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            GenderService genderService = new GenderService(context);

            string invalidName = "asdasdasdasdasdasdasd";
            bool isValid = genderService.IsValidName(invalidName);

            // Deberia ser falso ya que la cadena tiene 21 caracteres, siendo el maximo 20
            Assert.False(isValid);
        }

        [Fact]
        public async void ShouldBeNotRepeated()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            GenderService genderService = new GenderService(context);

            string notRepeatedName = "Masculino";
            bool isValid = await genderService.IsNotRepeated(notRepeatedName);

            // Deberia ser falso ya que la cadena tiene 21 caracteres, siendo el maximo 20
            Assert.True(isValid);
        }

        [Fact]
        public async Task ShouldUpdateAsync()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            Gender gender = CreateGender("Masculino");
            GenderService genderService = new GenderService(context);

            await genderService.Save(gender);
            gender.Name = "Femenino";
            await genderService.Save(gender);
            int numberOfGenders = await context.Genders.CountAsync();

            // Deberia haber tres generos ya que dos vienen por defecto e insertamos uno solo
            Assert.Equal<int>(3, numberOfGenders);
        }

        [Fact]
        public async Task ShouldDelete()
        {
            PetFinderContext context = dbContextFactory.CreateContext();
            Gender gender = CreateGender("Masculino");
            GenderService genderService = new GenderService(context);

            await genderService.Save(gender);
            await genderService.Delete(gender.Id);
            int numberOfGenders = await context.Genders.CountAsync();

            // Deberia haber 2 generos que son los que vienen por defecto
            Assert.Equal<int>(2, numberOfGenders);
        }
    }
}
