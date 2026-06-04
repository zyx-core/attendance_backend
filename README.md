
# 📚 Student Attendance System

A comprehensive Student Attendance System developed using **ASP.NET Core Web API**. The application provides secure JWT authentication, attendance tracking, leave management, student information management, and role-based access control for teachers and parents.

## 👨‍💻 Team Members

- **Ershad S**
- **Jonadh**
- **Arjun**
- **Ambed**

## 📖 Project Overview

The Student Attendance System is designed to digitize and streamline attendance management in educational institutions. Teachers can efficiently mark attendance, track student records, generate reports, and manage leave requests, while parents can submit leave applications through a secure role-based platform.

## ✨ Features

### 🔐 Authentication
- Teacher Registration
- Parent Registration (with invitation token)
- JWT Token Authentication
- Forgot Password / Reset Password
- Secure Logout

### 📝 Attendance Management
- Mark Attendance (Present/Absent/Late/Excused)
- Update Attendance Records
- View Student Attendance History
- Calculate Attendance Percentage
- Class Average Reports
- Daily & Monthly Reports

### 👨‍🎓 Student Management
- View All Students
- Add New Students
- Edit Student Details
- Delete Student Records
- Search Students (by name, class, status)
- Student Photo Upload (5MB max)

### 📋 Leave Management
- Submit Leave Requests (Teacher/Parent)
- View All Leave Requests
- Approve Leave Requests
- Reject Leave Requests

## 🛠️ Technologies Used

### Backend
- ASP.NET Core Web API (.NET 6+)
- Entity Framework Core
- JWT Authentication
- SQL Server


 
 
 
 <img width="1912" height="672" alt="docker" src="https://github.com/user-attachments/assets/3400679e-1fc1-46be-9f71-36d4e457c18d" />




## ⚙️ Installation

```bash
git clone https://github.com/zyx-core/attendance_backend.git
cd attendance_backend
dotnet restore
dotnet ef database update
dotnet run
