using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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


        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signManager)
        {
            _userManager = userManager;
            _signManager = signManager;
        }

        // This would be the action name in the address bar. e.g., ex.com/api/account/register
        // so we use the action attribute.
        // FromBody attribute says that our information is going to come from the body of our front-end app
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

    }
}
