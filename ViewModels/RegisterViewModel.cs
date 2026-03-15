using System.ComponentModel.DataAnnotations;

namespace MiniHelpDesk.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "E-posta (Opsiyonel)")]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}