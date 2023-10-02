using IUstaApi.Models.DTOs.Category;
using IUstaApi.Models.DTOs.Worker;
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



        [HttpGet("profile")]
        public async Task<ActionResult<ProfileDto>> GetProfile() => Ok(await _service.GetWorkerProfile());

        [HttpGet("seeActiveRequests")]
        public ActionResult<IEnumerable<RequestDto>> GetActiveRequests() => Ok(_service.SeeActiveRequests());

        [HttpGet("seeInactiveRequests")]

        public ActionResult<IEnumerable<RequestDto>> GetInactiveRequests() => Ok(_service.SeeInactiveRequests());

        [HttpGet("seeCompletedRequests")]

        public ActionResult<IEnumerable<RequestDto>> GetCompletedRequests() => Ok(_service.SeeCompletedTasks());

        [HttpPost("completeTask")]

        public async Task<ActionResult<bool>> CompleteTask([FromBody] SetWorkDoneRequest request) => Ok(await _service.SetWorkDoneAsync(request));

        [HttpPost("acceptTask")]

        public async Task<ActionResult<bool>> AcceptTask([FromBody] AcceptWorkRequest request) => Ok(await _service.AcceptWorkAsync(request));

        [HttpPost("rejectTask")]

        public async Task<ActionResult<bool>> RejectTask([FromBody] RejectWorkRequest request) => Ok(await _service.RejectWorkAsync(request));
    }
}
