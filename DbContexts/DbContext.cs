using Microsoft.EntityFrameworkCore;
using Gameball_Elevate.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Gameball_Elevate.DbContexts
{
    public class GlobalDbContext : IdentityDbContext<User>
    {
        public GlobalDbContext(DbContextOptions<GlobalDbContext> options) : base(options){}
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
