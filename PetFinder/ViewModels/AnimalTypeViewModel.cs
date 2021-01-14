using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.ViewModels
{
    public class AnimalTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public AnimalType ConvertToAnimalType()
        {
            var animalType = new AnimalType();
            animalType.Id = Id;
            animalType.Name = Name;
            return animalType;
        }

        public void ConvertFromAnimalType(AnimalType animalType)
        {
            Id = animalType.Id;
            Name = animalType.Name;
        }
    }
}
