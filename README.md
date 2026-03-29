# Hotel Booking System — .NET 10 MVC
### Grand Palace Hotel Management System

---

## ✅ Features Implemented

| Feature | Details |
|---|---|
| **Admin Login** | ASP.NET Core Identity with JWT cookie auth |
| **Role-Based Access** | Admin / Manager / Receptionist roles |
| **User Management** | Admin creates users, assigns roles, enable/disable accounts |
| **Dashboard** | Occupancy stats, check-ins/outs, revenue, arrivals/departures |
| **Room Availability** | Search available rooms by date range and room type |
| **Guest Management** | Add / Edit / View guest profiles with booking history |
| **Booking System** | New booking, check-in, check-out, cancel, edit |
| **Room Types** | Standard / Deluxe / Suite with amenities |
| **Auto Fare Calculation** | Nights × rate + 12% tax with live preview |
| **Invoice Generation** | Auto-generated on checkout; printable view |
| **SQL Server** | Entity Framework Core with code-first migrations |
| **Vertical Sidebar UI** | Collapsible dark sidebar navigation |

---

## 🚀 Quick Start

### 1. Prerequisites
- .NET 10 SDK — https://dotnet.microsoft.com/download
- SQL Server (LocalDB, Express, or full)
- Visual Studio 2022 v17.12+ or VS Code with C# extension

### 2. Configure Connection String
Edit **`appsettings.json`**:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HotelBookingDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```
For SQL Server Express:
```
Server=.\\SQLEXPRESS;Database=HotelBookingDB;Trusted_Connection=True;TrustServerCertificate=True;
```

### 3. Apply Migrations & Run
```bash
cd HotelBookingSystem

# Restore packages
dotnet restore

# Create & apply migration (first time only)
dotnet ef migrations add InitialCreate
dotnet ef database update

# Run
dotnet run
```

### 4. Default Login Credentials
| Role | Email | Password |
|---|---|---|
| **Admin** | admin@hotel.com | Admin@123 |
| **Receptionist** | receptionist@hotel.com | Recep@123 |

> Credentials are seeded automatically on first run via `DbInitializer.SeedAsync()`

---

## 🏗️ Project Structure

```
HotelBookingSystem/
├── Controllers/
│   ├── AccountController.cs      ← Login / Logout
│   ├── AdminController.cs        ← User management (Admin only)
│   ├── DashboardController.cs    ← Main dashboard stats
│   ├── BookingController.cs      ← Full booking CRUD + check-in/out
│   ├── GuestController.cs        ← Guest CRUD
│   ├── RoomController.cs         ← Room CRUD + availability check
│   └── InvoiceController.cs      ← Invoice generation & print
├── Models/
│   ├── ApplicationUser.cs        ← Extended IdentityUser
│   ├── Room.cs                   ← Room with type/status/amenities
│   ├── Guest.cs                  ← Guest profile
│   ├── Booking.cs                ← Booking with fare fields
│   └── Invoice.cs                ← Invoice linked to booking
├── Data/
│   ├── ApplicationDbContext.cs   ← EF Core DbContext + seed data
│   └── DbInitializer.cs          ← Seeds roles + default users
├── Services/
│   └── BookingService.cs         ← Fare calc, availability, check-in/out
├── ViewModels/
│   └── ViewModels.cs             ← All view model classes
├── Views/
│   ├── Shared/_Layout.cshtml     ← Vertical sidebar layout
│   ├── Account/Login.cshtml
│   ├── Dashboard/Index.cshtml
│   ├── Booking/                  ← Index, Create, Details, Edit
│   ├── Room/                     ← Index, Create, Edit, Availability
│   ├── Guest/                    ← Index, Create, Edit, Details
│   ├── Invoice/                  ← Index, Details, Print
│   └── Admin/                    ← Users, CreateUser, EditUser
└── wwwroot/
    ├── css/site.css              ← Full custom design system
    └── js/site.js                ← Sidebar toggle + fare calculator
```

---

## 🔐 Role Permissions

| Feature | Admin | Manager | Receptionist |
|---|:---:|:---:|:---:|
| Dashboard | ✅ | ✅ | ✅ |
| View Bookings | ✅ | ✅ | ✅ |
| Create Booking | ✅ | ✅ | ✅ |
| Edit Booking | ✅ | ✅ | ✅ |
| View Guests | ✅ | ✅ | ✅ |
| Create/Edit Guest | ✅ | ✅ | ✅ |
| Delete Guest | ✅ | ✅ | ❌ |
| View Rooms | ✅ | ✅ | ✅ |
| Add/Edit Room | ✅ | ✅ | ❌ |
| Delete Room | ✅ | ❌ | ❌ |
| View Invoices | ✅ | ✅ | ✅ |
| User Management | ✅ | ❌ | ❌ |

---

## 🗄️ Database Schema

```
AspNetUsers          → ApplicationUser (Identity + IsActive, FullName)
AspNetRoles          → IdentityRole (Admin, Manager, Receptionist)
Rooms                → RoomId, RoomNumber, RoomType, PricePerNight, Status
Guests               → GuestId, FirstName, LastName, Email, Phone, IdType
Bookings             → BookingId, GuestId→Guests, RoomId→Rooms, Dates, Fares
Invoices             → InvoiceId, BookingId→Bookings, SubTotal, Tax, GrandTotal
```

---

## 💡 Notes

- **Fare Formula**: `(Nights × PricePerNight) + 12% Tax`
- **Auto Invoice**: Generated automatically on guest check-out
- **Room Seeding**: 6 rooms (2 Standard, 2 Deluxe, 2 Suite) seeded on first run
- **Account Lockout**: 5 failed attempts → 15-minute lockout
- Disabled users cannot log in (checked before `PasswordSignInAsync`)
