using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServerApp.Configurations;
using ServerApp.Enums;
using ServerApp.Models;

namespace ServerApp.Data
{
    public class AppDbContext: DbContext
    {
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentStatus> DocumentStatuses { get; set; }
        public DbSet<Status> Statuses { get; set; }

        private readonly DatabaseConfig _dbConfig;

        public AppDbContext(IOptions<DatabaseConfig> dbConfig)
        {
            _dbConfig = dbConfig.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = new SqlConnectionStringBuilder
            {
                DataSource = _dbConfig.DataSource,
                InitialCatalog = _dbConfig.InitialCatalog,
                IntegratedSecurity = _dbConfig.IntegratedSecurity
                
            };

            optionsBuilder.UseSqlServer(connectionString.ConnectionString);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfiguration(new StatusConfiguration());
        }
    }
}
