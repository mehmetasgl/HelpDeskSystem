using MiniHelpDesk.Models;

namespace MiniHelpDesk.ViewModels
{
    public class TicketIndexViewModel
    {
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int ClosedTickets { get; set; }
        public List<Category> Categories { get; set; } = new List<Category>();
        public int? SelectedCategoryId { get; set; }
        public string? SearchTerm { get; set; } 
    }
}