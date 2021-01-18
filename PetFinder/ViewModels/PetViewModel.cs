using System;
using PetFinder.Models;

namespace PetFinder.ViewModels
{
    public class PetViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AnimalTypeId { get; set; }
        public int CityId { get; set; }
        public int GenderId { get; set; }
        public DateTime Date { get; set; }
        public string PhoneNumber { get; set; }
        public string Photo { get; set; }
        public string Description { get; set; }

        public Pet ConvertToPet()
        {
            var pet = new Pet
            {
                Id = Id,
                Name = Name,
                AnimalTypeId = AnimalTypeId,
                CityId = CityId,
                GenderId = GenderId,
                Date = Date,
                PhoneNumber = PhoneNumber,
                Description = Description
            };
            return pet;
        }

        public void ConvertFromPet(Pet pet)
        {
            Id = pet.Id;
            Name = pet.Name;
            AnimalTypeId = pet.AnimalTypeId;
            CityId = pet.CityId;
            GenderId = pet.GenderId;
            Date = pet.Date;
            PhoneNumber = pet.PhoneNumber;
            Photo = pet.Photo;
            Description = pet.Description;
        }
    }
}