using Microsoft.EntityFrameworkCore;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class DbAppContext : DbContext
    {
        public DbSet<Phone> Phones { get; set; }
        public DbSet<Company> Companies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Phone;Username=postgres;Password=C0d38_50AdM1Nn6");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Phone>().HasOne(p  => p.CompanyEntity)
                .WithMany(p => p.PhoneEntities)
                .HasForeignKey(p => p.CompanyId);


            modelBuilder.Entity<Phone>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.ToTable("phone");

                entity.Property(e => e.Title)
                    .HasColumnName("title");

                entity.Property(e => e.CompanyId)
                    .HasColumnName("companyid");

                entity.Property(e => e.Price)
                    .HasColumnName("price");
            });


            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.ToTable("company");

                entity.Property(e => e.Title)
                    .HasColumnName("title");

                entity.Property(e => e.CEO)
                    .HasColumnName("ceo");

                entity.Property(e => e.Capital)
                    .HasColumnName("capital");
            });
        }
    }
}
