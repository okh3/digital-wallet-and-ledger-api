# 🏦 Digital Wallet & Ledger API

An enterprise-grade, highly secure RESTful API for managing digital wallets and processing financial transactions. Built with **.NET 10** and **PostgreSQL**, this project demonstrates modern backend architecture, including the CQRS pattern, JWT authentication, global error handling, and structured logging.

## 🚀 Tech Stack & Architecture
* **Framework:** .NET 10 Web API
* **Database:** PostgreSQL (Containerized via Docker)
* **ORM:** Entity Framework Core (with auto-schema generation)
* **Architecture Pattern:** CQRS (Command Query Responsibility Segregation) via MediatR
* **Security:** JWT (JSON Web Tokens) Authentication & Role Authorization
* **Resilience:** ASP.NET Core Rate Limiting (Fixed Window)
* **Observability:** Serilog (Rolling file & Console logging)
* **Documentation:** Swagger / OpenAPI 3.0

---

## ⚙️ Prerequisites
To run this project locally, ensure you have the following installed:
* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* [Docker Desktop](https://www.docker.com/products/docker-desktop)
* Visual Studio 2022 or VS Code

---

## 🛠️ Zero-Friction Setup (Local Development)

This project is designed for immediate, frictionless testing. The database tables will automatically generate themselves upon the first successful run.

**1. Clone the repository**
git clone https://github.com/okh3/digital-wallet-and-ledger-api.git
cd DigitalWalletAndLedgerAPI

**2. Start the PostgreSQL Database**
Ensure Docker Desktop is running, then spin up the database container:
docker-compose up postgres-db -d

**3. Verify your Connection String**
Ensure your `appsettings.json` is configured to allow the API container to talk to the database container:
"DefaultConnection": "Host=host.docker.internal;Database=WalletLedgerDb;Username=wallet_admin;Password=SuperSecretPassword123!;"

**4. Run the Application**
Launch the application via Visual Studio (Docker profile) or the .NET CLI. The API will automatically connect to Postgres, build the necessary tables, and launch the Swagger UI in your browser.

---

## 🧪 How to Test the API Flow

The application includes Development-Only utility endpoints to make testing seamless for reviewers.

1. **Generate a Token:** * Navigate to the `Test Utilities` section in Swagger and execute `POST /api/auth/login`. 
   * Copy the returned JWT token.
2. **Authenticate:** * Click the green **Authorize 🔓** button at the top of Swagger.
   * Type `Bearer <YOUR_TOKEN>` (including the space) and click Authorize.
3. **Seed your first Wallet:** * Execute `POST /api/wallet/test-seed`. 
   * This generates a random wallet pre-funded with a $1000 balance. Copy the `walletId`.
4. **Seed a second Wallet:**
   * Execute `POST /api/wallet/test-seed` a second time to create the receiving account. Copy this second `walletId`.
5. **Transfer Funds:** * Navigate to `POST /api/wallet/transfer`.
   * Paste the two Wallet IDs into the JSON body along with an `amount` (e.g., `250`). Execute the transfer.
6. **Verify Balances:** * Use `GET /api/wallet/{id}/balance` to confirm the funds were successfully deducted and deposited.

---

## 🛡️ Key Features Demonstrated

### 1. Global Exception Handling
Instead of leaking stack traces or crashing the server, exceptions (like invalid JSON or database transient failures) are caught by a custom Middleware pipeline and returned as professional `400 Bad Request` or `500 Internal Server Error` JSON responses.

### 2. CQRS Pattern with MediatR
Business logic is strictly decoupled from the Controllers/Minimal APIs. Read operations (`GetWalletBalanceQuery`) are separated from write operations (`TransferFundsCommand`), ensuring the codebase remains scalable and testable.

### 3. Enterprise Logging
Configured with **Serilog** to bypass the ephemeral default console logger. All API interactions, including successful transfers and caught exceptions, are written to a daily rolling text file in the `/Logs` directory for permanent auditing.

### 4. Rate Limiting
Critical endpoints are protected by a `StrictPolicy` fixed-window rate limiter, preventing brute-force attacks and transaction spam.
