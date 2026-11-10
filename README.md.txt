
# RaceDay – Sporting Events Management System

## Student Information

- **Student Name:** Lufuno Tshivhase
- **Student Number:** ST10456188
- **Module:** Programming 2B
- **Project:** RaceDay

---

## 1. Project Overview

RaceDay is a sporting events management system developed as part of the Programming 2B practical assignment.

The system provides a platform where event organisers can manage sporting events, categories, participant registrations and race results. Participants can register, log in, browse upcoming sporting events, enrol in events and view their personal results.

The application uses an ASP.NET Core MVC web application that communicates with a separate ASP.NET Core RESTful Web API. Data is managed through Entity Framework Core and Microsoft SQL Server, while Azure Blob Storage is used for image storage.

---

## 2. Technologies and Tools

The following technologies and tools were used:

- C#
- .NET 8
- ASP.NET Core MVC
- ASP.NET Core Web API
- RESTful API integration
- Entity Framework Core
- Microsoft SQL Server
- Azure Blob Storage
- JWT Authentication
- Session-based authentication
- Role-based authorisation
- HTML5
- CSS3
- Bootstrap
- JavaScript
- Docker
- Visual Studio 2022

---

## 3. Solution Structure

The solution consists of three projects:

### RaceDay.API

The backend RESTful Web API responsible for:

- User registration and authentication.
- Event management.
- Event category management.
- Participant enrolments.
- Race result management.
- Profile management.
- Image upload and retrieval through Azure Blob Storage.
- Database operations through Entity Framework Core.

### RaceDay.web

The ASP.NET Core MVC frontend responsible for:

- Displaying the RaceDay website.
- Communicating with the RESTful API.
- Session-based user authentication.
- Organiser and participant dashboards.
- Event browsing and management.
- Participant enrolments and results.
- Profile management.
- Form validation and user feedback.

### RaceDay.Tests

The testing project created for application testing.

---

## 4. System Features

### 4.1 User Authentication

- User registration.
- User login and logout.
- JWT-based authentication.
- Session-based authentication in the MVC application.
- Role-based access for Organisers and Participants.

### 4.2 Organiser Functionality

Organisers can:

- Access an organiser dashboard.
- Create sporting events.
- View available events.
- Update event information.
- Delete events.
- Upload event banner images.
- Manage event categories.
- View participant enrolments.
- Record race results.

### 4.3 Participant Functionality

Participants can:

- Register and log in.
- Access a participant dashboard.
- Browse upcoming sporting events.
- View event details.
- View event banner images.
- Enrol in sporting events.
- View their enrolments.
- Cancel their enrolments.
- View their race results.
- Update their profile information.
- Upload a profile picture.

---

## 5. Database Implementation

RaceDay uses Microsoft SQL Server for persistent data storage.

Entity Framework Core is used for database access and management.

The database includes the application's main entities, including:

- Users
- Events
- Event Categories
- Enrolments
- Results

Database schema changes are managed through Entity Framework Core migrations.

---

## 6. Azure Blob Storage

Azure Blob Storage is integrated into the RaceDay API to store uploaded images.

The application uses Azure Blob Storage for:

- Participant profile pictures.
- Sporting event banner images.

The API handles image uploads and retrieval. Storage credentials are configured securely through development secrets or environment variables and should not be committed to the GitHub repository.

---

## 7. Application Security

Security features implemented include:

- JWT authentication.
- Role-based authorisation.
- Session-based authentication.
- Password hashing.
- Protected API endpoints.
- Server-side form validation.
- Secure configuration of sensitive credentials.

Organiser and Participant permissions are separated to restrict access to authorised functionality.

---

## 8. Running the Application

### Requirements

- Visual Studio 2022.
- .NET 8 SDK.
- Microsoft SQL Server or SQL Server Express.
- Azure Storage account.
- Internet connection for Azure services.

### Setup Instructions

1. Clone or download the RaceDay repository.
2. Open `RaceDay.Part2.sln` in Visual Studio 2022.
3. Configure the SQL Server connection string.
4. Configure the Azure Blob Storage connection string using User Secrets.
5. Ensure the required Entity Framework Core migrations have been applied.
6. Configure `RaceDay.API` and `RaceDay.web` as startup projects.
7. Run the solution.

The API and MVC applications must both be running for the complete application to function.

---

## 9. Docker

A Dockerfile is included in the RaceDay.web project for containerising the MVC application.

The application requires access to the RaceDay API, SQL Server and Azure Blob Storage configuration when running in a containerised environment.

Docker configuration and execution instructions should be followed using the appropriate environment-specific connection settings.

---

## 10. Testing

The application should be tested to verify:

- User registration and login.
- Organiser and Participant access restrictions.
- Event creation, editing and deletion.
- Event category management.
- Participant enrolments and cancellations.
- Recording and viewing race results.
- Profile updates.
- Profile picture uploads.
- Event banner uploads.
- API communication.
- Database persistence.

---

## 11. Repository Structure

```text
RaceDay.Part2/
│
├── RaceDay.API/
│   ├── Controllers/
│   ├── Data/
│   ├── Models/
│   ├── Services/
│   ├── Migrations/
│   └── Program.cs
│
├── RaceDay.web/
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   ├── Views/
│   ├── wwwroot/
│   ├── Dockerfile
│   └── Program.cs
│
├── RaceDay.Tests/
│
├── RaceDay.Part2.sln
└── README.md
```

---

## 12. Conclusion

RaceDay demonstrates the development of a multi-project sporting events management system using C# and ASP.NET Core.

The project integrates an MVC frontend with a RESTful Web API, database management through Entity Framework Core, authentication and authorisation, and cloud-based image storage using Azure Blob Storage.

The system provides separate functionality for organisers and participants and demonstrates the practical application of modern software development concepts covered in Programming 2B.

---

## Author

**Lufuno Tshivhase**

**Student Number:** ST10456188

**Module:** Programming 2B
```

**Save the README.md.**

One important submission check remains: the assignment's actual POE rubric should be compared against the completed project before we claim every requirement has been met. We will not invent completed features or testing results.