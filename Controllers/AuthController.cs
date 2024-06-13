using ApplicationToSellThings.APIs.Areas.Identity.Data;
using ApplicationToSellThings.APIs.Models;
using ApplicationToSellThings.APIs.Static;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApplicationToSellThings.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<ApplicationToSellThingsAPIsUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        public AuthController(ILogger<AuthController> logger, UserManager<ApplicationToSellThingsAPIsUser> userManager, RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _logger = logger;
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin(RegisterModel model)
        {
            try
            {
                var userExists = await userManager.FindByNameAsync(model.Username);
                if (userExists != null)
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string> { Status = "Error", Message = "Admin User already exists!" });

                ApplicationToSellThingsAPIsUser user = new()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Username,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };
                var result = await userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string> { Status = "Error", Message = "User creation failed! Please check user details and try again." });

                if (!await roleManager.RoleExistsAsync(UserRole.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRole.Admin));

                if (await roleManager.RoleExistsAsync(UserRole.Admin))
                {
                    await userManager.AddToRoleAsync(user, UserRole.Admin);
                }

                return Ok(new ResponseModel<string> { Status = "Success", Message = "Admin User created successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            try
            {
                var userExists = await userManager.FindByNameAsync(model.Username);
                if (userExists != null)
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string> { Status = "Error", Message = "User already exists!" });

                ApplicationToSellThingsAPIsUser user = new()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Username,
                    FirstName = model.FirstName,
                    LastName = model.LastName

                };
                var result = await userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string> { Status = "Error", Message = "User creation failed! Please check user details and try again." });

                var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync("User"))
                {
                    await roleManager.CreateAsync(new IdentityRole("User"));
                }

                await userManager.AddToRoleAsync(user, UserRole.User);

                return Ok(new ResponseModel<string> { Status = "Success", Message = "User created successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
                {
                    var userRoles = await userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString())
                    };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    string token = GenerateToken(authClaims);

                    return Ok(new ResponseModel<string>{ Status = "Success", StatusCode = 200, Message = "User Logged In successfully!", Data = token, Items = (List<string>)userRoles });

                }
                return Ok(new ResponseModel<string> { Status = "Error", StatusCode = 404, Message = "Invalid username or password" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                return Ok(new ResponseModel<UserDetailApiModel>
                {
                    Status = "Success",
                    StatusCode = 200,
                    Message = "User Details Successful",
                    Data = new UserDetailApiModel
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private string GenerateToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_configuration["JWT:Secret"]));
            var TokenExpiryTimeInHour = Convert.ToInt64(_configuration["JWT:TokenExpiryTimeInHour"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWT:ValidIssuer"],
                Audience = _configuration["JWT:ValidAudience"],
                Expires = DateTime.UtcNow.AddHours(TokenExpiryTimeInHour),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(authClaims),
                
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
