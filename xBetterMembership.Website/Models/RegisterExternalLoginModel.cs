namespace BetterMembership.Website.Models
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterExternalLoginModel
    {
        public string ExternalLoginData { get; set; }

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }
}