using System.ComponentModel.DataAnnotations;
namespace BlazorServerApp.Models
{
    public class LoginForm
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}