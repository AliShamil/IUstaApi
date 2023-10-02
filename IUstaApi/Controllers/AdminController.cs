using IUstaApi.Models.DTOs.Auth;
using IUstaApi.Models;
using IUstaApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IUstaApi.Models.DTOs.Category;

namespace IUstaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _service;

        public AdminController(IAdminService service)
        {
            _service = service;
        }

        [HttpPost("addCategory")]
        public async Task<ActionResult<bool>> AddCategory([FromBody] CategoryDto model) => await _service.AddCategoryAsync(model);
        
        [HttpDelete("removeCategory")]
        public async Task<ActionResult<bool>> RemoveCategory([FromBody] string categoryId) => await _service.RemoveCategoryAsync(categoryId);


        [HttpGet("showAllCategories")]
        public ActionResult<IEnumerable<CategoryInfoDto>> GetAllCategories() => Ok(_service.GetAllCategories());

        [HttpPut("updateCategory")]
        public async Task<ActionResult<bool>> UpdateCategory([FromBody] CategoryUpdateDto model) => await _service.UpdateCategoryAsync(model);



        [HttpGet("getStatistics")]
        public ActionResult<Statistics> GetStatistics() => _service.GetStatistics();
    }
}
