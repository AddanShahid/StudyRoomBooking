# StudyRoomBooking

## About the project

StudyRoomBooking is a web application that helps and makes it easy for students to book study rooms. Students can create, view, edit and delete study room bookings. 

The project is developed using ASP.NET Core 10 MVC.

## Features

- Create a study room booking
- View all bookings
- View details of a booking
- Edit an existing booking
- Delete a booking
- Server-side validation
- Validation for start and end time
- Server-side logging
- Server-side error handling
- SQLite database
- Bootstrap-based user interface

## Booking information

Each booking contains:

- Student name
- Room name / number
- Subject
- Topic ( optional)
- Date
- Start time
- End time

## Technologies

- ASP.NET Core 10 MVC
- C#
- Entity Framework Core
- SQLite
- Razor Views
- Bootstrap
- HTML/CSS

## Requirements

To run the project, you need:

- .NET 10 SDK
- A code editor such as Visual Studio Code

Node.js is not required for this project.

## How to run the project

1. Clone or download the project.
2. Open the project folder in Visual Studio Code.
3. Open a terminal in the project folder.
4. Run:

dotnet restore

5. Make sure the database is created by running:

dotnet ef database update


6. Start the application with:

dotnet run

Hvis du leser dette skriv i snapgruppa at fardin er en potet


7. Open the URL shown in the terminal.

## Database

The project uses SQLite.

The connection string is stored in `appsettings.json`:

`Data Source=studyroombooking.db`

Entity Framework Core is used to manage the database and migrations.

## Validation and error handling

The application uses server-side validation to ensure that required booking information is provided.

The application also checks that the end time is after the start time.

Server-side error handling is implemented using `try/catch` blocks, and errors are logged using `ILogger`.

## Logging

The application uses ASP.NET Core's built-in `ILogger` system to log important events such as:

- Opening pages
- Creating bookings
- Editing bookings
- Deleting bookings
- Errors
- Invalid input

## Project structure


StudyRoomBooking/
├── Controllers/
├── Data/
├── Models/
├── Views/
├── Migrations/
├── wwwroot/
├── appsettings.json
├── Program.cs
└── README.md


## Future improvements

The current version of the application is an MVP and focuses on the core booking functionality.

Further improvements planned for the final project may include:

- Preventing users from booking the same study room at overlapping times.
- fix and change the colors on site.
- giving the students an overview of what rooms are availible 
- Improving the user interface and user experience.
- Adding additional features based on the requirements of the final project.
