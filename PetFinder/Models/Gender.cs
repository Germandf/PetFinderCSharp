using System.Collections.Generic;

#nullable disable

namespace PetFinder.Models
{
    public class Gender: CategoryBase
    {
        public Gender()
        {
            Pets = new HashSet<Pet>();
        }
    }
}