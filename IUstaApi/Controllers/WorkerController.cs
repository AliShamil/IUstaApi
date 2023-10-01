using IUstaApi.Models.DTOs.Category;
using IUstaApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IUstaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "worker")]
    public class WorkerController : ControllerBase
    {
        private readonly IWorkerService _service;

        public WorkerController(IWorkerService service)
        {
            _service=service;
        }

        [HttpGet("ShowAllCategories")]
        public ActionResult<IEnumerable<CategoryInfoDto>> GetAllCategories() => Ok(_service.GetAllCategories());
        [HttpGet("ShowOwnCategories")]
        public ActionResult<IEnumerable<CategoryInfoDto>> GetOwnCategories() => Ok(_service.GetOwnCategories());
        [HttpPost("JoinToCategory")]
        public async Task<ActionResult<bool>> JoinToCategory([FromBody] string categoryId) => await _service.JoinToCategory(categoryId);        
        [HttpDelete("LeaveFromCategory")]
        public async Task<ActionResult<bool>> LeaveFromCategory([FromBody] string categoryId) => await _service.LeaveFromCategory(categoryId);
    }
}
