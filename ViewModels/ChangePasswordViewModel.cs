using System.ComponentModel.DataAnnotations;

namespace MiniHelpDesk.ViewModels
{
    public class ChangePasswordViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Mevcut Şifre")]
        public string CurrentPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre Tekrar")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}