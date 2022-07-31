using Microsoft.EntityFrameworkCore;
using SingalR.API.Model;

namespace SingalR.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<ConnectedUser> ConnectedUsers { get; set; }
    }
}
