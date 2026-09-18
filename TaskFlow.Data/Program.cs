using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Data.Models;

var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlServer("Server=localhost;Database=TaskFlow;Trusted_Connection=True;TrustServerCertificate=True")
    .Options;

using var db = new AppDbContext(options);

// 1. Insert a user
var user = new User { Email = "nuzha@test.com", PasswordHash = "fake-hash" };
db.Users.Add(user);
await db.SaveChangesAsync();

// 2. Insert a task for that user
db.Tasks.Add(new TaskItem
{
    UserId = user.Id,
    Title = "Deploy to AWS Lambda",
    Priority = 2,
    DueDate = DateTime.UtcNow.AddDays(7)
});
await db.SaveChangesAsync();

// 3. Read it back
var tasks = await db.Tasks.Include(t => t.User).ToListAsync();
foreach (var t in tasks)
{
    Console.WriteLine($"[P{t.Priority}] {t.Title} — {t.User.Email} — Done: {t.IsDone}");
}
