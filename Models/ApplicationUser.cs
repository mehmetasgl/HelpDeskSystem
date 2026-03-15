using Microsoft.AspNetCore.Identity;

namespace MiniHelpDesk.Models
{
    public class ApplicationUser : IdentityUser
    {

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}