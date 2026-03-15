using System.ComponentModel.DataAnnotations;

namespace MiniHelpDesk.ViewModels
{
    public class EditTicketViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        [Display(Name = "Durum")]
        public string Status { get; set; } = "Open";

        [Display(Name = "Öncelik")]
        public string Priority { get; set; } = "Medium"; 
    }
}