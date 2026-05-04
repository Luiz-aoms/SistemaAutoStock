using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SistemaAutoStock.Data
{
    public class appDbContext : IdentityDbContext<IdentityUser>
    {
        public appDbContext(DbContextOptions<appDbContext> options) : base(options)
        {

        }
    }
}
