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

            entity.Property<Guid>("RowVersion")
                .IsConcurrencyToken()
                .HasValueGenerator<Microsoft.EntityFrameworkCore.ValueGeneration.GuidValueGenerator>();
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

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
    // 1. Znajdź wszystkie obiekty w pamięci EF Core, które zostały zmodyfikowane lub dodane
        var modifiedEntries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in modifiedEntries)
        {
            // 2. Sprawdź, czy dana encja posiada ukrytą właściwość (Shadow Property) o nazwie "RowVersion"
            var rowVersionProperty = entry.Metadata.FindProperty("RowVersion");
            if (rowVersionProperty != null)
            {
                // 3. Wymuś przypisanie nowego, świeżego GUID-a bezpośrednio przed wysłaniem SQL-a
                entry.Property("RowVersion").CurrentValue = Guid.NewGuid();
            }
        }

        // 4. Uruchom standardowy zapis do bazy danych
        return base.SaveChangesAsync(cancellationToken);
    }
}
