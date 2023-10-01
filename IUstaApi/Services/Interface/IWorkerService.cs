using IUstaApi.Models.DTOs;
using IUstaApi.Models.DTOs.Category;

namespace IUstaApi.Services.Interface
{
    public interface IWorkerService
    {
       Task<bool>JoinToCategory(string categoryId);
       Task<bool>LeaveFromCategory(string categoryId);
       IEnumerable<CategoryInfoDto> GetAllCategories();
       IEnumerable<CategoryInfoDto> GetOwnCategories();
       Task<bool> SendWorkRequest(WorkRequestDto request);
    }

}
