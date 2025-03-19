using FitHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitHub.Domain.DataBase
{
    public class FitHubDbContext: DbContext
    {
        public FitHubDbContext(DbContextOptions<FitHubDbContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<Nutritionist> Nutritionists { get; set; }
        public virtual DbSet<Trainer> Trainers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Email)
                    .IsRequired();
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Role)
                    .IsRequired();
                entity.HasOne(u => u.Nutritionist)
                    .WithOne(n => n.User)
                    .HasForeignKey<Nutritionist>(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(u => u.Trainer)
                    .WithOne(t => t.User)
                    .HasForeignKey<Trainer>(t => t.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(u => u.Member)
                    .WithOne(m => m.User)
                    .HasForeignKey<Member>(m => m.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Trainer Configuration
            modelBuilder.Entity<Trainer>(entity =>
            {
                entity.HasMany(t => t.Members)
                    .WithOne(m => m.Trainer)
                    .HasForeignKey(m => m.TrainerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Member Configuration
            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasOne(m => m.Nutritionist)
                    .WithMany(n => n.Members)
                    .HasForeignKey(m => m.NutritionistId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Trainer)
                    .WithMany(t => t.Members)
                    .HasForeignKey(m => m.TrainerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Nutritionist Configuration
            modelBuilder.Entity<Nutritionist>(entity =>
            {
                entity.HasMany(n => n.Members)
                    .WithOne(m => m.Nutritionist)
                    .HasForeignKey(m => m.NutritionistId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
