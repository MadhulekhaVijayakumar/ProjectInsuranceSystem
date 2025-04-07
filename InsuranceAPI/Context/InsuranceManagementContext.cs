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
       
        //public DbSet<Proposal> Proposals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Client ↔ User relationship (One-to-One)
            modelBuilder.Entity<Client>()
                 .HasOne(c => c.User)
                 .WithOne(u => u.Client)
                 .HasForeignKey<Client>(c => c.Email) // Email in Client is FK
                 .HasPrincipalKey<User>(u => u.Username) // Username in User is PK
                 .OnDelete(DeleteBehavior.Cascade);

            // Admin ↔ User relationship (One-to-One)
            modelBuilder.Entity<Admin>()
                .HasOne(a => a.User)
                .WithOne(u => u.Admin)
                .HasForeignKey<Admin>(a => a.Email)
                .HasPrincipalKey<User>(u => u.Username)
                .OnDelete(DeleteBehavior.Cascade);

            // Ensure unique usernames
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // DateOfBirth stored as Date (for Client)
            modelBuilder.Entity<Client>()
                .Property(c => c.DateOfBirth)
                .HasColumnType("date");

           
            modelBuilder.Entity<Client>()
                .Property(c => c.AadhaarNumber)
                .IsRequired();

            modelBuilder.Entity<Client>()
                .Property(c => c.PANNumber)
                .IsRequired();

            modelBuilder.Entity<Client>()
                .Property(c => c.Address)
                .IsRequired();
        }

    }
}
