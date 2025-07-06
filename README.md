# 😂 Dad Joke Explorer – Fullstack Application (React + .NET 8 + SQL + Elasticsearch)

This is a full-stack, production-grade Dad Joke Explorer app that allows users to:
- Fetch a random dad joke
- Search jokes with keyword highlighting
- Group jokes by length (Short, Medium, Long)
- Cache results using in-memory caching
- Persist jokes in SQL Server
- Index/search using **Elasticsearch** for scalability
- Full backend developed with **.NET 8**, **Dapper**, **Stored Procedures**, and **Clean Architecture**

---

## 🔧 Tech Stack

### 🖥 Backend (.NET 8)
- ASP.NET Core Web API
- Dapper (Micro ORM)
- Clean Architecture (Domain, Infrastructure, Services, Controllers)
- SQL Server + Stored Procedures
- In-Memory Caching
- Elasticsearch Integration (Fallback strategy)
- FluentValidation + Middleware-based Exception Handling
- Logging with Serilog

### 🌐 Frontend (React + TypeScript)
- Ant Design components
- Axios for API calls
- Real-time highlighting of search terms
- Loading indicators + clean responsive UI

---

## 🚀 Setup Instructions

### 🧱 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Node.js (LTS)](https://nodejs.org/en/download)
- [SQL Server](https://www.microsoft.com/en-in/sql-server/sql-server-downloads)
- [Elasticsearch](https://www.elastic.co/downloads/elasticsearch) (8.x)

---

## 🔌 Backend Setup

```bash
cd backend
dotnet restore
dotnet build
dotnet run
```

✅ Make sure to configure `appsettings.local.json` with your local SQL & Elasticsearch settings.

---

## 🌍 Frontend Setup

```bash
cd frontend
npm install
npm start
```

The frontend will launch on `http://localhost:3000` and will connect to the backend at `https://localhost:7245`.

---

## 🧪 API Endpoints

- `GET /api/joke/random` – fetch a random joke
- `GET /api/joke/search?term=cat` – fetch jokes matching a term

---

## 🧠 Features to Highlight (Interview Focus)

- **Clean Architecture** with interface segregation and dependency injection
- **Fallback Strategy** (SQL ➝ Elasticsearch ➝ API)
- **Fuzzy Search** support with Elasticsearch
- **Stored Procedures** with Dapper (secured & indexed)
- **Global Exception Middleware** with consistent JSON error response
- **Flexible Configuration** via `IOptions` + local overrides
- **Readable UI** with highlighted search terms and grouping

---

## 📁 Project Structure

```
root/
├── backend/
│   ├── Controllers/
│   ├── Domain/
│   ├── Infrastructure/
│   ├── Services/
│   └── Program.cs, appsettings.json, etc.
├── frontend/
│   └── src/App.tsx
└── README.md
```

---

## 📌 Tips for Deployment

- Enable HTTPS
- Add Redis for distributed caching
- Deploy backend to Azure App Service or AWS ECS
- Deploy frontend to Vercel/Netlify
- Use Azure SQL or AWS RDS for DB

---

## 🙌 Credits

**Author:** Ashraf Borugula  
**Email:** ashraf.borugula.ab@gamil.com  
**GitHub:** https://github.com/ashrafashu413/DadJokeApplication
