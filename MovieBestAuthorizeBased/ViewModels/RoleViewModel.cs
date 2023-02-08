using System.ComponentModel.DataAnnotations;

namespace MovieBestAuthorizeBased.ViewModels
{
    public class RoleViewModel
    {
        [Required, Display(Name ="Role Name")]
        public string RoleName { get; set; }
    }
}
