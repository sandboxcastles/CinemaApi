using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaApi.DTOModels
{
    public class MovieSummary
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Duration { get; set; }
        public string Language { get; set; }
        public double Rating { get; set; }
        public string Genre { get; set; }
        public string ImageUrl { get; set; }
        public string TrailerUrl { get; set; }
    }
}
