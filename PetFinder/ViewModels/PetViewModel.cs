using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.ViewModels
{
    public class PetViewModel
    {
        public string Name { get; set; }
        public int AnimalTypeId { get; set; }
        public int CityId { get; set; }
        public int GenderId { get; set; }
        public DateTime Date { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }

        public Pet ConvertToPet()
        {
            var pet = new Pet();
            pet.Name = Name;
            pet.AnimalTypeId = AnimalTypeId;
            pet.CityId = CityId;
            pet.GenderId = GenderId;
            pet.Date = Date;
            pet.PhoneNumber = PhoneNumber;
            pet.Description = Description;
            return pet;
        }
    }
}
