using System;
using System.Collections.Generic;

#nullable disable

namespace PetFinder.Models
{
    public partial class Gender
    {
        public Gender()
        {
            Pets = new HashSet<Pet>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string SerializedName { get; set; }

        public virtual ICollection<Pet> Pets { get; set; }
    }
}
