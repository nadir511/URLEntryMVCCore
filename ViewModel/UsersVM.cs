namespace URLEntryMVC.ViewModel
{
    public class UsersVM
    {
        public string Id { get; set; } = null!;

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }
        public int CustomerIdFk { get; set; }
        public string? CustomerName { get; set; }
        public string? UserRole { get; set; }

    }
}
