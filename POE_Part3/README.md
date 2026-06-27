# POE_Part3
POE Part 3 – Cybersecurity Awareness Chatbot

Student Information

Module: Programming POE Part 3
Project: Cybersecurity Awareness Chatbot
Framework: .NET 8 WPF (Windows Presentation Foundation)
Language: C#
Database: SQL Server LocalDB

⸻

Project Overview

This application is an AI-inspired cybersecurity awareness chatbot developed as part of the Programming POE Part 3 assessment. The chatbot educates users on cybersecurity best practices while also providing a task management system where users can create, complete, delete, and manage reminder tasks.

The application uses a conversational interface that allows users to interact naturally with the chatbot.

⸻

Features

Cybersecurity Chatbot

•⁠  ⁠Responds to user messages
•⁠  ⁠Provides cybersecurity awareness information
•⁠  ⁠Interactive conversation interface
•⁠  ⁠Chat bubble UI

Task Management

Users can:

•⁠  ⁠Add new tasks
•⁠  ⁠View saved tasks
•⁠  ⁠Mark tasks as completed
•⁠  ⁠Delete tasks
•⁠  ⁠Set task reminders
•⁠  ⁠Store tasks in SQL Server LocalDB

User Interface

•⁠  ⁠Modern WPF interface
•⁠  ⁠Chat-style conversation layout
•⁠  ⁠Simple and user-friendly navigation

⸻

Technologies Used

•⁠  ⁠C#
•⁠  ⁠.NET 8
•⁠  ⁠WPF
•⁠  ⁠SQL Server LocalDB
•⁠  ⁠XAML

⸻

Database

The application connects to SQL Server LocalDB using the following connection string:

Server=(localdb)\MSSQLLocalDB;
Database=TaskChat;
Trusted_Connection=True;
TrustServerCertificate=True

Ensure the TaskChat database has been created before running the application.

⸻

Project Structure

POE_Part3
│
├── Assets
│   └── Greeting.wav
│
├── Models
│   └── UserProfile.cs
│
├── Services
│   ├── AudioPlayer.cs
│   └── Questions.cs
│
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── WindowGame.xaml
├── WindowGame.xaml.cs
│
└── App.xaml

⸻

Requirements

Before running the project, ensure you have:

•⁠  ⁠Visual Studio 2022
•⁠  ⁠.NET 8 SDK
•⁠  ⁠SQL Server LocalDB
•⁠  ⁠Windows Operating System

⸻

How to Run

1.⁠ ⁠Extract the ZIP file.
2.⁠ ⁠Open the solution/project in Visual Studio.
3.⁠ ⁠Restore all NuGet packages.
4.⁠ ⁠Ensure SQL Server LocalDB is installed.
5.⁠ ⁠Create the TaskChat database if it does not already exist.
6.⁠ ⁠Build the solution.
7.⁠ ⁠Run the application.

⸻

Example Commands

Create a task

Add task

Delete a task

Delete 1

Complete a task

Done 1

View tasks

Show tasks

⸻

Future Improvements

•⁠  ⁠User authentication
•⁠  ⁠AI integration
•⁠  ⁠Email reminders
•⁠  ⁠Calendar integration
•⁠  ⁠Task prioritisation
•⁠  ⁠Cloud database support
•⁠  ⁠Enhanced cybersecurity knowledge base

⸻

Author

Student: Siphelele Maduna
Module: Programming POE Part 3
Institution: Rosebank College

⸻

Notes

This project was developed for educational purposes as part of the Programming POE Part 3 practical assessment. It demonstrates the use of object-oriented programming, WPF application development, SQL Server database integration, and chatbot interaction principles.
