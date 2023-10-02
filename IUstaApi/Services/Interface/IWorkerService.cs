using IUstaApi.Models.DTOs;
using IUstaApi.Models.DTOs.Category;
using IUstaApi.Models.DTOs.Worker;

namespace IUstaApi.Services.Interface
{
    public interface IWorkerService
    {
        Task<bool> JoinToCategory(string categoryId);
        Task<bool> LeaveFromCategory(string categoryId);
        IEnumerable<CategoryInfoDto> GetAllCategories();
        IEnumerable<CategoryInfoDto> GetOwnCategories();

        Task<ProfileDto?> GetWorkerProfile(string email);
        Task<bool> AcceptWorkAsync(AcceptWorkRequest request);
        Task<bool> RejectWorkAsync(RejectWorkRequest request);
        Task<bool> SetWorkDoneAsync(SetWorkDoneRequest requestId);
        IEnumerable<RequestDto> SeeInactiveRequests(string email);
        IEnumerable<RequestDto> SeeActiveRequests(string email);
        IEnumerable<RequestDto> SeeCompletedTasks(string email);
    }

}
