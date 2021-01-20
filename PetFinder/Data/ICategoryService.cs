using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PetFinder.Helpers;
using PetFinder.Models;
using PetFinder.ViewModels;
using Serilog;

namespace PetFinder.Data
{
    public interface ICategoryService<T> where T : CategoryBase, new()
    {
        /// <summary>
        ///     Gets all <typeparamref name="T" />
        /// </summary>
        /// <returns>
        ///     An IEnumerable of type <typeparamref name="T" />
        /// </returns>
        Task<IEnumerable<T>> GetAll();

        /// <summary>
        ///     Gets a specific <typeparamref name="T" /> by its Id
        /// </summary>
        /// <returns>
        ///     A <typeparamref name="T" /> object
        /// </returns>
        Task<T> Get(int id);

        /// <summary>
        ///     Inserts a <typeparamref name="T" />
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Insert(T category);

        /// <summary>
        ///     Updates a <typeparamref name="T" />
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Update(T category);

        /// <summary>
        ///     Deletes a <typeparamref name="T" />
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Delete(int id);

        /// <summary>
        ///     Inserts or Updates a <typeparamref name="T" /> depending on the case.
        /// </summary>
        /// <returns>
        ///     A GenericResult that indicates if it was successfull or not, if not, it will contain the error/s
        /// </returns>
        Task<GenericResult> Save(T category);

        /// <summary>
        ///     Transforms the CategoryViewModel into a <typeparamref name="T" /> and calls Save with the
        ///     <typeparamref name="T" /> parameter
        /// </summary>
        /// <returns>
        ///     A GenericResult that indicates if it was successfull or not, if not, it will contain the error/s
        /// </returns>
        Task<GenericResult> Save(CategoryViewModel<T> categoryViewModel);

        /// <summary>
        ///     Checks if the name is used by an already created <typeparamref name="T" />
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it's being used or not
        /// </returns>
        Task<bool> IsRepeated(string name);

        /// <summary>
        ///     Checks if exists a Pet that depends on a specific <typeparamref name="T" />
        /// </summary>
        /// <returns>
        ///     A bool that indicates if there is at least one
        /// </returns>
        Task<bool> HasPetsAssociated(T category);
    }

    public class CategoryService<T> : ICategoryService<T> where T : CategoryBase, new()
    {
        public const string InvalidNameError = "El nombre ingresado no corresponde a un nombre valido";
        public const string RepeatedCategoryError = "El nombre ya está registrado. Intenta con uno diferente!";
        public const string SavingError = "Ocurrio un error al guardar";
        private readonly PetFinderContext _context;
        private readonly ILogger _logger;

        public CategoryService(PetFinderContext context,
            ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Delete(int id)
        {
            var category = await Get(id);
            _context.Remove(category);
            _logger.Warning("Category {category} with name {name} deleted, Id: {id}", 
                typeof(T).Name, category.Name, category.Id);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> Get(int id)
        {
            var category = await _context.Set<T>().FindAsync(id);
            _context.Entry(category).State = EntityState.Detached;
            return category;
        }

        public async Task<bool> Insert(T category)
        {
            _context.Set<T>().Add(category);
            _logger.Information("Category of type:{category} with name {name} created", typeof(T).Name, category.Name);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(T category)
        {
            //Le decimos a la DB que la categoria fue modificada
            _context.Entry(category).State = EntityState.Modified;
            _logger.Information("Category of type:{category}  with name   {name} updated, Id: {id}", 
                typeof(T).Name, category.Name, category.Id);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<GenericResult> Save(CategoryViewModel<T> categoryViewModel)
        {
            var category = categoryViewModel.ConvertToCategory();
            return await Save(category);
        }

        public async Task<GenericResult> Save(T category)
        {
            var result = new GenericResult();
            if (IsValidName(category.Name))
            {
                category.SerializedName = category.Name.ToUpper().Replace(" ", "");
                if (await IsRepeated(category.SerializedName) && category.Id == 0)
                    result.AddError(RepeatedCategoryError);
                if (result.Success)
                {
                    if (category.Id > 0)
                    {
                        if (!await Update(category)) result.AddError(SavingError);
                    }
                    else if (!await Insert(category))
                    {
                        result.AddError(SavingError);
                    }
                }
            }
            else
            {
                result.AddError(InvalidNameError);
            }

            return result;
        }

        public async Task<bool> IsRepeated(string serializedName)
        {
            var existingCategoryCount = await _context.Set<T>().CountAsync(a => a.SerializedName == serializedName);
            if (existingCategoryCount > 0) return true;
            return false;
        }

        public async Task<bool> HasPetsAssociated(T category)
        {
            var categoryList = await _context.Set<T>().Include(p => p.Pets).FirstAsync();
            var petsFromThisCategory = categoryList.Pets;

            return petsFromThisCategory.Count() == 0;
        }

        public bool IsValidName(string name)
        {
            if (name == null) return false;
            if (name.Length <= 0 || name.Length > 35) return false;
            // Chequeo que sean caracteres de a - Z con espacios
            var match = Regex.Match(name, "^[a-zA-Z ]+$");
            if (!match.Success) return false;
            return true;
        }
    }
}