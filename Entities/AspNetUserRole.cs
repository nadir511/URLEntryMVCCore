namespace URLEntryMVC.Entities
{
    public class AspNetUserRole
    {
        public string RoleId { get; set; } = null!;
        public virtual AspNetRole Role { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public virtual AspNetUser User { get; set; } = null!;

    }
}
