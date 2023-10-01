
using Azure.Core;
using IUstaApi.Data;
using IUstaApi.Models.DTOs;
using IUstaApi.Models.DTOs.Category;
using IUstaApi.Models.Entities;
using IUstaApi.Providers;
using IUstaApi.Services.Interface;

namespace IUstaApi.Services.Concrete
{
    public class WorkerService : IWorkerService
    {
        private readonly UstaDbContext _context;
        private readonly IWorkerCategoryService _service;
        private readonly IRequestUserProvider _provider;

        public WorkerService(UstaDbContext context, IRequestUserProvider provider, IWorkerCategoryService service)
        {
            _context=context;
            _provider=provider;
            _service=service;
        }

        public IEnumerable<CategoryInfoDto> GetAllCategories() => _context.Categories.Select(c => new CategoryInfoDto { Id = c.Id.ToString(), Name = c.Name});


        public  IEnumerable<CategoryInfoDto> GetOwnCategories()
        {
           var result = _service.GetAllCategoriesByWorkerId(_provider.GetUserInfo().id);
           return result;
        }

        public async Task<bool> JoinToCategory(string categoryId)
        {
            try
            {
                var workerCategory = new WorkerCategoryDto()
                {
                    CategoryId = categoryId,
                    WorkerId = _provider.GetUserInfo().id
                };
                var result = await _service.AddAsync(workerCategory);

                return result;
            }
            catch (Exception)
            {
                return false;
            }
          
        }

        public Task<bool> LeaveFromCategory(string categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendWorkRequest(WorkRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
}
