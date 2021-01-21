using System.Collections.Generic;

#nullable disable

namespace PetFinder.Models
{
    public class AnimalType : CategoryBase
    {
        public AnimalType()
        {
            Pets = new HashSet<Pet>();
        }
    }
}