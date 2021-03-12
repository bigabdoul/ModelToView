using Carfamsoft.ModelToView.Testing.Resources;
using Carfamsoft.ModelToView.ViewAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Carfamsoft.ModelToView.Testing
{
    [FormDisplayDefault(ShowGroupName = true, ResourceType = typeof(DisplayStrings))]
    public class UpdateUserModel
    {
        [Required]
        [StringLength(100)]
        [FormDisplay(GroupName = "PersonalInfo", Icon = "fas fa-user")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        [FormDisplay(GroupName = "PersonalInfo", Icon = "fas fa-user")]
        public string LastName { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress]
        [FormDisplay(GroupName = "ContactDetails", Icon = "fas fa-envelope", UITypeHint = "email")]
        public string Email { get; set; }

        [StringLength(30)]
        [FormDisplay(GroupName = "ContactDetails", Icon = "fas fa-phone", UITypeHint = "phone")]
        public string PhoneNumber { get; set; }
    }
}
