using MiniHelpDesk.Models;

namespace MiniHelpDesk.ViewModels
{
    public class UserDetailsViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int ClosedTickets { get; set; }
        public List<Ticket> RecentTickets { get; set; } = new List<Ticket>();
    }
}