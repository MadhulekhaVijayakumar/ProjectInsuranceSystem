using InsuranceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceAPI.Context
{
    public class InsuranceManagementContext : DbContext
    {
        public InsuranceManagementContext(DbContextOptions options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<InsuranceDetails> InsuranceDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ---------------- USER RELATIONSHIPS ----------------

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

            // ---------------- CLIENT PROPERTIES ----------------

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

            modelBuilder.Entity<Client>()
                .HasIndex(c => c.AadhaarNumber)
                .IsUnique();

            modelBuilder.Entity<Client>()
                .HasIndex(c => c.PANNumber)
                .IsUnique();

            // ---------------- VEHICLE CONFIG ----------------

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Client)
                .WithMany(c => c.Vehicles)
                .HasForeignKey(v => v.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.VehicleNumber)
                .IsUnique();

            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.ChassisNumber)
                .IsUnique();

            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.EngineNumber)
                .IsUnique();

            // ---------------- PROPOSAL CONFIG ----------------

            modelBuilder.Entity<Proposal>()
                .HasOne(p => p.Vehicle)
                .WithMany(v => v.Proposals)
                .HasForeignKey(p => p.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proposal>()
                .HasOne(p => p.Client)
                .WithMany(c => c.Proposals)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proposal>()
                .Property(p => p.FitnessValidUpto)
                .HasColumnType("date");

            modelBuilder.Entity<Proposal>()
                .Property(p => p.InsuranceValidUpto)
                .HasColumnType("date");

            // ---------------- INSURANCE DETAILS ----------------

            modelBuilder.Entity<InsuranceDetails>(entity =>
            {
                entity.HasKey(i => i.Id);

                entity.Property(i => i.InsuranceStartDate)
                      .HasColumnType("date");

                entity.Property(i => i.InsuranceSum)
                      .HasColumnType("decimal(18,2)");

                entity.Property(i => i.DamageInsurance)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(i => i.LiabilityOption)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(i => i.Plan)
                      .HasMaxLength(20)
                      .IsRequired();

                // One-to-One: Proposal → InsuranceDetails
                entity.HasOne(i => i.Proposal)
                      .WithOne(p => p.InsuranceDetails)
                      .HasForeignKey<InsuranceDetails>(i => i.ProposalId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Many-to-One: Vehicle → InsuranceDetails
                entity.HasOne(i => i.Vehicle)
                      .WithMany(v => v.InsuranceDetails)
                      .HasForeignKey(i => i.VehicleId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
