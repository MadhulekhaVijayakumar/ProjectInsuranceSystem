using InsuranceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceAPI.Context
{
    public class InsuranceManagementContext: DbContext
    {
        public InsuranceManagementContext(DbContextOptions options): base(options)
        {

        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Proposal> Proposals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                 .HasOne(c => c.User)
                 .WithOne(u => u.Client)
                 .HasForeignKey<Client>(c => c.Email)
                 .HasPrincipalKey<User>(u => u.Username)
                 .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<Admin>()
                .HasOne(a => a.User)
                .WithOne(u => u.Admin)
                .HasForeignKey<Admin>(a => a.Email)
                .HasPrincipalKey<User>(u => u.Username)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            //modelBuilder.Entity<Client>()
            //     .Property(c => c.DateOfBirth)
            //     .HasColumnType("date");

        }

    }
}
