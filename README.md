# Prosjektplan (internt for gruppen)

Eksamensprosjekt i ITPE3200 Webapplikasjoner, 2026H. Oppgave 1: Study Room Booking Tool.

**Frist:** mandag 30. november 2026 kl. 12:00 (WISEflow). Vi leverer senest søndag 29.11.

| Del | Teknologi | Vekt |
|---|---|---|
| Subapp 1 | ASP.NET 10 MVC | ~40 % |
| Subapp 2 | ASP.NET 10 Web API + React (TypeScript) | ~30 % |
| Dokumentasjon | 2000–5000 ord, etter kursets mal (PDF) | ~30 % |

Vi leverer én zip med begge appene (uten `bin`, `obj`, `node_modules` og `dist`) og én PDF med dokumentasjonen.

---

## Status (27.10)

| Krav | Subapp 1 (MVC) | Subapp 2 (API + React) |
|---|---|---|
| CRUD på minst 1 entitet | ✅ Booking | ❌ |
| 3 domene-entiteter med CRUD | ⚠️ 1 av 3 (Room mangler sider, én entitet til mangler) | ❌ |
| N:M-relasjon | ❌ | ❌ |
| Kompleks domeneregel på serveren | ✅ Ingen dobbeltbooking | ❌ |
| Repository + DAL, async/await | ✅ | ❌ |
| Validering på klient og server | ✅ | ❌ |
| Feilhåndtering og logging (ILogger) | ✅ | ❌ |
| ViewModels / DTOs | ❌ | ❌ |
| Søk og filtrering på flere parametere | ❌ | ❌ |
| 2 typer conditional rendering | ⚠️ 1 av 2 | ❌ |
| Autentisering og autorisasjon | ❌ | ❌ |
| 12 enhetstester (6 positive, 6 negative) | ❌ | – |
| API-servicelag på klienten | – | ❌ |
| Design og UX | ⚠️ Grunnleggende | ❌ |
| Dokumentasjon | ❌ | |

---

## Domenemodell

| Entitet | Relasjon | Innhold | Status |
|---|---|---|---|
| **Room** | 1:N Booking, N:M Equipment | Navn, lokasjon, kapasitet, beskrivelse | ✅ Modell, ❌ sider |
| **Booking** | N:1 Room, 1:N Participation | Student, emne, tema, dato, start, slutt | ✅ |
| **Equipment** | N:M Room | Prosjektor, whiteboard, skjerm (dekker N:M-kravet) | ❌ |
| **Participation** | N:1 Booking | Student som blir med i en booking, med en melding | ❌ |

**Domeneregler på serveren:**
- ✅ Ingen overlappende bookinger i samme rom.
- ✅ En booking kan vare maks 4 timer, innenfor åpningstid 08–22.
- ✅ Man kan ikke booke tilbake i tid eller mer enn 60 dager frem.
- ❌ Man kan ikke bli med i en booking når rommet er fullt (kapasitet).
- ❌ Samme student kan ikke bli med i samme booking to ganger.

---

## Roller

Skriv navn bak hver rolle.

### Medlem A: Data og domene (navn: ______)
- Equipment med N:M-relasjon mot Room, og Participation-modellen
- Migrasjoner og seeding for de nye entitetene
- Repositories for Equipment og Participation
- Domeneregler for kapasitet og dobbel påmelding
- 12+ enhetstester (xUnit + Moq), 6 positive og 6 negative
- **Dokumentasjon:** kap. 4 Database, testdelen av kap. 6

### Medlem B: MVC-grensesnitt (navn: ______)
- CRUD-sider for Room og Equipment
- Sider for å bli med i og forlate en booking (Participation)
- ViewModels og partial views (tabell, grid, kort)
- Søk og filtrering på dato, rom, emne og utstyr
- Conditional rendering, navigasjon, layout, farger og design
- **Dokumentasjon:** kap. 8 UX og brukervennlighet, kap. 5 for Subapp 1

