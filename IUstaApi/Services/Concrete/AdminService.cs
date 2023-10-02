using IUstaApi.Data;
using IUstaApi.Models;
using IUstaApi.Models.DTOs.Admin;
using IUstaApi.Models.DTOs.Auth;
using IUstaApi.Models.DTOs.Category;
using IUstaApi.Models.Entities;
using IUstaApi.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IUstaApi.Services.Concrete
{
    public class AdminService : IAdminService
    {
        private readonly UstaDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminService(UstaDbContext context, UserManager<AppUser> userManager)
        {
            _context=context;
            _userManager=userManager;
        }

        public async Task<bool> AddCategoryAsync(CategoryDto model)
        {
            try
            {
                var category = new Category { Id = Guid.NewGuid(), Name = model.Name, Description = model.Description, CreatedTime=DateTime.Now };
                await _context.AddAsync(category);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public IEnumerable<CategoryInfoDto> GetAllCategories() => _context.Categories.Select(c => new CategoryInfoDto { Id = c.Id.ToString(), Name = c.Name });

        public async Task<IEnumerable<AppUserInfo>> GetAllUsers()
        {
            // Retrieve all users from the context
            var users = await _context.Users.ToListAsync();

            // Retrieve roles for each user
            var userRoles = new Dictionary<string, List<string>>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.ToList();
            }

            // Create AppUserInfo instances
            var appUserInfos = users.Select(user => new AppUserInfo
            {
                Email = user.Email,
                Roles = userRoles.ContainsKey(user.Id) ? userRoles[user.Id] : new List<string>()
            }).ToList();

            return appUserInfos;
            // 'appUserInfos' now contains the desired AppUserInfo instances with roles

        }

        public Statistics GetStatistics()
        {
            var workRequests = _context.WorkRequests.ToList();
            var totalRequestsCount = workRequests.Count();
            var yearlyRequestsCount = workRequests.Where(wr => wr.CreatedTime.Year == DateTime.Now.Year).Count();
            var monthlyRequestsCount = workRequests.Where(wr => wr.CreatedTime.Year == DateTime.Now.Year && wr.CreatedTime.Month == DateTime.Now.Month).Count();
            var dailyRequestsCount = workRequests.Where(wr => wr.CreatedTime == DateTime.Today).Count();
            var acceptedTasks = workRequests.Where(wr => wr.IsAccepted.HasValue && wr.IsAccepted.Value).Count();
            var completedTasks = workRequests.Where(wr => wr.IsCompleted).Count();

            return new Statistics
            {
                TotalWorkRequests = totalRequestsCount,
                ThisYearWorkRequests = yearlyRequestsCount,
                ThisMonthWorkRequests = monthlyRequestsCount,
                TodayWorkRequests = dailyRequestsCount,
                AcceptedWorkRequest = acceptedTasks,
                ComlpetedWorks = completedTasks
            };
        }

        public async Task<bool> RemoveCategoryAsync(string categoryId)
        {
            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id==Guid.Parse(categoryId));
                _context.Categories.Remove(category);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveUser(RemoveUserDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return false;
            if(await _userManager.IsInRoleAsync(user, "admin"))
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCategoryAsync(CategoryUpdateDto model)
        {
            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == Guid.Parse(model.Id));

                if (category == null)
                    return false;

                category.Name = model.Name;
                category.Description = model.Description;

                _context.Categories.Update(category);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
