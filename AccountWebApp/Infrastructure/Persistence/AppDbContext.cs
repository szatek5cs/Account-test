using System;
using AccountWebApp.Domain;
using Microsoft.EntityFrameworkCore;

namespace AccountWebApp.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.Balance).HasPrecision(18, 2);

            entity.Property(p => p.RowVersion)
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Description).HasMaxLength(500);


            entity.HasOne<Account>()
                .WithMany()
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(t => t.Type)
            .HasConversion<string>();

            entity.Property(e => e.Amount).HasPrecision(18, 2);
        });

        base.OnModelCreating(modelBuilder);
    }
}
