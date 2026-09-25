using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;

namespace TaskFlow.Lambda.Helpers;

public static class DbContextFactory
{
    // RDS SQL Server in us-east-1 (production database)
    private const string ConnectionString =
        "Server=taskflow-db.ck9aois6ed9u.us-east-1.rds.amazonaws.com,1433;Database=TaskFlow;User Id=admin;Password=TaskFlow2026!;TrustServerCertificate=True;Encrypt=False";

    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        return new AppDbContext(options);
    }
}