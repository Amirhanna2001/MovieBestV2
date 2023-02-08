using System.Collections.Generic;

namespace MovieBestAuthorizeBased.ViewModels
{
    public class PermissionsFormViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<CheckBoxViewModel> RoleClaims { get; set; }
    }
}
