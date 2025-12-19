using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OracleEF.AzFunction.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OracleEF.AzFunction.OracleContext
{
    public class OracleContext : DbContext
    {
        public OracleContext(DbContextOptions<OracleContext> options)
            : base(options)
        {
            options.AddInterceptors(new QueryTimingInterceptor(logger));
        }
        // Define DbSets for your entities here
        // public DbSet<YourEntity> YourEntities { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure your entity mappings here

            optionsBuilder.AddInterceptors(new QueryTimingInterceptor(logger));
        }
    }
}
