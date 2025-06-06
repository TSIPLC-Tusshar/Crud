using Authentication.Data;
using Authentication.Data.Entities;
using Authentication.Models;
using Authentication.Models.DTOs;
using Authentication.Services.Interfaces;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Authentication.Services
{
    public class UserService : IUserService
    {
        private readonly AuthDbContext _dbContext;
        private readonly SignInManager<AspNetUser> _signInManager;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(AuthDbContext dbContext,
            SignInManager<AspNetUser> signInManager,
            UserManager<AspNetUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<LoginDto>> Authentication(string username, string password)
        {
            var userMaster = await _dbContext.AspNetUsers.FirstOrDefaultAsync(x => x.UserName == username && x.UserMaster.IsActive);
            if (userMaster == null)
            {
                return new ResponseDto<LoginDto>()
                {
                    Message = "User has been de-activated.",
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            var signInResult = await _signInManager.PasswordSignInAsync(userMaster, password, isPersistent: false, lockoutOnFailure: true);
            if (signInResult.IsLockedOut)
            {
                return new ResponseDto<LoginDto>()
                {
                    Message = "User has been blocked due to invalid attempts of login.",
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            if (!signInResult.Succeeded)
            {
                return new ResponseDto<LoginDto>()
                {
                    Message = $"Username or password was incorrect. Retry attempt {userMaster.AccessFailedCount}/5",
                    StatusCode = HttpStatusCode.BadRequest
                };
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

            return new ResponseDto<LoginDto>()
            {
                Message = $"User authenticated successfully.",
                StatusCode = HttpStatusCode.OK,
                Success = true,
                Data = new LoginDto()
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    ExpiredOn = token.ValidTo,
                    SessionId = _httpContextAccessor.HttpContext.Session.Id
                }
            };
        }

        public async Task<ResponseDto> CreateUser(UserRegistrationModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                return new ResponseDto()
                {
                    Message = "User already exist with the same email. Please try with another email.",
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            string username = $"User000{await _dbContext.AspNetUsers.CountAsync() + 1}";

            var userData = new AspNetUser()
            {
                Email = model.Email,
                PhoneNumber = model.Mobile,
                UserName = username
            };
            string password = "Test@1234";

            var result = await _userManager.CreateAsync(userData, password);
            if (!result.Succeeded)
            {
                return new ResponseDto()
                {
                    Message = "User registration failed!",
                    StatusCode = HttpStatusCode.BadRequest
                };
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

            return new ResponseDto()
            {
                Message = "User has been created successfully.",
                Success = true,
                StatusCode = HttpStatusCode.OK
            };
        }

        private JwtSecurityToken GenerateJSONWebToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("lnO0xg5lemIXjvxSgMqkffuJv0eGrKHB"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken("PracticeIssuer",
                "PracticeIssuer",
                claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials);

            return token;
        }
    }
}
