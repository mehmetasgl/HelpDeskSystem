using System.ComponentModel.DataAnnotations;

namespace MiniHelpDesk.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Kategori Adı")]
        public string Name { get; set; } = string.Empty;
    }
}