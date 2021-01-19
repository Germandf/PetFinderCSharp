using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Models
{
    public class CategoryBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SerializedName { get; set; }

        public virtual ICollection<Pet> Pets { get; set; }
    }
}
