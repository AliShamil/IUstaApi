using IUstaApi.Data;
using IUstaApi.Models.DTOs;
using IUstaApi.Models.DTOs.Category;
using IUstaApi.Models.Entities;
using IUstaApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace IUstaApi.Services.Concrete
{
    public class WorkerCategoryService : IWorkerCategoryService
    {
        private readonly UstaDbContext _context;

        public WorkerCategoryService(UstaDbContext context)
        {
            _context=context;
        }

        public async Task<bool> AddAsync(WorkerCategoryDto model)
        {
            if (await _context.Categories.FirstAsync(c => c.Id == Guid.Parse(model.CategoryId)) is null)
                return false;

            var entity = new WorkerCategory()
            {
                CategoryId = Guid.Parse(model.CategoryId),
                WorkerId = model.WorkerId
            };
            try
            {
                var result = await _context.WorkerCategories.AddAsync(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }


        }

        public IEnumerable<CategoryInfoDto> GetAllCategoriesByWorkerId(string id)
        {
            var categoriesForWorker = _context.WorkerCategories
               .Where(wc => wc.WorkerId == id)
               .Select(wc => new CategoryInfoDto
               {
                   Id = wc.CategoryId.ToString(),
                   Name = wc.Category.Name
               })
               .ToList();

            return categoriesForWorker;
        }
        
    }
}
