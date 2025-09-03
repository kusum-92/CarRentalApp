// Data/CarRentalContext.cs
using CarRentalApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CarRentalApp.Data
{
    public class CarRentalContext : IdentityDbContext<IdentityUser>
    {
        public CarRentalContext(DbContextOptions<CarRentalContext> options) : base(options) { }

        public DbSet<Car> Cars => Set<Car>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Rental> Rentals => Set<Rental>();
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Car)
                .WithMany()
                .HasForeignKey(r => r.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Customer)
                .WithMany()
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Rental)
                .WithMany() 
                .HasForeignKey(p => p.RentalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Customer>()
                .HasOne(c=>c.User) // navigationless FK
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .HasPrincipalKey(u => u.Id) 
                .IsRequired()// maps to AspNetUsers.Id
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
