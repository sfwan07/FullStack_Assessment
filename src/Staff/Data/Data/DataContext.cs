using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Staff.Core;

namespace Staff.Data
{
    public class DataContext:DbContext
    {

        private readonly HttpContext _httpContext;

        public DataContext(DbContextOptions options,IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        DbSet<Employee> Employees { get; set; }


        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            AddTimestamps();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void AddTimestamps()
        {
            if (_httpContext is null) return;

            Guid userId = _httpContext.User.HasClaim(c => c.Type == "SID") ? (Guid.Parse(_httpContext.User.FindFirst("SID").Value)) : Guid.Empty;

            ChangeTracker.Entries().Where(e =>
                e.Entity is BaseEntity &&
                (e.State == EntityState.Added || e.State == EntityState.Modified)
            ).ToList().ForEach(entry =>
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        ((BaseEntity)entry.Entity).SetUpdate(userId);
                        break;
                    case EntityState.Added:
                        ((BaseEntity)entry.Entity).SetCreate(userId);
                        break;
                }
            });
        }
    }
}
