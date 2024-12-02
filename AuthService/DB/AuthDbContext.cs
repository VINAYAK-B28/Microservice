using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AuthService.DB
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }

        public DbSet<Buyer> Buyer { get; set; }
        public DbSet<Seller> Seller { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Buyer>().ToTable("Buyer");  // Make sure table name matches
            modelBuilder.Entity<Admin>().ToTable("Admin");
            modelBuilder.Entity<Seller>().ToTable("Seller");
            modelBuilder.Entity<UserRole>().HasNoKey();
        }


    }
}