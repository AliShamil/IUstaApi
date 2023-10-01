using IUstaApi.Models.DTOs;
using IUstaApi.Models.DTOs.Category;

namespace IUstaApi.Services.Interface
{
    public interface IWorkerCategoryService
    {
        public Task<bool> AddAsync(WorkerCategoryDto model);
        public Task<bool> RemoveAsync(string id);
        public IEnumerable<CategoryInfoDto> GetAllCategoriesByWorkerId(string id);
    }
}
