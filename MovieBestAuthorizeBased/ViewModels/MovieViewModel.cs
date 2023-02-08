using MovieBestAuthorizeBased.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieBestAuthorizeBased.ViewModels
{
    public class MovieViewModel
    {
        public int Id { get; set; }
        [Required, StringLength(250)]
        public string Title { get; set; }

        public int Year { get; set; }
        [Range(1, 10)]
        public double Rate { get; set; }

        [Required, StringLength(2500), Display(Name = "Story Description")]
        public string Storeline { get; set; }
        [Display(Name = "Choose Poster...")]
        public byte[] Poster { get; set; }
        [Display(Name = "Genre")]
        public byte GenreId { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
        public bool IsConfirmed { get; set; }
        [Display(Name ="User Id")]
        public string UserId { get; set; }
    }
}
