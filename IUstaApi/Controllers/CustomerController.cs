using IUstaApi.Models.DTOs;
using IUstaApi.Models.DTOs.Auth;
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



}