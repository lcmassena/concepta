using Concepta.Application.Interfaces.Repositories;
using Massena.Infrastructure.Core.Web;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Concepta.API.Controllers
{
    [Produces("application/json")]
    [Route("token")]
    [AllowAnonymous]
    public class TokenController : ApiController
    {
        private IUserRepository UserRepository;

        public TokenController(IUserRepository userRepository, IMediator mediator) : base(mediator) => UserRepository = userRepository;

        [HttpPost]
        public async Task<IActionResult> Token([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Token failed to generate");

            var user = UserRepository.GetUser(model.username, model.password);

            if (user == null)
                return Unauthorized();

            //Add Claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, "data"),
                new Claim(JwtRegisteredClaimNames.Sub, "data"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("rlyaKithdrYVl6Z80ODU350md")); //Secret
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken("me",
                "you",
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return Ok(new JsonWebToken()
            {
                access_token = new JwtSecurityTokenHandler().WriteToken(token),
                expires_in = 600000,
                token_type = "bearer"
            });
        }
    }

    public class JsonWebToken
    {
        public string access_token { get; set; }

        public string token_type { get; set; } = "bearer";

        public int expires_in { get; set; }

        public string refresh_token { get; set; }
    }


    public class LoginViewModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string grant_type { get; set; }
    }
}
