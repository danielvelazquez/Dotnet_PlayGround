using Microsoft.EntityFrameworkCore;
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
        }
        // Define DbSets for your entities here
        // public DbSet<YourEntity> YourEntities { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure your entity mappings here
        }
    }
}
