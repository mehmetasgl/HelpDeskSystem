using System.ComponentModel.DataAnnotations;

namespace MiniHelpDesk.ViewModels
{
    public class EditProfileViewModel
    {
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "E-posta")]
        public string? Email { get; set; }
    }
}