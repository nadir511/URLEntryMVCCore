using Microsoft.AspNetCore.Identity;

namespace URLEntryMVC.Extensions
{
    public class ApplicationUserExtension:IdentityUser
    {
        public int? CustomerIdFk { get; set; }
        
    }
}
