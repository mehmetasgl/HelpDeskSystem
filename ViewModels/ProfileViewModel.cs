using System.ComponentModel.DataAnnotations;

namespace MiniHelpDesk.ViewModels
{
    public class ProfileViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "E-posta")]
        public string? Email { get; set; }

        [Display(Name = "Üyelik Tarihi")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Toplam Ticket Sayısı")]
        public int TotalTickets { get; set; }

        [Display(Name = "Açık Ticket Sayısı")]
        public int OpenTickets { get; set; }

        [Display(Name = "Kapalı Ticket Sayısı")]
        public int ClosedTickets { get; set; }
    }
}