### Medlem C: Sikkerhet i MVC (navn: ______)
- Identity med registrering og innlogging
- Rollene Admin (styrer rom og utstyr) og Student (booker og blir med)
- Autorisasjon: bare eieren eller en admin kan endre eller slette en booking
- Gjennomgang av feilhåndtering og logging i alle controllere
- Sikkerhetsgjennomgang: anti-forgery, overposting, XSS
- **Dokumentasjon:** sikkerhetsdelen av kap. 6, kap. 1 metadata og innloggingsinfo til sensor

### Medlem D: Web API (navn: ______)
- Opprette `SubApp2.Api` og gjenbruke modellene og DAL-en
- API-controllere med CRUD for Room, Booking og Participation
- DTOs, validering, feilhåndtering og logging
- Søk og filtrering via query-parametere
- JWT-autentisering og CORS mot React-appen
- **Dokumentasjon:** kap. 3 Arkitektur og ADR-er

### Medlem E: React-klient (navn: ______)
- Opprette `SubApp2.Client` (Vite + React + TypeScript)
- API-servicelag (egne `services/`-filer)
- Komponenter og sider for CRUD, søk og filtrering
- Conditional rendering, skjemaer med validering, feilmeldinger
- Innlogging med JWT på klienten, beskyttede ruter
- **Dokumentasjon:** kap. 2 Krav og traceability, kap. 5 for Subapp 2

**Alle** skriver kap. 7 (bruk av AI) og kap. 9 (konklusjon og egen refleksjon). Hver person noterer fortløpende hvordan de brukte AI og hvordan de sjekket resultatet.

---

## Uke for uke

> D og E kan starte med en gang. D bygger API-et på den eksisterende DAL-en. E bruker testdata til endepunktene er klare.

| Uke | A: Data | B: MVC-grensesnitt | C: Sikkerhet | D: API | E: React |
|---|---|---|---|---|---|
| **44** (26.10–01.11) | Merge DAL-grenen. Modeller for Equipment og Participation, migrasjon. | CRUD-sider for Room. Skisser av de andre sidene. | Identity, registrering og innlogging. | Opprette API-prosjektet. Endepunkter for Room og Booking. | Opprette React-prosjektet. Komponenter med testdata. |
| **45** (02.11–08.11) | Repositories og domeneregler for Participation. | CRUD for Equipment. Bli med i og forlate en booking. ViewModels. | Roller og autorisasjon på alle controllere. | DTOs, validering, logging. Participation-endepunkter. | Servicelag mot API-et. CRUD-sider for Room og Booking. |
| **46** (09.11–15.11) | 12+ enhetstester. | Søk og filtrering. Partial views. Design og farger. | Sikkerhetsgjennomgang. Testbrukere i seeding. | JWT og CORS. Søk via query-parametere. | Søk og filtrering. Participation-sider. |
| **47** (16.11–22.11) | Skrive kap. 4 og testdelen av kap. 6. | UX- og tilgjengelighetsgjennomgang. Skrive kap. 8. | Skrive sikkerhetsdelen av kap. 6. | Feilretting. Skrive kap. 3. | JWT-innlogging og beskyttede ruter. Skrive kap. 2. |
| **48** (23.11–29.11) | **Alle:** Ingen nye funksjoner. Rette advarsler fra `dotnet build` og `npm run build`. Testkjøre fra en ren klone på en annen maskin. Ferdigstille og korrekturlese dokumentasjonen. Lage zip uten `bin`, `obj`, `node_modules` og `dist`. Levere søndag 29.11. | | | | |

---

## Pensum per person

Modulene i Canvas, i den rekkefølgen de bør leses. Vi trenger ikke Angular-materialet fra 2023.

### Alle
- Reading: how to work as a group?
- Info: Using .gitignore to avoid unnecessary sync between group members push
- Reading: Introduction to GitHub
- FAQ: Examination & Mandatory Assignments
- FAQ: Can I use Generative AI for the Course?
- Lecture: How to work with AI
- ITPE3200 Project Documentation Template (2026H) + Detailed Explanations & Examples
- Lecture: Overview of MVC

