# Currency2 - FX Transfer Platform (Basic)

A basic ASP.NET Core Web API modeling a UK-based money transfer platform, inspired by Wise, TransferGo, and Revolut.

## Features

- Create multi-currency wallet accounts per user
- Top-up a GBP wallet from a designated UK bank account
- Convert funds between currency wallets using daily exchange rates
- Pay out funds from a wallet to any bank account (UK or international), enforcing matching currency

## Business Rules

1. **Create accounts in different currencies** — one wallet per currency per user.
2. **Wallet-to-wallet transfer** — uses a stored daily exchange rate (not live) when currencies differ.
3. **UK bank -> wallet top-up** — source must be the user's designated UK bank account, and destination wallet must be GBP.
4. **Wallet -> external bank payout** — destination can be any country/bank, but destination currency must match the source wallet currency. If they differ, the user must convert first.

## Project Structure

```
currency2/
├── FxBasic.csproj
├── Program.cs
├── appsettings.json
├── Data/
│   └── AppDbContext.cs
├── Models/
│   ├── User.cs
│   ├── WalletAccount.cs
│   ├── BankAccount.cs
│   ├── ExchangeRate.cs
│   └── Transfer.cs
├── DTOs/
│   ├── CreateWalletRequest.cs
│   ├── TopUpRequest.cs
│   ├── ConvertRequest.cs
│   └── PayoutRequest.cs
└── Controllers/
    ├── WalletsController.cs
    ├── TopUpController.cs
    ├── ConvertController.cs
    └── PayoutController.cs
```

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Run locally

```bash
dotnet restore
dotnet run
```

The API will start (by default) at `https://localhost:5001` or `http://localhost:5000`, with Swagger UI available at `/swagger` in development mode.

On first run, the SQLite database (`fxbasic.db`) is created automatically and seeded with:
- A demo user
- A UK main bank account (GBP) and an EU beneficiary bank account (EUR)
- A GBP wallet (balance 1000) and a EUR wallet (balance 100)
- Daily GBP<->EUR exchange rates

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/wallets` | Create a new currency wallet for a user |
| GET | `/api/wallets/{userId}` | List wallets for a user |
| POST | `/api/topup` | Top-up a GBP wallet from the UK designated bank account |
| POST | `/api/convert` | Convert funds between two wallets using the daily rate |
| POST | `/api/payout` | Pay out funds from a wallet to a bank account (same currency only) |

### Example: Create a wallet
```json
POST /api/wallets
{
  "userId": 1,
  "currency": "USD"
}
```

### Example: Top-up GBP wallet
```json
POST /api/topup
{
  "userId": 1,
  "bankAccountId": 1,
  "destinationWalletId": 1,
  "amount": 200
}
```

### Example: Convert GBP to EUR
```json
POST /api/convert
{
  "userId": 1,
  "fromWalletId": 1,
  "toWalletId": 2,
  "amount": 100
}
```

### Example: Payout to bank account
```json
POST /api/payout
{
  "userId": 1,
  "sourceWalletId": 2,
  "destinationBankAccountId": 2,
  "amount": 50
}
```

## Notes
- Exchange rates are stored per day and are **not** fetched live (as per project scope); insert new `ExchangeRate` rows daily to simulate Bank of England rate updates.
- This is a basic/reference implementation with no authentication layer — add auth before any production use.
