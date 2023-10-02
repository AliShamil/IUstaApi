using IUstaApi.Models.DTOs.Auth;
using IUstaApi.Models;
using IUstaApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IUstaApi.Models.DTOs.Category;
using Azure.Core;
using IUstaApi.Models.Entities;
using Microsoft.AspNetCore.Identity;
using IUstaApi.Models.DTOs.Admin;
using Microsoft.Extensions.Caching.Memory;

namespace IUstaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _service;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMemoryCache _memoryCache;
        public AdminController(IAdminService service, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IMemoryCache memoryCache)
        {
            _service = service;
            _userManager=userManager;
            _roleManager=roleManager;
            _memoryCache=memoryCache;
        }

        [HttpPost("addCategory")]
        public async Task<ActionResult<bool>> AddCategory([FromBody] CategoryDto model) => await _service.AddCategoryAsync(model);

        [HttpPost("addUser")]
        public async Task<ActionResult<bool>> AddUser([FromBody] RegisterRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser is not null)
                return Conflict("User already exists");

            var user = new AppUser
            {
                UserName = request.Email,
                Email = request.Email,
            };
            var role = request.Role.Trim().ToLower();

            if (role=="admin")
            {
                return BadRequest("You cannot register as admin! Only 1 admin is required!");
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                return NotFound("Role not found!"); ;
            }

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            try
            {
                var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _userManager.AddToRoleAsync(user, role);
                await _userManager.ConfirmEmailAsync(user, confirmToken);
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
            }
            return Ok();
        }

        [HttpDelete("removeUser")]
        public async Task<ActionResult<bool>> RemoveUser([FromBody] RemoveUserDto userDto) => await _service.RemoveUser(userDto);       
        
        [HttpDelete("removeCategory")]
        public async Task<ActionResult<bool>> RemoveCategory([FromBody] string categoryId) => await _service.RemoveCategoryAsync(categoryId);


        [HttpGet("showAllCategories")]
        public ActionResult<IEnumerable<CategoryInfoDto>> GetAllCategories() => Ok(_service.GetAllCategories());

        //[HttpGet("showAllUsers")]
        //public ActionResult<IEnumerable<AppUserInfo>> GetAllUsers() => Ok(_service.GetAllUsers().Result);

        [HttpGet("showAllUsers")]
        public ActionResult<IEnumerable<AppUserInfo>> GetAllUsers()
        {
            // Check if the data is available in the cache
            if (_memoryCache.TryGetValue("AllUsers", out IEnumerable<AppUserInfo> cachedUsers))
            {
                return Ok(cachedUsers);
            }

            // Data was not found in the cache, fetch and cache the data
            var users = _service.GetAllUsers().Result;

            // Cache the data with a specified cache expiration time
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Adjust the expiration time as needed
            };

            _memoryCache.Set("AllUsers", users, cacheEntryOptions);

            return Ok(users);
        }

        [HttpPut("updateCategory")]
        public async Task<ActionResult<bool>> UpdateCategory([FromBody] CategoryUpdateDto model) => await _service.UpdateCategoryAsync(model);



        //[HttpGet("getStatistics")]
        //public ActionResult<Statistics> GetStatistics() => _service.GetStatistics();

        [HttpGet("getStatistics")]
        public ActionResult<Statistics> GetStatistics()
        {
            if (_memoryCache.TryGetValue("Statistics", out Statistics cachedStatistics))
            {
                return Ok(cachedStatistics);
            }

            var statistics = _service.GetStatistics();

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            };

            _memoryCache.Set("Statistics", statistics, cacheEntryOptions);

            return Ok(statistics);
        }

    }
}
