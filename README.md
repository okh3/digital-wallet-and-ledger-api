# 🏦 Digital Wallet & Ledger API

An enterprise-grade, highly secure RESTful API for managing digital wallets and processing financial transactions. Built with **.NET 10**, **PostgreSQL**, and **Redis**, this project demonstrates modern cloud-native backend architecture, focusing on financial data integrity, distributed systems, and resiliency.

## 🚀 Tech Stack & Architecture
* **Framework:** .NET 10 Minimal APIs
* **Database:** PostgreSQL (Containerized via Docker)
* **Distributed Cache:** Redis (Containerized via Docker)
* **ORM:** Entity Framework Core (with automated Migrations & Resiliency)
* **Architecture Pattern:** CQRS (Command Query Responsibility Segregation) via MediatR
* **Security:** JWT (JSON Web Tokens) Authentication & Role Authorization
* **Resilience:** Idempotency Filters & Rate Limiting (Fixed Window)
* **Observability:** Serilog (Structured rolling file & Console logging)
* **Documentation:** Swagger / OpenAPI 3.0

---

## ⚙️ Prerequisites
To run this project locally, ensure you have the following installed:
* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* [Docker Desktop](https://www.docker.com/products/docker-desktop)
* Visual Studio 2022 or VS Code

---

## 🛠️ Zero-Friction Setup (Local Development)

This project is designed for immediate, frictionless testing. 

**1. Clone the repository**
git clone https://github.com/okh3/digital-wallet-and-ledger-api.git
cd DigitalWalletAndLedgerAPI

**2. Start the Infrastructure (Database & Cache)**
Ensure Docker Desktop is running, then spin up both the PostgreSQL and Redis containers:
docker-compose up -d

**3. Verify your Connection Strings**
Ensure your `appsettings.json` is configured to allow the API to talk to your Docker containers:
"DefaultConnection": "Host=localhost;Database=WalletLedgerDb;Username=postgres;Password=your_password;",
"RedisConnection": "localhost:6379"
*(Note: Use `host.docker.internal` instead of `localhost` if running the API inside a container).*

**4. Run the Application**
Launch the application via Visual Studio or the `.NET CLI`. The API will automatically connect to Postgres, safely apply EF Core Migrations, and launch the Swagger UI in your browser.

---

## 🧪 How to Test the API Flow

The application includes Development-Only utility endpoints to make testing seamless.

1. **Generate a Token:** * Navigate to the `Test Utilities` section in Swagger and execute `POST /api/auth/login`. 
   * Copy the returned JWT token.
2. **Authenticate:** * Click the green **Authorize 🔓** button at the top of Swagger.
   * Type `Bearer <YOUR_TOKEN>` (including the space) and click Authorize.
3. **Seed your Wallets:** * Execute `POST /api/wallet/test-seed` twice to create two random wallets pre-funded with $1000 each. Copy their `walletId`s.
4. **Transfer Funds:** * Navigate to `POST /api/wallet/transfer`.
   * Paste the two Wallet IDs into the JSON body along with an `amount` (e.g., `250`). Execute the transfer.
5. **Test Idempotency (Cloud Simulation):**
   * In the same transfer request, add a custom header: `X-Idempotency-Key` with a random GUID. 
   * Fire the request twice. The first will process the transaction. The second will be intercepted by the **Redis Idempotency Filter**, returning the cached `200 OK` response instantly without hitting the database or double-charging the account.
6. **Verify Balances:** * Use `GET /api/wallet/{id}/balance` to confirm the funds were successfully deducted and deposited exactly once.

---

## 🛡️ Senior Engineering Features Demonstrated

### 1. Distributed Idempotency (Redis)
Financial systems must survive client network drops and accidental retries. Money-movement endpoints are protected by an `IEndpointFilter` backed by **Redis**. Duplicate requests with the same Idempotency Key are short-circuited, returning the cached response to prevent catastrophic double-spending.

### 2. Optimistic Concurrency Control
To prevent race conditions where two simultaneous requests attempt to withdraw funds at the exact same millisecond, the `Wallet` domain model utilizes EF Core Concurrency Tokens. Concurrent updates throw a `DbUpdateConcurrencyException`, safely blocking the transaction.

### 3. Database Connection Resiliency
Cloud environments suffer from transient network blips. The Entity Framework Core `Npgsql` configuration implements `EnableRetryOnFailure()`, ensuring the API automatically retries dropped database connections before failing the user's request.

### 4. CQRS Pattern with MediatR
Business logic is strictly decoupled from the Minimal API endpoints. Read operations (`GetWalletBalanceQuery`) are separated from write operations (`TransferFundsCommand`), ensuring the codebase remains highly scalable, readable, and testable.

### 5. Global Exception Handling & Observability
Instead of leaking stack traces or crashing the server, domain exceptions (`WalletNotFoundException`, `InsufficientFundsException`) and system crashes are intercepted by a custom Middleware pipeline. The user receives a safe, sanitized `400` or `500` JSON response, while **Serilog** records the full structured stack trace to the console and a daily rolling log file for auditing.
