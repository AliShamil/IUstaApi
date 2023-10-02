
using IUstaApi.Data;
using IUstaApi.Models.DTOs;
using IUstaApi.Models.DTOs.Category;
using IUstaApi.Models.DTOs.Customer;
using IUstaApi.Models.Entities;
using IUstaApi.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IUstaApi.Services.Concrete
{
    public class CustomerService : ICustomerService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly UstaDbContext _context;

        public CustomerService(UserManager<AppUser> userManager, UstaDbContext context)
        {
            _userManager=userManager;
            _context=context;
        }

        public async Task<IEnumerable<WorkerDto>> GetAllWorkersAsync()
        {
            var workers = await _userManager.GetUsersInRoleAsync("worker");

            var dtos = workers.Select(w => new WorkerDto { Email = w.Email, Rating = CalculateRating(w) });

            return dtos;
        }

        public IEnumerable<WorkerDto> GetWorkersByCategory(string categoryId)
        {
            return _context.WorkerCategories.Where(cw => cw.CategoryId == Guid.Parse(categoryId)).ToList().Select(w => new WorkerDto { Email = w.Worker.Email, Rating = CalculateRating(w.Worker) });
        }

        public IEnumerable<CategoryInfoDto> SeeAllCategories()
        {
            return _context.Categories.Select(c => new CategoryInfoDto { Id = c.Id.ToString(), Name = c.Name });
        }

        public async Task<IEnumerable<WorkerDto>> GetWorkersByRatingAsync(bool desc = true)
        {
            var workers = (await _userManager.GetUsersInRoleAsync("worker")).ToList();
            return workers.OrderBy(w => CalculateRating(w)).Select(w => new WorkerDto { Email = w.Email, Rating = CalculateRating(w) });
        }

        private double CalculateRating(AppUser worker)
        {
            var totalRatings = _context.WorkRequests.Where(r => r.WorkerEmail == worker.Email && r.Rating.HasValue).Select(r => r.Rating.Value).Sum();
            if (_context.WorkRequests.Where(r => r.WorkerEmail == worker.Email && r.Rating.HasValue).Count() != 0)
            {
                var avarageRating = totalRatings / _context.WorkRequests.Where(r => r.WorkerEmail == worker.Email && r.Rating.HasValue).Count();
                return avarageRating;
            }
            return 0;
        }

        public IEnumerable<CustomerRequestDto> GetUsersRequests(string userEmail)
        {
            return _context.WorkRequests
                .Where(wr => wr.ClientEmail == userEmail)
                .Select(wr => new CustomerRequestDto
                {
                    Id = wr.Id.ToString(),
                    IsAccepted = wr.IsAccepted.HasValue ? wr.IsAccepted.Value : false,
                    IsCompleted = wr.IsCompleted,
                    WorkerEmail = wr.WorkerEmail
                });
               
        }

        public async Task<bool> RateWorkDoneAsync(RateWorkDto model)
        {
            try
            {
                var wr = await _context.WorkRequests.FirstOrDefaultAsync(wr=>wr.Id==Guid.Parse(model.Id));
                if (wr is null || wr.IsCompleted is false)
                    return false;

                wr.Rating = model.Rate;
                _context.WorkRequests.Update(wr);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public async Task<bool> SendWorkRequest(WorkRequestDto request)
        {
            try
            {
                var client = await _userManager.FindByEmailAsync(request.UserEmail);
                var worker = await _userManager.FindByEmailAsync(request.WorkerEmail);

                if (client is null || worker is null)
                    return false;

                var workRequest = new WorkRequest
                {
                    ClientEmail = client.Email,
                    WorkerEmail = worker.Email,
                    Message = request.Message,
                    IsCompleted = false
                };

                await _context.WorkRequests.AddAsync(workRequest);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
