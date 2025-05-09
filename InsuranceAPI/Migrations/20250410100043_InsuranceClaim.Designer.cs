﻿// <auto-generated />
using System;
using InsuranceAPI.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace InsuranceAPI.Migrations
{
    [DbContext(typeof(InsuranceManagementContext))]
    [Migration("20250410100043_InsuranceClaim")]
    partial class InsuranceClaim
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AadhaarNumber")
                        .IsRequired()
                        .HasMaxLength(12)
                        .HasColumnType("nvarchar(12)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("date");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PANNumber")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.HasKey("Id");

                    b.HasIndex("AadhaarNumber")
                        .IsUnique();

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("PANNumber")
                        .IsUnique();

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("InsuranceAPI.Models.Admin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Admins");
                });

            modelBuilder.Entity("InsuranceAPI.Models.Document", b =>
                {
                    b.Property<int>("DocumentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DocumentId"));

                    b.Property<int?>("ClaimId")
                        .HasColumnType("int");

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProposalId")
                        .HasColumnType("int");

                    b.HasKey("DocumentId");

                    b.HasIndex("ClaimId");

                    b.HasIndex("ProposalId");

                    b.ToTable("Documents");
                });

            modelBuilder.Entity("InsuranceAPI.Models.Insurance", b =>
                {
                    b.Property<string>("InsurancePolicyNumber")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("InsuranceStartDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("InsuranceSum")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("PremiumAmount")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("ProposalId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VehicleId")
                        .HasColumnType("int");

                    b.HasKey("InsurancePolicyNumber");

                    b.HasIndex("ClientId");

                    b.HasIndex("ProposalId")
                        .IsUnique();

                    b.HasIndex("VehicleId");

                    b.ToTable("Insurances");
                });

            modelBuilder.Entity("InsuranceAPI.Models.InsuranceClaim", b =>
                {
                    b.Property<int>("ClaimId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ClaimId"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<DateTime>("IncidentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("InsurancePolicyNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValue("Pending");

                    b.HasKey("ClaimId");

                    b.HasIndex("InsurancePolicyNumber");

                    b.ToTable("InsuranceClaims");
                });

            modelBuilder.Entity("InsuranceAPI.Models.InsuranceDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("CalculatedPremium")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("DamageInsurance")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("InsuranceStartDate")
                        .HasColumnType("date");

                    b.Property<decimal>("InsuranceSum")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("LiabilityOption")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Plan")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("ProposalId")
                        .HasColumnType("int");

                    b.Property<int>("VehicleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProposalId")
                        .IsUnique();

                    b.HasIndex("VehicleId");

                    b.ToTable("InsuranceDetails");
                });

            modelBuilder.Entity("InsuranceAPI.Models.Payment", b =>
                {
                    b.Property<int>("PaymentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PaymentId"));

                    b.Property<decimal>("AmountPaid")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PaymentMode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProposalId")
                        .HasColumnType("int");

                    b.Property<string>("TransactionStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PaymentId");

                    b.HasIndex("ProposalId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("InsuranceAPI.Models.Proposal", b =>
                {
                    b.Property<int>("ProposalId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProposalId"));

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FitnessValidUpto")
                        .HasColumnType("date");

                    b.Property<string>("InsuranceType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("InsuranceValidUpto")
                        .HasColumnType("date");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("VehicleId")
                        .HasColumnType("int");

                    b.HasKey("ProposalId");

                    b.HasIndex("ClientId");

                    b.HasIndex("VehicleId");

                    b.ToTable("Proposals");
                });

            modelBuilder.Entity("InsuranceAPI.Models.User", b =>
                {
                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(450)");

                    b.Property<byte[]>("HashKey")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("Password")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Username");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("InsuranceAPI.Models.Vehicle", b =>
                {
                    b.Property<int>("VehicleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("VehicleId"));

                    b.Property<string>("ChassisNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<string>("EngineNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("FuelType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("MakerName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ModelName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("date");

                    b.Property<int>("SeatCapacity")
                        .HasColumnType("int");

                    b.Property<string>("VehicleColor")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("VehicleNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("VehicleType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("VehicleId");

                    b.HasIndex("ChassisNumber")
                        .IsUnique();

                    b.HasIndex("ClientId");

                    b.HasIndex("EngineNumber")
                        .IsUnique();

                    b.HasIndex("VehicleNumber")
                        .IsUnique();

                    b.ToTable("Vehicles");
                });

            modelBuilder.Entity("Client", b =>
                {
                    b.HasOne("InsuranceAPI.Models.User", "User")
                        .WithOne("Client")
                        .HasForeignKey("Client", "Email")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("InsuranceAPI.Models.Admin", b =>
                {
                    b.HasOne("InsuranceAPI.Models.User", "User")
                        .WithOne("Admin")
                        .HasForeignKey("InsuranceAPI.Models.Admin", "Email")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("InsuranceAPI.Models.Document", b =>
                {
                    b.HasOne("InsuranceAPI.Models.InsuranceClaim", "Claim")
                        .WithMany("Documents")
                        .HasForeignKey("ClaimId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("InsuranceAPI.Models.Proposal", "Proposal")
                        .WithMany("Documents")
                        .HasForeignKey("ProposalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Claim");

                    b.Navigation("Proposal");
                });

            modelBuilder.Entity("InsuranceAPI.Models.Insurance", b =>
                {
                    b.HasOne("Client", "Client")
                        .WithMany("Insurances")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InsuranceAPI.Models.Proposal", "Proposal")
                        .WithOne("Insurance")
                        .HasForeignKey("InsuranceAPI.Models.Insurance", "ProposalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InsuranceAPI.Models.Vehicle", "Vehicle")
                        .WithMany("Insurances")
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("Proposal");

                    b.Navigation("Vehicle");
                });

            modelBuilder.Entity("InsuranceAPI.Models.InsuranceClaim", b =>
                {
                    b.HasOne("InsuranceAPI.Models.Insurance", "Insurance")
                        .WithMany("Claims")
                        .HasForeignKey("InsurancePolicyNumber")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Insurance");
                });

            modelBuilder.Entity("InsuranceAPI.Models.InsuranceDetails", b =>
                {
                    b.HasOne("InsuranceAPI.Models.Proposal", "Proposal")
                        .WithOne("InsuranceDetails")
                        .HasForeignKey("InsuranceAPI.Models.InsuranceDetails", "ProposalId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("InsuranceAPI.Models.Vehicle", "Vehicle")
                        .WithMany("InsuranceDetails")
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Proposal");

                    b.Navigation("Vehicle");
                });

            modelBuilder.Entity("InsuranceAPI.Models.Payment", b =>
                {
                    b.HasOne("InsuranceAPI.Models.Proposal", "Proposal")
                        .WithMany("Payments")
                        .HasForeignKey("ProposalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Proposal");
                });

            modelBuilder.Entity("InsuranceAPI.Models.Proposal", b =>
                {
                    b.HasOne("Client", "Client")
                        .WithMany("Proposals")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("InsuranceAPI.Models.Vehicle", "Vehicle")
                        .WithMany("Proposals")
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("Vehicle");
                });

            modelBuilder.Entity("InsuranceAPI.Models.Vehicle", b =>
                {
                    b.HasOne("Client", "Client")
                        .WithMany("Vehicles")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("Client", b =>
                {
                    b.Navigation("Insurances");

                    b.Navigation("Proposals");

                    b.Navigation("Vehicles");
                });

            modelBuilder.Entity("InsuranceAPI.Models.Insurance", b =>
                {
                    b.Navigation("Claims");
                });

            modelBuilder.Entity("InsuranceAPI.Models.InsuranceClaim", b =>
                {
                    b.Navigation("Documents");
                });

            modelBuilder.Entity("InsuranceAPI.Models.Proposal", b =>
                {
                    b.Navigation("Documents");

                    b.Navigation("Insurance");

                    b.Navigation("InsuranceDetails");

                    b.Navigation("Payments");
                });

            modelBuilder.Entity("InsuranceAPI.Models.User", b =>
                {
                    b.Navigation("Admin");

                    b.Navigation("Client");
                });

            modelBuilder.Entity("InsuranceAPI.Models.Vehicle", b =>
                {
                    b.Navigation("InsuranceDetails");

                    b.Navigation("Insurances");

                    b.Navigation("Proposals");
                });
#pragma warning restore 612, 618
        }
    }
}
