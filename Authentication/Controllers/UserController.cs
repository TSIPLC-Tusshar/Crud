using Authentication.Models;
using Authentication.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

namespace Authentication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
           _userService = userService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser(UserRegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.CreateUser(model);
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(s => s.Errors));
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([Required]string username, [Required]string password)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.Authentication(username, password);
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(s => s.Errors));
            }
        }


    }
}
