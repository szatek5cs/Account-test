using AccountWebApp.Domain;
using AccountWebApp.Endpoints;
using AccountWebApp.Endpoints.MonthlyFee;
using AccountWebApp.Endpoints.Transfers;
using AccountWebApp.Endpoints.Withdraw;
using AccountWebApp.Infrastructure.Persistence;
using AccountWebApp.Services.Account;
using AccountWebApp.Services.Transfers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransfersService, TransfersService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DbConnection")));

var app = builder.Build();

app.UseExceptionHandler();
app.MapWithdrawEndpoints();
app.MapMonthlyFeeEndpoints();
app.MapTransfersEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.GetPendingMigrations().Any())
    {
        db.Database.Migrate();
    }

    // seed data
    if (db.Accounts.Count() == 0)
    {
        db.Accounts.AddRange(
            new Account(1000),
            new Account(1000),
            new Account(0)
        );
        db.SaveChanges();
    }
}

app.Run();