### Medlem A: Data og domene
1. Demo: Expanded database with CRUD (relasjoner og N:M)
2. Demo: Database seeding
3. Demo: Repository pattern and data access layer (DAL)
4. Reading: Entity framework and DAL
5. Reading: Essential Strategies and Practices for Entity Framework Core
6. Summary Lecture: EntityFrameworkSummary
7. Demo: Unit testing
8. Reading: Software testing
9. Lecture: Security and Unit Testing (testdelen)

### Medlem B: MVC-grensesnitt
1. Demo: Shop MVC
2. Demo: HomeView, Navigation and Routing
3. Demo: Dynamic Content, Client-Side Files
4. Demo: Dynamic Views, View Model, Partial View
5. Summary Lecture: MVC
6. Reading: Reference Answers MVC
7. Demo: Input validation (klientsiden)
8. Lecture: Usability and Accessibility of Websites
9. Ekstra: Bootstrap på kunde-applikasjonen, Responsivt grid, Navigering Bootstrap

### Medlem C: Sikkerhet i MVC
1. Lecture: Logging, Error Handling, and Input Validation
2. Demo: Error handling and logging
3. Demo: Input validation (serversiden)
4. Demo: Authentication and authorisation
5. Lecture: Security and Unit Testing (sikkerhetsdelen)

### Medlem D: Web API
1. Demo: Repository pattern and data access layer (DAL)
2. Demo-React-2-APIBackendAndDebugging
3. Demo-React-6-HttpPost og Demo-React-7-HttpPutDelete (backend-delen)
4. Demo-React-9-JWTAuthentication-Backend
5. Lecture: Security and Unit Testing (sikkerhetsdelen)
6. Ekstra: Forskjell på GET og POST

### Medlem E: React-klient
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
| 3+ domene-entiteter med CRUD | MVC / SPA | B / E |
| Minst én N:M-relasjon | Begge | A |
| Kompleks domeneregel på serveren | Begge | A |
| To typer conditional rendering | MVC / SPA | B / E |
| Søk og filtrering på flere parametere | MVC / API / SPA | B / D / E |
| Repository og DAL, async/await | Begge | A, D |
| ViewModels | MVC | B |
| DTOs | API | D |
| API-servicelag på klienten | SPA | E |
| Validering på klient og server | Begge | B, C, D, E |
| Feilhåndtering og logging med ILogger | MVC / API | C / D |
| Autentisering og autorisasjon | MVC / API / SPA | C / D / E |
| 12+ enhetstester, 6 positive og 6 negative | MVC | A |
| Design og UX | MVC / SPA | B / E |
| Bygg uten advarsler | Begge | Alle |

---

## Arbeidsregler

- **Hent alltid først.** Kjør `git pull` (eller Fetch i GitHub Desktop) før du begynner å jobbe.
- **Én gren per oppgave.** Lag f.eks. `feature/room-crud` og merge til `main` via pull request. Push aldri direkte til main.
- **Én reviewer per PR.** En annen i gruppen ser over koden før merge.
- **Aldri `bin/`, `obj/` eller `.db`-filer.** Sjekk `git status` før hver commit.
- **Gode commit-meldinger.** Skriv hva du har gjort, f.eks. «Add Room CRUD views», ikke «.» eller «..».
- **Kort statusmøte to ganger i uken.** Hva er gjort, hva er neste, hva stopper oss.
- **Skriv mens vi koder.** Noter beslutninger (ADR-er) og bruk av AI med en gang.
- **Felles stil.** Engelske navn i koden og korte engelske kommentarer der logikken ikke er åpenbar.

---

## Kjøre prosjektet (Subapp 1)

```
dotnet tool restore
dotnet restore
dotnet run
```

Databasen (SQLite) lages og fylles med eksempeldata automatisk første gang appen starter.
