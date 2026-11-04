# 🚗 AutoSync

**AutoSync** is a console-based car management and logistics system written in **C#**. It lets you register and control user access, vehicles, maintenance requests, driver CNHs (driver's licenses) and reports, all from the terminal.

The project simulates a real-world system for access control and information management, including users, access logs, maintenance, vehicle registration and drivers' CNHs.

---

## ✨ Features

- 🔐 **Authentication** with role-based access (Admin, Operator, Driver)
- 🙈 **Hidden password input** (`*******`)
- 🚫 **Login attempt limit**: 3 attempts before a temporary block
- 🧾 **Access logs**: every successful login is recorded with user ID, date and time
- 🚘 **Vehicle registration** with validation rules (cargo / passenger)
- 🛠️ **Maintenance requests** with an approval / rejection flow
- 🪪 **CNH and Company ID consultation** for Operators
- 👥 **Driver and Operator registration**, restricted to Admins (simulating in-person registration)
- 💾 **JSON-based persistence** (no external database required)

---

## 👥 Users and Roles

| Role | What it can do | Limit |
|------|----------------|-------|
| **Driver** | Register a vehicle, request / view maintenances, verify vehicle and account status | Unlimited |
| **Operator** | Check CNHs and Company ID, approve / reject maintenance requests | 3 |
| **Admin** | Full access: register drivers and operators, create financial reports, view access logs | 1 |

> New Drivers and Operators **cannot sign up on their own**. They can only be added by an Admin, simulating an in-person registration system.

### Default users (demo data)

| User | Role | ID | Password |
|------|------|----|----------|
| Harry Jenkins | Driver | `Harry` | `H4RR1` |
| Rose Baker | Operator | `Rose` | `R0S3` |
| Joseph Williams | Admin | `Joseph` | `J0S3P4` |

> ⚠️ These credentials are **demo data for learning purposes only**. Never use real passwords in this project.

---

## 🔐 Security Triggers

On sign-in, the system applies the following rules:

- 3 login attempts before a temporary block
- Password typing is hidden
- When authentication succeeds, an **Access Log** is saved with the user ID and the date/time

Example entry in `AccessLogs.json`:

```json
{
    "userID": "Harry",
    "Date": "19/10/2026 21:33"
}
```

---

## 📋 Menus

### Driver Menu
1. Register a vehicle
2. Request / view maintenances
3. Verify registered vehicles

### Operator Menu
1. Check CNHs
2. Check Company ID
3. Approve / reject maintenance requests

### Admin Menu
1. Register a new Driver
2. Register a new Operator

---

## 🚘 Vehicle Registration

To register a vehicle, the driver must provide:

| Field | Description |
|-------|-------------|
| **Max Velocity** | Maximum speed of the vehicle |
| **License Plate** | 4 letters and 3 numbers |
| **Model** | The vehicle's brand (BMW, Porsche, Audi, etc.) |
| **Year** | The vehicle's year |
| **Capacity** | How much weight the vehicle supports |
| **Type** | `Cargo` or `Passengers` |

**Rules:**
- **Cargo** vehicles have a capacity of **500 kg or more** and cannot exceed **120 km/h**.
- **Passenger** vehicles cannot handle more than **500 kg**.

---

## 🛠️ Maintenance Request

The driver must inform:

- **Description:** what happened to the vehicle
- **Type:** Corrective or Preventive
- **Date:** when the driver wants the maintenance done

An Operator then approves or rejects the request. When approved, the Operator returns a **price** for the maintenance and the request status changes to `WAITING`. The Driver can check the result in the Driver Menu.

---

## 🗂️ Project Structure

```
AutoSync/
├── AutoSync.sln
├── Program.cs
├── CLI/
│   ├── CLISystem.cs
│   └── System.cs
├── Data/
│   ├── Database.cs
│   ├── Users.json
│   ├── AccessLogs.json
│   ├── Vehicles.json
│   ├── Drivers.json
│   └── Operators.json
├── Interfaces/
│   ├── ILogin.cs
│   └── Menus/
│       ├── AdminMenu.cs
│       ├── OperatorMenu.cs
│       └── DriverMenu.cs
└── Services/
    ├── VehicleRegistrationService.cs
    ├── CNHRegistrationService.cs
    ├── DriverRegistrationService.cs
    ├── OperatorRegistrationService.cs
    └── MaintenanceService.cs
```

### How the pieces fit together

- **`Interfaces/ILogin.cs`**: handles the login flow, checks the user's role and loads the matching menu.
- **`Interfaces/Menus/`**: one menu per role (`AdminMenu`, `OperatorMenu`, `DriverMenu`). They call the services.
- **`Services/`**: scripts for specific actions, such as registering and viewing vehicles, CNHs and maintenances.
- **`Data/Database.cs`**: manages all data (users, vehicles, drivers, operators, maintenances).
- **`CLI/CLISystem.cs`**: terminal helpers such as:
  - stylized messages (foreground / background colors)
  - Yes/No questions
  - skipping rows
  - tables and info boxes
  - date input handling
  - hidden input (`*`)
  - action / option lists
- **`CLI/System.cs`**: helper methods for working with data, such as conversions.

### Data files

| File | Content |
|------|---------|
| `Users.json` | Authentication info (user ID and password), separated by role |
| `AccessLogs.json` | Successful logins with user ID and date |
| `MaintenanceRequests.json` | Maintenance requests created by Drivers |
| `Drivers.json` | Drivers' CNHs: full name, birth date, registration number, category, validity and security code |
| `Operators.json` | Operators' company IDs: full name, birth date and Company ID |
| `Vehicles.json` | Registered vehicles and their info |

---

## 🚀 Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (use the version targeted by `AutoSync.sln`)
- A terminal (Windows Terminal, PowerShell, bash, etc.)

### Running

```bash
# Clone the repository
git clone https://github.com/ItalloCasimiro/AutoSync.git
cd AutoSync

# Build and run
dotnet build
dotnet run
```

### Quick tour

1. Run the program and log in with one of the demo users above.
2. Explore the menu for that role.
3. Log in as **Joseph** (Admin) to register new Drivers and Operators.
4. Log in as **Harry** (Driver) to register a vehicle and request a maintenance.
5. Log in as **Rose** (Operator) to approve or reject that maintenance request.

---

## 🧰 Tech Stack

- **C#** / **.NET**
- **System.Text.Json** for data persistence
- Console / CLI interface

---

## 🗺️ Roadmap / Coming Soon Features

- [ ] Financial reports (create / view) - Admins only
- [ ] Access logs visualization for Admins
- [ ] Travels (assign drivers, accept / reject travels, Client Role)
- [ ] Password hashing

---

## 📄 License

This project is for educational and portfolio purposes.

---

## 👤 Author

Made by **Itallo Casimiro**: [GitHub](https://github.com/ItalloCasimiro)