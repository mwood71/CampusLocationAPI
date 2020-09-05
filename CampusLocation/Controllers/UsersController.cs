using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CampusLocation.Contracts;
using CampusLocation.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CampusLocation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;

        public UsersController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILoggerService logger,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _config = config;
        }


        /// <summary>
        /// User login endpoint 
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns>user login</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UsersDTO userDTO)
        {

            var location = GetControllerActionNames();

            try
            {
                _logger.LogInfo($"{location}: user login requested");

                var username = userDTO.UserName;
                var password = userDTO.Password;
                var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

                if (result.Succeeded)
                {
                    _logger.LogInfo($"{location}: user successfully logged in");
                    var user = await _userManager.FindByNameAsync(username);
                    var tokenString = await GenerateJSONWebToken(user);
                    return Ok(new { token = tokenString });
                }

                _logger.LogWarn($"User login was not authorized");
                return Unauthorized(userDTO);

            }
            catch (Exception e)
            {

                return InternalError($"{e.Message} - {e.InnerException}");
            }




        }

        private async Task<string> GenerateJSONWebToken(IdentityUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r)));

            var token = new JwtSecurityToken(_config["Jwt:Issuer"]
                , _config["Jwt:Issuer"],
                claims,
                null,
                expires: DateTime.Now.AddMinutes(2),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Soemthing went wrong, please contact admin");
        }
        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

    }
}
