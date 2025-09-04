"# E-CommerceSystem-" 


![Status](https://img.shields.io/badge/Project-E--Commerce-blue?style=for-the-badge&logo=github)
![Framework](https://img.shields.io/badge/.NET%20Core-Backend-success?style=for-the-badge&logo=dotnet)
![Database](https://img.shields.io/badge/SQL%20Server-Database-orange?style=for-the-badge&logo=microsoftsqlserver)
![License](https://img.shields.io/badge/Status-Completed-green?style=for-the-badge)

---

## ✨ Overview
The **Order Management System (OMS)** is a **scalable e-commerce backend** designed with **clean architecture, security, and analytics** in mind.  
This is **Part 2** of the project, where advanced features, reporting mechanisms, and business rules were added.  

> 💡 Built to showcase **enterprise-level backend engineering** with production-ready practices.

---

## 🎯 Features at a Glance
✔️ **Users** – Customers, Admins, Managers with role-based access  
✔️ **Products** – CRUD operations, category & supplier assignment, image upload  
✔️ **Orders** – Place, cancel, track status (`Pending → Paid → Shipped → Delivered → Cancelled`)  
✔️ **Reviews** – Only verified buyers can post, one review per product  
✔️ **Reports** – Best-selling products, revenue trends, active customers  
✔️ **Security** – JWT with refresh tokens, password hashing, cookie storage  
✔️ **Code Quality** – Centralized error handling, logging, AutoMapper  

---

## 🏗 System Design

### 🔹 Architecture
- **Controller Layer** → Handles API requests.  
- **Service Layer** → Business rules & workflows.  
- **Repository Layer** → Data access using EF Core.  
- **Database** → SQL Server with migrations.  

📌 **Every Model has its own Repository + Service + Controller + DTOs.**

---

### 🔹 Entity-Relationship Diagram (ERD)

[ User ]───< places >───[ Order ]───< contains >───[ OrderProducts ]───< includes >───[ Product ]
│ │
└──< writes >──[ Review ] [ Category ]──< has >───[ Product ]
[ Supplier ]──< supplies >───[ Product ]

*(Diagram image placeholder — see `/docs/erd.png`)*  

---

## 📊 Reporting Mechanism

📌 **Workflow:**  
Admin → API Request → Report Service → Database → Aggregation → API Response → Dashboard  

📈 **Reports Available:**  
- Best-selling products  
- Revenue (daily & monthly)  
- Top-rated products  
- Most active customers  

*(Dashboard chart placeholders — see `/docs/reports.png`)*  

---

## 🔐 Security Highlights
- **JWT Authentication** with **refresh tokens**  
- **BCrypt password hashing**  
- **Cookies** for token storage  
- **Role-based access**: `Admin`, `Customer`, `Manager`  

---

## 🚀 Tech Stack
- **Backend**: ASP.NET Core  
- **Database**: SQL Server (EF Core ORM)  
- **Logging**: Serilog / ILogger  
- **Mapping**: AutoMapper  
- **Reports**: LINQ aggregation + PDF/CSV export  

---

## 📸 Screenshots (Flex Zone 😎)
*(Add screenshots of your dashboard here to impress reviewers)*

- 🖼 **Admin Dashboard** – Reports & Analytics  
- 🖼 **Product Management** – Category & Supplier assignment  
- 🖼 **Order Tracking** – Status flow and cancellation  

---

## 🎨 Future Enhancements
🚀 Payment Gateway Integration  
🚀 AI-powered product recommendations  
🚀 Multi-language support  
🚀 Cloud deployment with CI/CD pipelines  

---

## 🙌 Authors
👨‍💻 **Mohammed Yusuf Alkhusaibi  **  
👨‍💻 **Ishaq AL baluchi GTR  **  
Backend Developer | Database Designer | Full Stack Trainee  


## API Endpoints

| Module   | API Endpoint                                   | Method | Role                  |
|----------|-----------------------------------------------|--------|-----------------------|
| **Order** | POST `/api/Order/PlaceOrder`                  | POST   | Admin                 |
|          | GET `/api/Order/GetAllOrders`                  | GET    | User                  |
|          | GET `/api/Order/GetOrderById/{orderId}`        | GET    | User (same order ID)  |
|          | GET `/api/Order/GetOrderSummaries`             | GET    | User                  |
|          | PUT `/api/Order/CancelOrder/{orderId}`         | PUT    | Admin, User (own ID)  |
|          | PUT `/api/Order/UpdateStatus/{orderId}`        | PUT    | Admin                 |
| **Product** | POST `/api/Product/AddProduct`              | POST   | Admin                 |
|          | PUT `/api/Product/UpdateProduct/{productId}`   | PUT    | Admin                 |
|          | GET `/api/Product/GetAllProducts`              | GET    | User                  |
|          | GET `/api/Product/GetProductByID/{productId}`  | GET    | Admin                 |
|          | POST `/api/Product/UploadImage/{productId}`    | POST   | Admin                 |
| **Report** | GET `/api/Report/best-selling-products`      | GET    | Admin                 |
|          | GET `/api/Report/daily-revenue`                | GET    | Admin                 |
|          | GET `/api/Report/monthly-revenue`              | GET    | Admin                 |
|          | GET `/api/Report/top-rated-products`           | GET    | Admin                 |
|          | GET `/api/Report/most-active-customers`        | GET    | Admin                 |
| **Review** | POST `/api/Review/AddReview`                 | POST   | User                  |
|          | GET `/api/Review/GetAllReviews`                | GET    | User                  |
|          | DELETE `/api/Review/DeleteReview/{ReviewId}`   | DELETE | User                  |
|          | PUT `/api/Review/UpdateReview/{ReviewId}`      | PUT    | User                  |
| **Supplier** | GET `/api/Supplier`                        | GET    | Admin                 |
|          | POST `/api/Supplier`                           | POST   | Admin                 |
|          | GET `/api/Supplier/{id}`                       | GET    | Admin                 |
|          | PUT `/api/Supplier/{id}`                       | PUT    | Admin                 |
|          | DELETE `/api/Supplier/{id}`                    | DELETE | Admin                 |
| **User** | POST `/api/User/register`                      | POST   | New User              |
|          | POST `/api/User/refresh-token`                 | POST   | User (token)          |
|          | POST `/api/User/login`                         | POST   | User (email/password) |
|          | GET `/api/User/GetUserById/{UserID}`           | GET    | Admin (with ID)       |
| **Category** | GET `/api/Category`                        | GET    | User                  |
|          | POST `/api/Category`                           | POST   | Admin                 |
|          | GET `/api/Category/{id}`                       | GET    | User                  |
|          | PUT `/api/Category/{id}`                       | PUT    | Admin                 |
|          | DELETE `/api/Category/{id}`                    | DELETE | Admin                 |
| **Invoice** | GET `/api/Invoice/{orderId}/pdf`            | GET    | Admin                 |


📌 *Part of my portfolio showcasing enterprise-grade backend solutions.*

---
