using PetFinder.Models;

namespace PetFinder.ViewModels
{
    public class AnimalTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public AnimalType ConvertToAnimalType()
        {
            var animalType = new AnimalType {Id = Id, Name = Name};
            return animalType;
        }

        public void ConvertFromAnimalType(AnimalType animalType)
        {
            Id = animalType.Id;
            Name = animalType.Name;
        }
    }
}