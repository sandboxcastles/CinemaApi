using CinemaApi.Data;
using CinemaApi.DTOModels;
using CinemaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private CinemaDbContext _dbContext;
        public ReservationsController(CinemaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetReservations()
        {
            var reservations = from reservation in _dbContext.Reservations
            join user in _dbContext.Users on reservation.UserId equals user.Id
            join movie in _dbContext.Movies on reservation.MovieId equals movie.Id
            select new
            {
                Id = reservation.Id,
                ReservarionTime = reservation.ReservationTime,
                CustomerName = user.Name,
                MovieName = movie.Name
            };

            return Ok(reservations);
        }

        [Authorize]
        [HttpGet("[action]/{id}")]
        public IActionResult GetReservationsByUserId(int id)
        {
            var reservationSummaries = from reservation in _dbContext.Reservations
                               join user in _dbContext.Users on reservation.UserId equals user.Id
                               join movie in _dbContext.Movies on reservation.MovieId equals movie.Id
                               where user.Id == id
                               select new ReservationSummary
                               {
                                   Id = reservation.Id,
                                   ReservationTime = reservation.ReservationTime,
                                   CustomerName = user.Name,
                                   MovieName = movie.Name,
                                   Email = user.Email,
                                   Qty = reservation.Qty,
                                   Price = reservation.Price,
                                   Phone = reservation.Phone,
                                   PlayingDate = movie.PlayingDate,
                                   PlayingTime = movie.PlayingTime
                               };

            return Ok(reservationSummaries);
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var reservationSummary = (from reservation in _dbContext.Reservations
                               join user in _dbContext.Users on reservation.UserId equals user.Id
                               join movie in _dbContext.Movies on reservation.MovieId equals movie.Id
                               where reservation.Id == id
                               select new ReservationSummary
                               {
                                   Id = reservation.Id,
                                   ReservationTime = reservation.ReservationTime,
                                   CustomerName = user.Name,
                                   MovieName = movie.Name,
                                   Email = user.Email,
                                   Qty = reservation.Qty,
                                   Price = reservation.Price,
                                   Phone = reservation.Phone,
                                   PlayingDate = movie.PlayingDate,
                                   PlayingTime = movie.PlayingTime
                               }).FirstOrDefault();

            return Ok(reservationSummary);
        }

        [Authorize]
        [HttpGet("[action]/{id}")]
        public IActionResult GetReservationsByMovieId(int id)
        {
            var reservationSummaries = from reservation in _dbContext.Reservations
                               join user in _dbContext.Users on reservation.UserId equals user.Id
                               join movie in _dbContext.Movies on reservation.MovieId equals movie.Id
                               where movie.Id == id
                               select new ReservationSummary
                               {
                                   Id = reservation.Id,
                                   ReservationTime = reservation.ReservationTime,
                                   CustomerName = user.Name,
                                   MovieName = movie.Name,
                                   Email = user.Email,
                                   Qty = reservation.Qty,
                                   Price = reservation.Price,
                                   Phone = reservation.Phone,
                                   PlayingDate = movie.PlayingDate,
                                   PlayingTime = movie.PlayingTime
                               };

            return Ok(reservationSummaries);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody]Reservation reservation)
        {
            reservation.ReservationTime = DateTime.UtcNow;
            var newReservation = _dbContext.Reservations.Add(reservation);
            if (_dbContext.SaveChanges() == 0)
            {
                return BadRequest("Reservation not added");
            }

            return StatusCode(StatusCodes.Status201Created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}")]
        public IActionResult Delete(int id)
        {
            Reservation reservation = _dbContext.Reservations.Find(id);
            if (reservation == null)
            {
                return NotFound("No Record found with this Id");
            }
            _dbContext.Reservations.Remove(reservation);
            _dbContext.SaveChanges();

            return Ok("Record Deleted");
        }
    }
}
