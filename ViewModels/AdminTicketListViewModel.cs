using MiniHelpDesk.Models;

namespace MiniHelpDesk.ViewModels
{
    public class AdminTicketListViewModel
    {
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public int? SelectedCategoryId { get; set; }
        public string? SelectedAnswerStatus { get; set; }
        public string? SearchTerm { get; set; }
        public string? PriorityFilter { get; set; } 
    }
}