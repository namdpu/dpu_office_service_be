using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeService.DAL.Entities;

namespace OfficeService.DAL
{
    public class DataBaseContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DataBaseContext()
        {
        }

        public DataBaseContext(DbContextOptions<DataBaseContext> option, IConfiguration configuration) : base(option)
        {
            _configuration = configuration;
        }

        public DbSet<Entities.File> Files { get; set; }
        public DbSet<FileVersion> FileVersions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration.GetConnectionString("postgres");
            //optionsBuilder.UseNpgsql("User ID =admin;Password=admin;Server=localhost;Port=5433;Database=dpu_webgis2");
            //optionsBuilder.UseNpgsql(connectionString);
            optionsBuilder.UseNpgsql("User ID =admin;Password=admin_password;Server=localhost;Port=5436;Database=dpu_office;Pooling=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileVersion>()
                .Property(s => s.Histotry)
                .HasColumnType("jsonb");
            modelBuilder.Entity<FileVersion>()
                .Property(s => s.Users)
                .HasColumnType("jsonb");
            modelBuilder.Entity<FileVersion>()
                .Property(s => s.Actions)
                .HasColumnType("jsonb");

            base.OnModelCreating(modelBuilder);
        }
    }
}
