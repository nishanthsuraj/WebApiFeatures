using SampleWebApplication.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static SampleWebApplication.Areas.Identity.Pages.Account.LoginModel;

namespace SampleWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<SampleWebApplicationUser> userManager;

        public TokenController(UserManager<SampleWebApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        // https://localhost:7270/api/Token/Login
        /*
            {
                "Email": "nishanthsuraj@hotmail.com",
                "Password": "Suraj123!"
            }
        */
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] InputModel model)
        {
            var user = await userManager.FindByNameAsync(model.Email);
            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized();
            }
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("V€r¥ $ecret (not!) V€r¥ $ecret (not!) V€r¥ $ecret (not!)"));
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(authClaims),
                Expires = DateTime.Now.AddMinutes(20),
                SigningCredentials = new SigningCredentials(
                    key, SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                expires = token.ValidTo
            });
        }

        // https://localhost:7270/api/Token/Products
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("products")]
        public async Task<IActionResult> GetProducts()
        {
            var httpClient = new HttpClient();

            var response = await httpClient.GetAsync("https://localhost:7056/api/v1.0/products");
            var data = await response.Content.ReadAsStringAsync();

            return Ok(data);
        }
    }
}
