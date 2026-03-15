namespace MiniHelpDesk.ViewModels
{
    public class UserManagementViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
    }
}