
using Azure.Core;
using IUstaApi.Data;
using IUstaApi.Models.DTOs;
using IUstaApi.Models.DTOs.Category;
using IUstaApi.Models.DTOs.Worker;
using IUstaApi.Models.Entities;
using IUstaApi.Providers;
using IUstaApi.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IUstaApi.Services.Concrete
{
    public class WorkerService : IWorkerService
    {
        private readonly UstaDbContext _context;
        private readonly IWorkerCategoryService _service;
        private readonly IRequestUserProvider _provider;
        private readonly UserManager<AppUser> _userManager;

        public WorkerService(UstaDbContext context, IRequestUserProvider provider, IWorkerCategoryService service, UserManager<AppUser> userManager)
        {
            _context=context;
            _provider=provider;
            _service=service;
            _userManager=userManager;
        }


        public IEnumerable<CategoryInfoDto> GetAllCategories() => _context.Categories.Select(c => new CategoryInfoDto { Id = c.Id.ToString(), Name = c.Name });


        public IEnumerable<CategoryInfoDto> GetOwnCategories()
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
        public async Task<bool> LeaveFromCategory(string categoryId)
        {
            var wc = _context.WorkerCategories.FirstOrDefault(wc => wc.WorkerId == _provider.GetUserInfo().id && wc.CategoryId == Guid.Parse(categoryId));
            if (wc == null)
                return false;
            try
            {
                await _service.RemoveAsync(wc.Id.ToString());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<bool> AcceptWorkAsync(AcceptWorkRequest request)
        {
            try
            {
                var worker = await _userManager.FindByEmailAsync(request.WorkerEmail);
                if (worker is not null)
                {
                    var workRequest = await _context.WorkRequests.FirstOrDefaultAsync(wr=> wr.Id == Guid.Parse(request.TaskId));
                    if (workRequest is null || workRequest.WorkerEmail != worker.Email)
                        return false;

                    workRequest.IsAccepted = true;
                    _context.WorkRequests.Update(workRequest);
                    await _context.SaveChangesAsync();
                    
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ProfileDto?> GetWorkerProfile(string email)
        {
            try
            {
                var worker = await _userManager.FindByEmailAsync(email);
                if (worker is not null)
                {
                    var requests = _context.WorkRequests.Where(r => r.WorkerEmail == worker.Email).ToList();
                    var totalRequests = requests.Count;
                    var inactiveRequests = requests.Where(r => !r.IsAccepted.HasValue).Count();
                    var activeRequests = requests.Where(r => r.IsAccepted.HasValue && r.IsAccepted.Value).Count();
                    var completedRequests = requests.Where(r => r.IsCompleted).Count();
                    var rating = _context.WorkRequests.Where(r => r.WorkerEmail == worker.Email && r.Rating.HasValue).Select(r => r.Rating.Value).Sum();
                    if (_context.WorkRequests.Where(r => r.WorkerEmail == worker.Email && r.Rating.HasValue).Any())
                        rating /= _context.WorkRequests.Where(r => r.WorkerEmail == worker.Email && r.Rating.HasValue).Count();
                    else
                        rating = 0;


                    var dto = new ProfileDto
                    {
                        Email = worker.Email,
                        ActiveRequestsCount = activeRequests,
                        CompletedRequestsCount = completedRequests,
                        InactiveRequestsCount = inactiveRequests,
                        TotalRequestsCount = totalRequests,
                        Rating = rating
                    };
                    return dto;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> RejectWorkAsync(RejectWorkRequest request)
        {
            var worker = await _userManager.FindByEmailAsync(request.WorkerEmail);
            if (worker is not null)
            {
                var workRequest = await _context.WorkRequests.FirstOrDefaultAsync(wr=> wr.Id == Guid.Parse(request.TaskId));
                if (workRequest is null || workRequest.WorkerEmail != worker.Email)
                    return false;

                workRequest.IsAccepted = false;
                _context.WorkRequests.Update(workRequest);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public IEnumerable<RequestDto> SeeActiveRequests(string email)
                    => _context.WorkRequests
                    .Where(r => r.WorkerEmail == email && r.IsAccepted.HasValue && r.IsAccepted.Value)
                    .Select(r => new RequestDto { Id = r.Id.ToString(), Message = r.Message, UserMail = r.ClientEmail });

        public IEnumerable<RequestDto> SeeCompletedTasks(string email)
             => _context.WorkRequests
                 .Where(r => r.WorkerEmail == email || r.IsCompleted)
                 .Select(r => new RequestDto { Id = r.Id.ToString(), Message = r.Message, UserMail = r.ClientEmail });

        public IEnumerable<RequestDto> SeeInactiveRequests(string email)
            => _context.WorkRequests
                .Where(r => r.WorkerEmail == email && !r.IsAccepted.HasValue)
                .Select(r => new RequestDto { Id = r.Id.ToString(), Message = r.Message, UserMail = r.ClientEmail });

        public async Task<bool> SetWorkDoneAsync(SetWorkDoneRequest request)
        {
            try
            {

            }
            catch (Exception)
            {
                return false;
            }
            var worker = await _userManager.FindByEmailAsync(request.WorkerEmail);
            if (worker is not null)
            {
                var workRequest = await _context.WorkRequests.FirstOrDefaultAsync(wr=>wr.Id == Guid.Parse(request.TaskId));
                if (workRequest is null || workRequest.WorkerEmail != worker.Email || !workRequest.IsAccepted.Value)
                    return false;

                workRequest.IsCompleted = true;
                _context.WorkRequests.Update(workRequest);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
