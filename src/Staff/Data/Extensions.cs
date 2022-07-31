using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Staff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Staff.Data
{
    public static class Extensions
    {
        public static void AddDBSqlServer(this IServiceCollection service,string connectionString, string assemblyName)
        {
            service.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(connectionString, builder =>
                {
                    builder.MigrationsAssembly(assemblyName);
                });
            });

            service.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        }
    }
}
