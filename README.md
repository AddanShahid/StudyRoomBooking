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
- fix and change the colors on site before the final delivery.  
- giving the students an overview of what rooms are availible 
- Improving the user interface and user experience.
- Adding additional features based on the requirements of the final project.

---

# Prosjektplan (internt for gruppen)

Eksamensprosjekt i ITPE3200 Webapplikasjoner, 2026H. Vi løser oppgave 1, Study Room Booking Tool: et verktøy der studenter kan booke grupperom til studieøkter og bli med i økter andre har booket.

**Frist:** mandag 30. november 2026 kl. 12:00 (WISEflow)

| Del | Teknologi | Vekt |
|---|---|---|
| Subapp 1 | ASP.NET 10 MVC | ~40 % |
| Subapp 2 | ASP.NET 10 Web API + React (TypeScript) | ~30 % |
| Dokumentasjon | 2000–5000 ord, etter kursets mal | ~30 % |

Vi leverer én zip med begge appene (uten `bin`, `obj`, `node_modules` og `dist`) og én PDF med dokumentasjonen.

---

## Planlagt mappestruktur

```
/SubApp1.Mvc        ASP.NET MVC-appen
/SubApp1.Tests      xUnit-tester
/SubApp2.Api        ASP.NET Web API
/SubApp2.Client     React + TypeScript (Vite)
/docs               Dokumentasjon
```

---

## Domenemodell (forslag)

| Entitet | Relasjon | Innhold |
|---|---|---|
| **Room** | 1:N StudySession | Navn, bygning, etasje, kapasitet |
| **Equipment** | N:M Room | Prosjektor, whiteboard, skjerm (dekker N:M-kravet) |
| **Course** | 1:N StudySession | Emnekode (f.eks. ITPE3200), navn |
| **StudySession** | N:1 Room, N:1 Course | Tema, start, slutt, maks deltakere, opprettet av |
| **Participation** | Bruker N:M StudySession | Hvem som har blitt med i en økt, og når |
| **Message** | N:1 StudySession | Tekst, forfatter, tidspunkt |

**Domeneregler på serveren:**
- Ingen overlappende økter i samme rom.
- Man kan ikke bli med i en full økt.
- Man kan ikke booke tilbake i tid.
- En økt kan vare maks 4 timer.
- Antall deltakere kan ikke være høyere enn romkapasiteten.

---

## Roller

Bytt ut «Medlem A–D» med navn.

### Medlem A: Data og domene
- Modeller, `DbContext`, SQLite, migrasjoner
- Seeding (`DBInit`) med realistiske rom og emner
- Repository-mønster og DAL, async/await
- Domeneregler (overlapp, kapasitet osv.)
- 12+ enhetstester (xUnit + Moq), 6 positive og 6 negative
- **Dokumentasjon:** kap. 4 Database, testdelen av kap. 6

### Medlem B: MVC-grensesnitt
- Controllere og views for Room, Course og StudySession (CRUD)
- ViewModels og partial views (tabell, grid, kort)
- Søk og filtrering på emne, dato, bygning og utstyr
- Conditional rendering, navigasjon, layout og design
- Validering på klientsiden
- **Dokumentasjon:** kap. 8 UX og brukervennlighet, kap. 5 for Subapp 1

### Medlem C: Sikkerhet og API
- Identity med rollene Admin og Student i MVC
- Feilhåndtering, `ILogger` overalt, validering på serveren
- Subapp 2 API: controllere, DTOs og gjenbruk av DAL
- JWT-autentisering i API-et, CORS
- Participation og Message (bli med, forlate, meldinger)
- **Dokumentasjon:** kap. 3 Arkitektur og ADR-er, sikkerhetsdelen av kap. 6

### Medlem D: React-klient
- Vite + React + TypeScript-oppsett
- API-servicelag (egne `services/`-filer)
- Komponenter og sider for CRUD, søk og filtrering
- Innlogging med JWT på klienten, beskyttede ruter
- Kravmatrisen gjennom hele prosjektet
- **Dokumentasjon:** kap. 2 Krav og traceability, kap. 5 for Subapp 2

**Alle** skriver kap. 1 (metadata), kap. 7 (bruk av AI) og kap. 9 (konklusjon og egen refleksjon). Hver person noterer fortløpende hvordan de brukte AI og hvordan de sjekket resultatet.

---

## Uke for uke

> **Første møte (uke 44):** Bli enige om domenemodellen. Alle de andre delene bygger på den, så Medlem A må ha modellene klare og pushet innen fredag i uke 44.

| Uke | Medlem A | Medlem B | Medlem C | Medlem D |
|---|---|---|---|---|
| **44** (26.10–01.11) | Rydde i git og flytte til `SubApp1.Mvc`. Modeller, DbContext, første migrasjon og seeding. | Layout, navigasjon og fargeprofil. Skisser av sidene. Starte på RoomController med views. | Legge til Identity, registrering og innlogging. Global feilside og oppsett av logging. | Sette opp React-prosjektet. Komponenter med testdata. Starte på kravmatrisen. |
| **45** (02.11–08.11) | Repositories for alle entiteter. Domeneregler med logging. | CRUD-views for Course og StudySession. ViewModels og partial views. | Autorisasjon med roller. Participation og Message i MVC. Opprette API-prosjektet. | Servicelag mot API-et etter hvert som endepunktene kommer. TypeScript-typer for DTO-ene. |
| **46** (09.11–15.11) | Testprosjekt med 12+ tester. Hjelpe C med å gjenbruke DAL-en i API-et. | Søk og filtrering på flere parametere. Validering på klientsiden. Polering av design. | API-controllere med DTOs og validering. JWT og CORS. | CRUD-sider i React med skjema og validering. Conditional rendering. |
| **47** (16.11–22.11) | Skrive kap. 4. Teste MVC-appen og fikse feil. | Tilgjengelighet og UX-gjennomgang. Skrive kap. 8. | Feilhåndtering og logging i API-et. Skrive kap. 3 og sikkerhetsdelen. | Innlogging og beskyttede ruter i React. Søk og filtrering. Skrive kap. 2. |
| **48** (23.11–29.11) | **Alle:** Ingen nye funksjoner. Rette advarsler fra `dotnet build` og `npm run build`. Testkjøre fra en ren klone på en annen maskin. Ferdigstille dokumentasjonen og korrekturlese. Lage zip uten `bin`, `obj`, `node_modules` og `dist`. Levere senest søndag 29.11, så har vi mandag morgen som buffer. | | | |

