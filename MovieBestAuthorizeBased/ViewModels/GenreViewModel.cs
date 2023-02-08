using System.ComponentModel.DataAnnotations;

namespace MovieBestAuthorizeBased.ViewModels
{
    public class GenreViewModel
    {
        [Required ]
        [Display(Name ="Genre Name")]
        public string Name { get; set; }
    }
}
