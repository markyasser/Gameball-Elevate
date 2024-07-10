using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gameball_Elevate.Models;
using Microsoft.AspNetCore.Authorization;
using Gameball_Elevate.Utils;

namespace Gameball_Elevate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            var user = new User
            {
                UserName = registerRequest.Username,
                Email = registerRequest.Email,
                Points = 0, 
            };

            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Check if the "Admin" role exists, create if not (optional)
            var adminRoleExists = await _roleManager.RoleExistsAsync("Admin");
            if (!adminRoleExists)
            {
                var adminRole = new IdentityRole("Admin");
                var createRoleResult = await _roleManager.CreateAsync(adminRole);
                if (!createRoleResult.Succeeded)
                {
                    return BadRequest(createRoleResult.Errors);
                }
            }

            // Assign role to the user ("Admin")
            var addToRoleResult = await _userManager.AddToRoleAsync(user, "Admin");

            if (!addToRoleResult.Succeeded)
            {
                return BadRequest(addToRoleResult.Errors);
            }

            // Generate JWT token with roles
            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);

            // Return token in response
            return Ok(new { Token = token });
        }

        [HttpPost("CreateCustomer"), Authorize(Roles ="Admin")]
        public async Task<IActionResult> CreateCustomer([FromBody] RegisterRequest registerRequest)
        {
            if (registerRequest == null)
            {
                return BadRequest("Invalid user data.");
            }

            var user = new User { 
                UserName = registerRequest.Username,
                Email = registerRequest.Email,
                Points = 0,
            };
            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Check if the "Customer" role exists, create if not (optional)
            var adminRoleExists = await _roleManager.RoleExistsAsync("Customer");
            if (!adminRoleExists)
            {
                var adminRole = new IdentityRole("Customer");
                var createRoleResult = await _roleManager.CreateAsync(adminRole);
                if (!createRoleResult.Succeeded)
                {
                    return BadRequest(createRoleResult.Errors);
                }
            }

            // Assign role to the user ("Customer")
            var addToRoleResult = await _userManager.AddToRoleAsync(user, "Customer");

            if (!addToRoleResult.Succeeded)
            {
                return BadRequest(addToRoleResult.Errors);
            }


            return Ok("Customer user created successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
                return Unauthorized(new { Error = "Invalid username or password" });
            }

            // Retrieve user roles
            var roles = await _userManager.GetRolesAsync(user);

            // Generate JWT token with roles
            var token = GenerateJwtToken(user, roles);

            // Return token in response
            return Ok(new { Token = token });
        }


        private string GenerateJwtToken(User user, IList<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("P8tUgRSTUvfd6pRaECNzd2wbqQ531K4P_k1V3PHNKs0="); // Ensure this is at least 256 bits

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            // Add roles as claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
