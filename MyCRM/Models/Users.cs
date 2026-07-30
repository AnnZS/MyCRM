using Microsoft.AspNetCore.Identity;

namespace MyCRM.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set;}
    }
}
