using AccountWebApp.Domain;
using AccountWebApp.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AcountWevAppTests;

public class ConcurencyTests
{
    [Fact]
    public async Task Should_throw_concurrency_exception()
    {
        // shared connection!
        var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        // create schema
        using (var setupContext = new AppDbContext(options))
        {
            await setupContext.Database.EnsureCreatedAsync();

            setupContext.Accounts.Add(new Account(1000));

            await setupContext.SaveChangesAsync();
        }

        int accountId;

        // get account id
        using (var context = new AppDbContext(options))
        {
            accountId = await context.Accounts
                .Select(x => x.Id)
                .SingleAsync();
        }

        // two separate DbContexts
        using var context1 = new AppDbContext(options);
        using var context2 = new AppDbContext(options);

        var account1 = await context1.Accounts
            .SingleAsync(x => x.Id == accountId);

        var account2 = await context2.Accounts
            .SingleAsync(x => x.Id == accountId);

        // first update
        account1.Withdraw(100);

        await context1.SaveChangesAsync();

        // second update based on stale row version
        account2.Withdraw(50);

        // should fail
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() =>
            context2.SaveChangesAsync());
    }
}
