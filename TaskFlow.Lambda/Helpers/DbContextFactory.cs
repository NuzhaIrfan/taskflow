using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;

namespace TaskFlow.Lambda.Helpers;

public static class DbContextFactory
{
    // Later this moves to Lambda environment variables
    private const string ConnectionString =
        "Server=127.0.0.1,1433;Database=TaskFlow;User Id=taskflowuser;Password=Password123!;TrustServerCertificate=True";

    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        return new AppDbContext(options);
    }
}