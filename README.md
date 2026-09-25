# TaskFlow — Full-Stack Task Manager on AWS

A cloud-native task management application built as a portfolio project. Features JWT authentication, per-user data isolation, full CRUD, and a modern React UI — all deployed on AWS serverless infrastructure.

**🔗 Live Demo:** https://taskflow-ui-pi.vercel.app/login
**📦 Repo:** https://github.com/NuzhaIrfan/taskflow.git
**🌐 Live API:** https://rvdbsec5zh.execute-api.us-east-1.amazonaws.com

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────┐
│              React + Tailwind + TypeScript              │
│                    (Vercel — free)                      │
└────────────────────────┬────────────────────────────────┘
                         │ HTTPS + JWT
                         ▼
┌─────────────────────────────────────────────────────────┐
│           AWS API Gateway (HTTP API)                    │
│        ANY /  +  ANY /{proxy+}  → Lambda                │
└────────────────────────┬────────────────────────────────┘
                         │ Lambda proxy event
                         ▼
┌─────────────────────────────────────────────────────────┐
│         AWS Lambda (.NET 8, C#) — single function       │
│  ┌─────────────────────────────────────────────────┐    │
│  │ Router → Handlers → Services → EF Core          │    │
│  │ • AuthHandlers (register, login, JWT)           │    │
│  │ • TaskHandlers (CRUD)                           │    │
│  └─────────────────────────────────────────────────┘    │
└────────────────────────┬────────────────────────────────┘
                         │ EF Core
                         ▼
┌─────────────────────────────────────────────────────────┐
│     Amazon RDS — SQL Server Express (free tier)         │
│           Tables: Users, Tasks                          │
└─────────────────────────────────────────────────────────┘
```

---

## ✨ Features

- **JWT authentication** with BCrypt password hashing
- **Per-user data isolation** — users only see their own tasks
- **Full CRUD** — create, read, update, delete tasks
- **Filter by status** — All / Active / Done
- **Priority levels** — Low / Medium / High with color coding
- **Due dates** with overdue detection
- **Dashboard stats** — total, active, done, overdue
- **Serverless architecture** — no servers to manage
- **Clean architecture** — router, handlers, helpers, DTOs

---

## 🧰 Tech Stack

| Layer | Technology |
|-------|------------|
| Frontend | React + TypeScript + Vite + Tailwind CSS |
| Backend | C# / .NET 8, AWS Lambda |
| API | AWS API Gateway (HTTP API) |
| Data | Entity Framework Core 8 |
| Database | SQL Server on Amazon RDS |
| Auth | JWT (System.IdentityModel.Tokens.Jwt) + BCrypt |
| Hosting | AWS (backend) + Vercel (frontend) |

---

## 📁 Project Structure

```
nuzha-aws-labs/
├── TaskFlow.Data/          # Shared data layer (EF Core, models, DbContext)
│   ├── Models/
│   │   ├── User.cs
│   │   └── TaskItem.cs
│   └── AppDbContext.cs
│
├── TaskFlow.Lambda/        # AWS Lambda function
│   ├── Function.cs         # Thin router
│   ├── Handlers/
│   │   ├── AuthHandlers.cs
│   │   └── TaskHandlers.cs
│   ├── Helpers/
│   │   ├── ApiResponse.cs
│   │   ├── DbContextFactory.cs
│   │   ├── JwtHelper.cs
│   │   └── AppConfig.cs
│   └── Models/
│       └── TaskDtos.cs
│
├── taskflow-ui/            # React frontend
│   ├── src/
│   │   ├── api.ts
│   │   ├── authContext.tsx
│   │   ├── components/
│   │   │   ├── TaskCard.tsx
│   │   │   └── CreateTaskModal.tsx
│   │   └── pages/
│   │       ├── LoginPage.tsx
│   │       ├── RegisterPage.tsx
│   │       └── DashboardPage.tsx
│   └── ...
│
└── README.md
```

---

## 🔌 API Endpoints

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| GET | `/health` | No | Health check |
| POST | `/auth/register` | No | Create user → JWT |
| POST | `/auth/login` | No | Login → JWT |
| GET | `/tasks` | Yes | List current user's tasks |
| GET | `/tasks/{id}` | Yes | Get one task |
| POST | `/tasks` | Yes | Create task |
| PUT | `/tasks/{id}` | Yes | Update task |
| DELETE | `/tasks/{id}` | Yes | Delete task |

**Live base URL:** `https://rvdbsec5zh.execute-api.us-east-1.amazonaws.com`

---

## 🚀 Running Locally

### Prerequisites
- .NET 8 SDK
- Node.js 20+
- SQL Server (local or RDS)
- AWS CLI configured (for deployment)

### Backend

```bash
cd TaskFlow.Lambda
dotnet restore
dotnet build
dotnet-lambda-test-tool-8.0
```

### Frontend

```bash
cd taskflow-ui
npm install
npm run dev
```

Opens at http://localhost:5173

### Deployment (AWS)

```bash
cd TaskFlow.Lambda
dotnet lambda deploy-function TaskFlowBackend
```

---

## 🧠 Engineering Decisions

### Why Lambda instead of EC2?
Serverless means zero cost when idle, auto-scaling, and no OS maintenance. Ideal for low-traffic portfolio applications.

### Why a single Lambda with a router?
For a small API surface, one function is simpler to deploy and manage than eight separate functions. Splitting into multiple functions would be the next step for a production system.

### Why RDS instead of DynamoDB?
Leverages existing SQL Server / EF Core skills and supports relational data (Users → Tasks) naturally with foreign keys and joins.

### Why JWT instead of sessions?
Stateless — no session store needed. Works naturally with Lambda's ephemeral compute model.

### Security tradeoffs (documented for production)
- **Demo:** RDS security group allows `0.0.0.0/0` on port 1433 for simplicity.
- **Production:** Place Lambda inside the RDS VPC, use security-group references, restrict inbound.
- **Demo:** JWT secret is a constant in code.
- **Production:** Store in AWS Secrets Manager, rotate regularly.

---

## 📈 Future Improvements

- Unit + integration tests (xUnit, Testcontainers)
- Refresh tokens
- Rate limiting per user via API Gateway usage plans
- CloudWatch custom metrics for task events
- CI/CD via GitHub Actions
- Move Lambda into VPC for RDS
- Swap to DynamoDB for demo comparison

---

## 👤 Author

**Nuzha Irfan**  
Full-Stack Developer — React · .NET · AWS  


---

## 📝 License

MIT — free to use as a learning reference.