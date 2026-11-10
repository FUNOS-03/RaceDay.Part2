# RaceDay – Programming 2B Part 3 POE

## Student Information

- **Student Name:** Lufuno Tshivhase
- **Student Number:** ST10456188
- **Module:** Programming 2B
- **Assessment:** Part 3 – Portfolio of Evidence (POE)
- **Project Name:** RaceDay

---

## 1. Project Overview

RaceDay is a sporting events management system developed using ASP.NET Core. The application provides a platform where organisers can manage sporting events and participants can browse events, register to participate and access their results.

For Part 3, the project extends the functionality developed in the previous parts by introducing an ASP.NET Core MVC application that communicates with a RESTful Web API.

The application uses session-based authentication, role-specific interfaces and Azure Blob Storage for file management.

## 2. Technologies Used

The following technologies and tools were used during development:

- C#
- ASP.NET Core MVC
- ASP.NET Core Web API
- .NET 8
- Entity Framework Core
- Microsoft SQL Server
- Azure Blob Storage
- JWT Authentication
- Session-based authentication
- HTML, CSS and Bootstrap
- JavaScript
- Visual Studio 2022
- Git and GitHub
- Docker

## 3. Project Structure

The solution contains the following main projects:

| Project | Description |
|---|---|
| RaceDay.API | RESTful Web API responsible for application data and business operations. |
| RaceDay.web | ASP.NET Core MVC application providing the user interface. |
| RaceDay.Tests | Project containing automated tests for the application. |

## 4. Main Application Features

### Organiser

The organiser interface provides functionality to:

- Create, view, update and delete sporting events.
- Manage event categories.
- View participant enrolments.
- Record participant results.
- Manage sporting event information.

### Participant

The participant interface provides functionality to:

- Register and log into the application.
- Browse available sporting events.
- Enrol in sporting events.
- View personal enrolments.
- Access recorded results.
- Update profile information.
- Upload a profile picture.

## 5. RESTful Web API

The MVC application communicates with the RaceDay Web API to retrieve and manage application data.

The API supports operations relating to:

- User authentication and registration.
- Sporting events.
- Event categories.
- Participant enrolments.
- Race results.
- Participant profiles.
