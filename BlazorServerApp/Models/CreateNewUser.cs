using System.ComponentModel.DataAnnotations;

namespace BlazorServerApp.Models
{
    public class CreateNewUser
    {
        [Required]
        public string UserName { get; set; }

        [Required, MinLength(4, ErrorMessage = "Please use a longer password!")]
        public string Password { get; set; }

        [Required]
        public string RepeatPassword { get; set; }

        [Required]
        public string Role { get; set; }
    }
}