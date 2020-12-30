using CinemaApi.Data;
using CinemaApi.DTOModels;
using CinemaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static CinemaApi.Extensions.IQueryableExtensions;

namespace CinemaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private CinemaDbContext _dbContext;

        public MoviesController(CinemaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize]
        [HttpGet("[action]")]
        public IActionResult AllMovies(string sort, int? pageNumber, int? pageSize)
        {
            var movies = from movie in _dbContext.Movies
                         select new MovieSummary
                         {
                             Id = movie.Id,
                             Name = movie.Name,
                             Duration = movie.Duration,
                             Language = movie.Language,
                             Rating = movie.Rating,
                             Genre = movie.Genre,
                             ImageUrl = movie.ImageUrl,
                             TrailerUrl = movie.TrailerUrl
                         };

            return Ok(movies.OrderSkipTake(sort, m => m.Rating, pageNumber, pageSize));
        }

        [Authorize]
        [HttpGet("[action]/{id}")]
        public IActionResult MovieDetail(int id)
        {
            var movie = _dbContext.Movies.Find(id);

            return movie == null
                ? NotFound()
                : Ok(movie);
        }

        [Authorize]
        [HttpGet("[action]")]
        public IActionResult FindMovies(string movieName)
        {
            var movies = from movie in _dbContext.Movies
                         where movie.Name.ToLower().Contains(movieName.ToLower())
                         select new
                         {
                             Id = movie.Id,
                             Name = movie.Name,
                             ImageUrl = movie.ImageUrl
                         };
            return Ok(movies);
        }


        // POST api/<MoviesController>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Post([FromForm] Movie movieObj)
        {
            string filePath = UploadFile(movieObj.Image);
            if (!String.IsNullOrWhiteSpace(filePath))
            {
                movieObj.ImageUrl = filePath;
            }
            var savedMovie = _dbContext.Movies.Add(movieObj);
            if (_dbContext.SaveChanges() > 0)
            {
                return Created($"https://localhost:44346/api/Movies/{savedMovie.Entity.Id}", savedMovie.Entity);
            }
            return StatusCode(StatusCodes.Status400BadRequest);
        }

        // PUT api/<MoviesController>/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromForm] Movie movieObj)
        {
            Movie movie = _dbContext.Movies.Find(id);
            if (movie == null)
            {
                return NotFound("No Record found with this Id");
            }

            string filePath = UploadFile(movieObj.Image);
            if (!String.IsNullOrWhiteSpace(filePath))
            {
                movie.ImageUrl = filePath;
            }

            movie.Name = movieObj.Name;
            movie.Description = movieObj.Description;
            movie.Language = movieObj.Language;
            movie.Duration = movieObj.Duration;
            movie.PlayingDate = movieObj.PlayingDate;
            movie.PlayingTime = movieObj.PlayingTime;
            movie.Rating = movieObj.Rating;
            movie.Genre = movieObj.Genre;
            movie.TrailerUrl = movieObj.TrailerUrl;
            movie.TicketPrice = movieObj.TicketPrice;
            _dbContext.SaveChanges();
            return Ok(movie);
        }

        // DELETE api/<MoviesController>/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Movie movie = _dbContext.Movies.Find(id);
            if (movie == null)
            {
                return NotFound("No Record found with this Id");
            }
            _dbContext.Movies.Remove(movie);
            _dbContext.SaveChanges();

            return Ok("Record Deleted");
        }


        private string UploadFile(IFormFile file)
        {
            if (file == null)
            {
                return String.Empty;
            }

            var guid = Guid.NewGuid();
            string relativeFilePath = guid + ".jpg";
            var filePath = Path.Combine("wwwroot", relativeFilePath);
            var fileStream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(fileStream);

            return relativeFilePath;
        }
    }
}