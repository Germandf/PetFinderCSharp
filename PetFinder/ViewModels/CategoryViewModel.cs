using PetFinder.Models;

namespace PetFinder.ViewModels
{
    public class CategoryViewModel<T> where T : CategoryBase, new()
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public T ConvertToCategory()
        {
            var category = new T {Id = Id, Name = Name};
            return category;
        }

        public void ConvertFromCategory(T category)
        {
            Id = category.Id;
            Name = category.Name;
        }
    }
}