using System.Collections.Generic;

#nullable disable

namespace PetFinder.Models
{
    public class City : CategoryBase
    {
        public City()
        {
            Pets = new HashSet<Pet>();
        }
    }
}