# 🚗 Car Sales Management API

<div align="center">
  <img src="https://img.shields.io/badge/.NET%20Core-8.0-blue" alt=".NET Core">
  <img src="https://img.shields.io/badge/Angular-18-red" alt="Angular">
  <img src="https://img.shields.io/badge/Architecture-Clean%20Architecture-green" alt="Architecture">
  <img src="https://img.shields.io/badge/Database-SQL%20Server-orange" alt="Database">
  <img src="https://img.shields.io/badge/Status-Production%20Ready-brightgreen" alt="Status">
</div>


<p align="center">
<table>
<tr>
<td><img src="Images/1.png" width="250"></td>
<td><img src="Images/2.png" width="250"></td>
<td><img src="Images/3.png" width="250"></td>
</tr>
<tr>
<td><img src="Images/4.png" width="250"></td>
<td><img src="Images/5.png" width="250"></td>
<td><img src="Images/6.png" width="250"></td>
</tr>
</table>
</p>









Car Sales Management System is a full-stack web application developed using .NET Core for backend, Angular 14 for frontend, and Microsoft SQL Server as the database. The system manages car models, users, roles, and sales commission calculations.

---

## Tech Stack

- Backend: .NET Core 8, C#
- Frontend: Angular 14
- Database: Microsoft SQL Server
- Data Access: Dapper
- Authentication: JWT
- Tools: Git, GitHub, Swagger, Postman

---

## Features

- Car model management (Add, Update, Delete, View)
- Brand and car class management
- Role-based authentication and authorization
- Sales commission calculation based on business rules
- Secure REST APIs using JWT
- Angular frontend with guards and interceptors

---

## Project Structure

CarSalesManagement/
│
├── API/            → .NET Core Backend
├── UI/             → Angular Frontend
├── Database/
│   ├── schema.sql
│   ├── seed-data.sql
│   └── ERD.png
└── README.md

---

## Database

The Database folder contains:
- schema.sql → Table creation scripts
- seed-data.sql → Initial insert scripts
- ERD.png → Entity Relationship Diagram

![ER Diagram](Images/ERD.png)

---

## Setup Instructions

### Backend Setup
1. Open the API project in Visual Studio
2. Update SQL Server connection string in `appsettings.json`
3. Execute `schema.sql` and `seed-data.sql` in SQL Server
4. Run the API project
5. Swagger will be available at `/swagger`

### Frontend Setup
1. Go to `UI/car-sales-ui`
2. Install dependencies:
   npm install
3. Run application:
   ng serve

---

## API Documentation

Swagger UI is enabled for API testing:
`/swagger`

---

## Author

Palak Mishra  
.NET & Angular Developer  
GitHub: https://github.com/mispalak9
