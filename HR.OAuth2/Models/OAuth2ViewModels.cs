using System.ComponentModel.DataAnnotations;

namespace HR.OAuth2.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }
    }

    public class VerityViewModel
    {
        public string Token { get; set; }
    }

    public class LogoutViewModel
    {
        public string ReturnUrl { get; set; }
    }
}