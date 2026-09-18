namespace TaskFlow.Lambda.Models;

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public bool IsDone { get; set; }
    public byte Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserEmail { get; set; } = "";
}

public class CreateTaskRequest
{
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public byte Priority { get; set; } = 1;
    public DateTime? DueDate { get; set; }
}

public class UpdateTaskRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool? IsDone { get; set; }
    public byte? Priority { get; set; }
    public DateTime? DueDate { get; set; }
}

public class RegisterRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class LoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class AuthResponse
{
    public string Token { get; set; } = "";
    public string Email { get; set; } = "";
    public int UserId { get; set; }
}