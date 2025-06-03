using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using MySqlAuthAPI.Data;
using MySqlAuthAPI.Data.Entities;
using MySqlAuthAPI.Models;

using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MySqlAuthAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UserController : ControllerBase
    {
        private readonly MySqlDbContext _dbContext;
        private readonly SignInManager<Aspnetuser> _signInManager;
        private readonly UserManager<Aspnetuser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(MySqlDbContext dbContext,
            SignInManager<Aspnetuser> signInManager,
            UserManager<Aspnetuser> userManager,
            RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser(UserRegistrationModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        return BadRequest("User already exist with the same email. Please try with another email.");
                    }

                    string username = $"User000{await _dbContext.Aspnetusers.CountAsync() + 1}";

                    var userData = new Aspnetuser()
                    {
                        Email = model.Email,
                        PhoneNumber = model.Mobile,
                        UserName = username
                    };
                    string password = "Test@1234";

                    var result = await _userManager.CreateAsync(userData, password);
                    if (!result.Succeeded)
                    {
                        return BadRequest("User registration failed!");
                    }

                    if (!await _roleManager.RoleExistsAsync("User"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("User"));
                    }

                    if (await _roleManager.RoleExistsAsync("User"))
                    {
                        await _userManager.AddToRoleAsync(userData, "User");
                    }

                    UserMaster userMaster = new UserMaster()
                    {
                        CreatedOn = DateTime.Now,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        IsActive = true,
                        PhoneNumber = model.Mobile,
                        UserId = userData.Id
                    };

                    _dbContext.UserMasters.Add(userMaster);
                    _dbContext.SaveChanges();

                    return Ok(result);
                }
                else
                {
                    return BadRequest(ModelState.Values.SelectMany(x => x.Errors));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([Required]string username, [Required]string password)
        {
            if (ModelState.IsValid)
            {
                var userMaster = await _dbContext.Aspnetusers.FirstOrDefaultAsync(x => x.UserName == username && x.UserMaster.IsActive);
                if (userMaster == null)
                {
                    return BadRequest("User has been de-activated.");
                }

                var signInResult = await _signInManager.PasswordSignInAsync(userMaster, password, isPersistent: false, lockoutOnFailure: true);
                if (signInResult.IsLockedOut)
                {
                    return BadRequest($"User has been blocked due to invalid attempts of login.");
                }
                
                if (!signInResult.Succeeded)
                {
                    return BadRequest($"Username or password was incorrect. Retry attempt {userMaster.AccessFailedCount}/5");
                }

                var userRoles = await _userManager.GetRolesAsync(userMaster);

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim(ClaimTypes.Sid, _httpContextAccessor.HttpContext.Session.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                foreach (var userRole in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GenerateJSONWebToken(claims);

                var loginSession = await _dbContext.LoginSessions.Where(x => x.UserId == userMaster.Id).ToListAsync();
                if (loginSession.Any())
                {
                    await _dbContext.LoginSessions.Where(x => x.UserId == userMaster.Id).ExecuteUpdateAsync(u =>
                    u.SetProperty(p => p.IsSessionExpired, true)
                    .SetProperty(p => p.ExpiredOn, DateTime.Now));
                }

                await _dbContext.LoginSessions.AddAsync(new LoginSession()
                {
                    CreatedOn = DateTime.Now,
                    ExpiredOn = DateTime.Now,
                    IsSessionExpired = false,
                    LoginFrom = "WEB",
                    SessionName = _httpContextAccessor.HttpContext.Session.Id,
                    UserId = userMaster.Id,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Validity = token.ValidTo.ToString()
                });

                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    ExpiredOn = token.ValidTo.ToString(),
                    SessionId = _httpContextAccessor.HttpContext.Session.Id
                });
            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(s => s.Errors));
            }
        }

        private JwtSecurityToken GenerateJSONWebToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("lnO0xg5lemIXjvxSgMqkffuJv0eGrKHB"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken("MySqlIssuer",
                "MySqlIssuer", 
                claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials);

            return token;
        }
    }
}
