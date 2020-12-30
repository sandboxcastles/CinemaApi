using AuthenticationPlugin;
using CinemaApi.Data;
using CinemaApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private CinemaDbContext _dbContext;
        public UsersController(CinemaDbContext dbContext)
        {
            _dbContext = dbContext;
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
    }
}
