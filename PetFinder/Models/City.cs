﻿using System;
using System.Collections.Generic;

#nullable disable

namespace PetFinder.Models
{
    public partial class City
    {
        public City()
        {
            Pets = new HashSet<Pet>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Pet> Pets { get; set; }
    }
}