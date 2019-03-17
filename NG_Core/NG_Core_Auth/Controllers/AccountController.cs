using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NG_Core_Auth.Helpers;
using NG_Core_Auth.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NG_Core_Auth.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        // Inject these services into controller
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signManager;
        private readonly AppSettings _appSettings;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signManager, IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _signManager = signManager;
            _appSettings = appSettings.Value;
        }


        /// <summary>
        /// Registration 
        /// </summary>
        /// <returns>The register.</returns>
        /// <param name="formData">Form data.</param>
        [HttpPost("action")]
        public async Task<IActionResult> Register( [FromBody] RegisterViewModel formData) 
        {
            List<String> errorList = new List<string>();
            var user = new IdentityUser
            { 
                Email = formData.Email,
                UserName = formData.UserName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, formData.Password);

            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");

                // Send confirmation email

                return Ok(new { username = user.UserName, email = user.Email, status = 1, message = "Registration Successful" });
            }
            else
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    errorList.Add(error.Description);
                }
            }

            return BadRequest(new JsonResult(errorList));
        }

        [HttpPost("action")]
        public async Task<IActionResult> Login( [FromBody] LoginViewModel formdata)
        {

            // Get user from database
            var user = await _userManager.FindByNameAsync(formdata.UserName);

            // Token vars
            var roles = await _userManager.GetRolesAsync(user);
            double tokenExpiryTime = Convert.ToDouble(_appSettings.ExpireTime);
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret));

            if (user != null && await _userManager.CheckPasswordAsync(user, formdata.Password))
            {

                // Create token descriptor
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, formdata.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                        new Claim("LoggedOn", DateTime.Now.ToString())
                    }),

                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _appSettings.Site,
                    Audience = _appSettings.Audience,
                    Expires = DateTime.UtcNow.AddMinutes(tokenExpiryTime)
                };

                // Generate token
                var token = tokenHandler.CreateToken(tokenDescriptor);

                // Return token
                return Ok(new { token = tokenHandler.WriteToken(token), expiration = token.ValidTo, username = user.UserName, userRole = roles.FirstOrDefault() });
            }

            // Return error
            ModelState.AddModelError("", "Username / Password not found!");
            return Unauthorized(new { LoginError = "Please verify login credentials - Invalid Username / Password Entered" });

        }

    }
}
