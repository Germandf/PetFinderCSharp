using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.ViewModels
{
    public class CityViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public City ConvertToCity()
        {
            var city = new City();
            city.Id = Id;
            city.Name = Name;
            return city;
        }

        public void ConvertFromCity(City city)
        {
            Id = city.Id;
            Name = city.Name;
        }
    }
}
