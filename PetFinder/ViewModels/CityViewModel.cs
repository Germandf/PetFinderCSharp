using PetFinder.Models;

namespace PetFinder.ViewModels
{
    public class CityViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public City ConvertToCity()
        {
            var city = new City {Id = Id, Name = Name};
            return city;
        }

        public void ConvertFromCity(City city)
        {
            Id = city.Id;
            Name = city.Name;
        }
    }
}