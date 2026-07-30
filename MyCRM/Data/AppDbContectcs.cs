using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyCRM.Models;

namespace MyCRM.Data
{
    public class AppDbContectcs : IdentityDbContext<Users>
    {
        public AppDbContectcs(DbContextOptions options) : base(options)
        {
        }

        protected AppDbContectcs()
        {
        }
    }
}
