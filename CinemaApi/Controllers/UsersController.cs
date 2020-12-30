using AuthenticationPlugin;
using CinemaApi.Data;
using CinemaApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CinemaApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private CinemaDbContext _dbContext;
        private IConfiguration _configuration;
        private readonly AuthService _auth;
        public UsersController(CinemaDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _auth = new AuthService(_configuration);
        }

        [HttpPost]
        public IActionResult Register([FromBody] User user)
        {
            if (_dbContext.Users.Any(u => u.Email == user.Email))
            {
                return BadRequest("User exists with this email");
            }

            var password = SecurePasswordHasherHelper.Hash(user.Password);

            User newUser = new User
            {
                Name = user.Name,
                Email = user.Email,
                Password = password,
                Role = "Users"
            };

            _dbContext.Users.Add(newUser);
            if (_dbContext.SaveChanges() == 0)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost]
        public IActionResult Login([FromBody] User user)
        {
            User userByEmail = _dbContext.Users.FirstOrDefault(u => u.Email == user.Email);
            if (userByEmail == null)
            {
                return NotFound();
            }

            if(!SecurePasswordHasherHelper.Verify(user.Password, userByEmail.Password))
            {
                return Unauthorized();
            }

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, userByEmail.Role)
            };
            var token = _auth.GenerateAccessToken(claims);
            return new ObjectResult(new
            {
                access_token = token.AccessToken,
                expires_in = token.ExpiresIn,
                token_type = token.TokenType,
                creation_Time = token.ValidFrom,
                expiration_Time = token.ValidTo,
                user_id = userByEmail.Id
            });
        }
    }
}
