using Catstagram.Server.Data.Models;
using Catstagram.Server.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Catstagram.Server.Controllers
{
    
    public  class IdentityController : ApiController
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationSettings _appSettings;

        public IdentityController(UserManager<User> userManager, IOptions<ApplicationSettings> appSettings)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;

        }
        [Route(nameof(Register))]
        public async Task<ActionResult> Register(RegisterRequestModel model)
        {
            var user = new User
            {
                Email = model.Email,
                UserName = model.UserName
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if(result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [Route(nameof(Login))]
        public async Task<ActionResult<string>> Login(LoginRequestModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if(user==null)
            {
                return Unauthorized();
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if(!passwordValid)
            {
                return Unauthorized();
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject=new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name,model.UserName)
                }
                    ),
                Expires=DateTime.UtcNow.AddDays(7),
                SigningCredentials=new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var encryptedToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(encryptedToken);
        }
    }
}
