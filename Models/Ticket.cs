namespace MiniHelpDesk.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        public int CategoryId { get; set; }
        
        public string Status { get; set; } = "Open";
        
        public string Priority { get; set; } = "Medium";
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        
        public string? AdminAnswer { get; set; }
        public DateTime? AnsweredAt { get; set; }
        
        public string UserId { get; set; } = string.Empty;
        
        public Category Category { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}