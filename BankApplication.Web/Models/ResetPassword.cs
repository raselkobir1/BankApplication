using System.ComponentModel.DataAnnotations;

namespace BankApplication.Web.Models
{
    public class ResetPassword
    {
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } 

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }

        public string Token { get; set; } 
    }
}
