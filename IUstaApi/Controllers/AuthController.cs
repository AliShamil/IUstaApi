using IUstaApi.Auth;
using IUstaApi.Models.DTOs.Auth;
using IUstaApi.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using IUstaApi.Mail;

namespace IUstaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtService _jwtService;
        private readonly IMailService _mailService;
        private readonly ILogger<WeatherForecastController> _logger;
        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJwtService jwtService, RoleManager<IdentityRole> roleManager, IMailService mailService, ILogger<WeatherForecastController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _roleManager=roleManager;
            _mailService=mailService;
            _logger=logger;
        }

        private async Task<AuthTokenDto> GenerateToken(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            var accessToken = _jwtService.GenerateSecurityToken(user.Id, user.Email!, roles, claims);

            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken.Token;
            user.TokenExpires = refreshToken.Expires;
            user.TokenCreated = refreshToken.Created;
            await _userManager.UpdateAsync(user);
            return new AuthTokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
            };
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
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
                return BadRequest("You cannot register as admin!Pls select another roles");
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
                var url = Url.Action(nameof(ConfirmEmail), "Auth", new { email = user.Email, token = confirmToken }, Request.Scheme);
                if (url is not null)
                    _mailService.SendConfirmationMessage(user.Email, url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            await _userManager.AddToRoleAsync(user, role);

            return Ok();
        }
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> ConfirmEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is not null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                return Ok();
            }
            return BadRequest();
        }
        [HttpGet("GetRoles")]
        public IActionResult GetRoles()
        {
            var list = _roleManager.Roles.Select(r=>r.Name).ToList();
            return Ok(list);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthTokenDto>> Login(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return BadRequest();
            }
            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                var canSignIn = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, false);

                if (!canSignIn.Succeeded)
                    return BadRequest();

                return await GenerateToken(user);
            }
            return Unauthorized();
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthTokenDto>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(e => e.RefreshToken == request.RefreshToken);

            if (user is null)
                return Unauthorized("Invalid RefreshToken");
            if (user.TokenExpires < DateTime.Now)
                return Unauthorized("Token expired");

            return await GenerateToken(user);
        }
        private RefreshToken GenerateRefreshToken()
        {
            return new RefreshToken()
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };
        }

    }
}
