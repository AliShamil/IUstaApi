using IUstaApi.Models.DTOs;
using IUstaApi.Models.DTOs.Auth;
using IUstaApi.Models.DTOs.Category;
using IUstaApi.Models.DTOs.Customer;
using IUstaApi.Models.Entities;
using IUstaApi.Providers;
using IUstaApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IUstaApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "customer")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _service;

    public CustomerController(ICustomerService service)
    {
        _service=service;
    }
    [HttpGet("getAllWorkers")]
    public async Task<ActionResult<IEnumerable<WorkerDto>>> GetAllWorkers() => Ok(await _service.GetAllWorkersAsync());

    [HttpGet("seeCategories")]
    public ActionResult<IEnumerable<CategoryInfoDto>> SeeAllCategories() => Ok(_service.SeeAllCategories());

    [HttpGet("getWorkersByRating")]
    public async Task<ActionResult<IEnumerable<WorkerDto>>> GetWorkersByRating(bool Descending) => Ok(await _service.GetWorkersByRatingAsync());

    [HttpGet("getWorkersByCategory")]
    public ActionResult<IEnumerable<WorkerDto>> GetWorkersByCategory(string categoryId) => Ok(_service.GetWorkersByCategory(categoryId));

    [HttpGet("seeMyRequests")]
    public ActionResult<CustomerRequestDto> GetAllRequests() => Ok(_service.GetUsersRequests());

    [HttpPost("rateWork")]
    public async Task<ActionResult<bool>> RateWork([FromBody] RateWorkDto model)
    {
        var result = await _service.RateWorkDoneAsync(model);

        if (!result)
            return BadRequest(result);

        return result;
    }

    [HttpPost("sendWorkRequest")]
    
    public async Task<ActionResult<bool>> SendWorkRequest([FromBody] WorkRequestDto request)
    {
        var result = await _service.SendWorkRequest(request);

        if (!result)
            return BadRequest(request);

        return result;
    }

}