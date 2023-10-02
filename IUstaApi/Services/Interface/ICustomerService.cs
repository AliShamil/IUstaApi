using IUstaApi.Models.DTOs;
using IUstaApi.Models.DTOs.Category;
using IUstaApi.Models.DTOs.Customer;

namespace IUstaApi.Services.Interface
{
    public interface ICustomerService
    {
        Task<IEnumerable<WorkerDto>> GetAllWorkersAsync();
        Task<IEnumerable<WorkerDto>> GetWorkersByRatingAsync(bool desc = true);
        IEnumerable<WorkerDto> GetWorkersByCategory(string categoryId);
        IEnumerable<CategoryInfoDto> SeeAllCategories();
        IEnumerable<CustomerRequestDto> GetUsersRequests(string userEmail);
        Task<bool> RateWorkDoneAsync(RateWorkDto model);
        Task<bool> SendWorkRequest(WorkRequestDto request);
    }
}
