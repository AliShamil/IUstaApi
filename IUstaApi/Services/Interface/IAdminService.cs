using IUstaApi.Models;
using IUstaApi.Models.DTOs.Auth;
using IUstaApi.Models.DTOs.Category;

namespace IUstaApi.Services.Interface
{
    public interface IAdminService
    {
        public Task<bool> AddCategoryAsync(CategoryDto model);
        public Task<bool> UpdateCategoryAsync(CategoryUpdateDto model);
        public Task<bool> RemoveCategoryAsync(string categoryId);
        public IEnumerable<CategoryInfoDto> GetAllCategories();
        public Statistics GetStatistics();
    }
}
