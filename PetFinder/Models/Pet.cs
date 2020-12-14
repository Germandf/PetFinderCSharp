using System;
using System.Collections.Generic;

#nullable disable

namespace PetFinder.Models
{
    public partial class Pet
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
        public int? UserId { get; set; }
        public byte Found { get; set; }

        public AnimalType AnimalType { get; set; }
        public City City { get; set; }
        public Gender Gender { get; set; }
    }
}