---

## Pensum per person

Modulene i Canvas, sortert etter rolle og i den rekkefølgen de bør leses. Vi trenger ikke Angular-materialet fra 2023, siden vi bruker React.

### Alle (denne uken)
- Reading: how to work as a group?
- Info: Using .gitignore to avoid unnecessary sync between group members push
- Reading: Introduction to GitHub (for collaboration and version control)
- FAQ: Examination & Mandatory Assignments
- FAQ: Can I use Generative AI for the Course?
- Lecture: How to work with AI (kap. 7 i dokumentasjonen)
- ITPE3200 Project Documentation Template (2026H) + Detailed Explanations & Examples
- Lecture: Overview of MVC (alle må forstå arkitekturen)
- Demo: Shop MVC (malen hele prosjektet bygger på)

### Medlem A: Data og domene
1. Lecture: Entity Framework
2. Demo: DbContext
3. Demo: Database CRUD
4. Demo: Asynchronous Database and Migrations
5. Demo: Database seeding
6. Demo: Expanded database with CRUD (relasjoner og N:M)
7. Demo: Repository pattern and data access layer (DAL)
8. Reading: Entity framework and DAL
9. Reading: Essential Strategies and Practices for Entity Framework Core
10. Summary Lecture: EntityFrameworkSummary
11. Demo: Unit testing
12. Reading: Software testing
13. Lecture: Security and Unit Testing (testdelen)

### Medlem B: MVC-grensesnitt
1. Demo: HomeView, Navigation and Routing
2. Demo: Dynamic Content, Client-Side Files
3. Demo: Dynamic Views, View Model, Partial View
4. Summary Lecture: MVC
5. Reading: Reference Answers MVC
6. Demo: Debugging
7. Demo: Database CRUD (controller- og view-delen)
8. Demo: Input validation (klientsiden)
9. Lecture: Usability and Accessibility of Websites
10. Ekstra: Bootstrap på kunde-applikasjonen, Responsivt grid, Navigering Bootstrap

### Medlem C: Sikkerhet og API
1. Lecture: Logging, Error Handling, and Input Validation
2. Demo: Error handling and logging
3. Demo: Input validation (serversiden)
4. Demo: Authentication and authorisation
5. Lecture: Security and Unit Testing (sikkerhetsdelen)
6. Demo: Repository pattern and DAL (for å gjenbruke den i API-et)
7. Demo-React-2-APIBackendAndDebugging
8. Demo-React-9-JWTAuthentication-Backend
9. Ekstra: Forskjell på GET og POST

### Medlem D: React-klient
1. Lecture: Introduction to React
2. Demo-React-0-HelloWorld
3. Demo-React-1-BuildingComponents
4. Demo-React-1.1-Debugging
5. Demo-React-5-TypeScript
6. Demo-React-3-HttpGet
7. Demo-React-4-ConditionalRendering
8. Demo-React-6-HttpPost
9. Demo-React-7-HttpPutDelete
10. Demo-React-8-ItemService (API-servicelaget)
11. Demo-React-10-JWTAuthentication-Frontend
12. Ekstra: Kompendie for JavaScript (ved behov)

---

## Hvem har ansvar for hvert krav

| Krav | Gjelder | Ansvarlig |
|---|---|---|
| 3+ domene-entiteter med CRUD | MVC + SPA | B, D |
| Minst én N:M-relasjon | Begge | A |
| Kompleks domeneregel på serveren | Begge | A |
| To typer conditional rendering | MVC + SPA | B, D |
| Søk og filtrering på flere parametere | MVC + SPA | B, D |
| Repository og DAL, async/await | Begge | A |
| ViewModels | MVC | B |
| DTOs | API | C |
| API-servicelag på klienten | SPA | D |
| Validering på klient og server | Begge | C, B, D |
| Feilhåndtering og logging med ILogger | Begge | C |
| Autentisering og autorisasjon (Identity / JWT, roller) | Begge | C, D |
| 12+ enhetstester, 6 positive og 6 negative | MVC | A |
| Design og UX | Begge | B, D |
| Bygg uten advarsler | Begge | Alle |

---

## Arbeidsregler

- **Én gren per oppgave.** Lag f.eks. `feature/room-crud` og merge til `main` via pull request. Push aldri direkte til main.
- **Én reviewer per PR.** En annen i gruppen ser over koden før merge. Da lærer alle hele kodebasen, noe vi trenger i refleksjonsdelen.
- **Aldri `bin/` og `obj/`.** Sjekk `git status` før hver commit. Byggefiler gir merge-konflikter.
- **Kort statusmøte to ganger i uken.** F.eks. tirsdag etter forelesning og fredag. Hva er gjort, hva er neste, hva stopper oss.
- **Skriv mens vi koder.** Noter beslutninger (ADR-er) og bruk av AI med en gang. Det er mye vanskeligere å huske det i uke 48.
- **Felles stil.** Engelske navn i koden og korte engelske kommentarer der logikken ikke er åpenbar.

