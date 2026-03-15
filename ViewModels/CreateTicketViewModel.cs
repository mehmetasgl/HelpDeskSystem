using System.ComponentModel.DataAnnotations;

namespace MiniHelpDesk.ViewModels
{
    public class CreateTicketViewModel
    {
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        [Display(Name = "Öncelik")]
        public string Priority { get; set; } = "Medium"; 
    }
}