using AutoRazorViewModels.Properties;
using Carfamsoft.ModelToView.ViewAnnotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace AutoRazorViewModels
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

    public class AutoUpdateUserModel : UpdateUserModel
    {
        [DisplayIgnore]
        public string Id { get; set; }

        [Range(typeof(DayOfWeek), "Monday", "Friday")]
        [FormDisplay(GroupName = "PleaseSelect", Tag = "select", Name = "", Order = 1, Prompt = nameof(FavouriteWorkingDay), Icon = "fas fa-calendar")]
        public string FavouriteWorkingDay { get; set; }

        [FormDisplay(GroupName = "PleaseSelect", UIHint = "select", Name = "", Order = 2)]
        public int AgeRange { get; set; }

        //[Range(typeof(ConsoleColor), "Black", "White")]
        [FormDisplay(Type = "radio", Order = 3, Options = "Black|Blue|White")]
        public string FavouriteColor { get; set; }

        [FormDisplay(Order = 4, Description = "LoginWithEmailAndSms")]
        public bool TwoFactorEnabled { get; set; }
    }
}