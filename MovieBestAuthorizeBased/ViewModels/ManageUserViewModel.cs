using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieBestAuthorizeBased.ViewModels
{
    public class ManageUserViewModel
    {
        [Required,Display(Name ="User Id")]
        public string UserId  { get; set; }

        [Required, Display(Name = "User Name")]
        public string UserName  { get; set; }

        [Required, Display(Name = "First Name")]
        public string FirstName  { get; set; }

        [Required, Display(Name = "Last Name")]
        public string LastName  { get; set; }
        [Required, Display(Name = "Email"),EmailAddress]
        public string Email  { get; set; }
        public List<CheckBoxViewModel> UserRoles { get; set; }
    }
}
