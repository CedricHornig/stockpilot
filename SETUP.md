# StockPilot – Setup Guide

> Ziel dieser Datei: Dich von Null bis zum ersten lauffähigen Feature führen.
> Hak jeden Schritt ab wenn er erledigt ist.

---

## Voraussetzungen

- [x] .NET 8 SDK installiert (`dotnet --version` → `8.x.x`)
- [x] Docker installiert und gestartet (`docker --version`)
- [x] Rider oder VS Code installiert
- [x] Git installiert (`git --version`)
- [x] GitHub-Account vorhanden

---

## Schritt 1: Repository auf GitHub anlegen

1. Geh auf https://github.com → **New Repository**
2. Name: `stockpilot`
3. Visibility: **Public**
4. **Kein** README, **kein** .gitignore (wir haben beides schon lokal)
5. Repository erstellen → die angezeigte URL kopieren (z.B. `https://github.com/DEINNAME/stockpilot.git`)

---

## Schritt 2: Lokales Projekt mit GitHub verbinden

Im Terminal, aus dem Ordner `/home/pingi/RiderProjects/StockPilot`:

```bash
git init
git add .
git commit -m "Initial project setup: Clean Architecture structure"
git branch -M main
git remote add origin https://github.com/DEINNAME/stockpilot.git
git push -u origin main
```

Ersetze `DEINNAME` mit deinem GitHub-Benutzernamen.

---

## Schritt 3: Projektstruktur verstehen

```
stockpilot/
├── src/
│   ├── StockPilot.Domain/          ← Entities, Value Objects (kein EF, keine externen Abhängigkeiten)
│   ├── StockPilot.Application/     ← CQRS Commands/Queries, MediatR Handlers, Interfaces
│   ├── StockPilot.Infrastructure/  ← EF Core, Azure Services, externe Implementierungen
│   └── StockPilot.API/             ← Controller, Endpoints, DI-Registrierung
├── tests/
│   └── StockPilot.Tests/           ← Unit Tests
├── docker-compose.yml              ← Lokale DB (SQL Server) + API
├── .gitignore
└── SETUP.md                        ← Diese Datei
```

### Abhängigkeitsregel (wichtig!)

```
API → Infrastructure → Application → Domain
```

- **Domain** kennt niemanden (keine NuGet-Pakete außer .NET selbst)
- **Application** kennt nur Domain
- **Infrastructure** implementiert die Interfaces aus Application
- **API** verdrahtet alles zusammen (Dependency Injection)

Das entspricht dem Prinzip das du aus deiner Arbeit kennst – nur strukturierter.

---

## Schritt 4: SQL Server lokal starten (Docker)

```bash
docker compose up db -d
```

Das startet einen SQL Server Container auf Port **1433**.

- **Username:** `sa`
- **Passwort:** `StockPilot_Dev123!`
- **Datenbank:** wird beim ersten EF-Migration-Apply angelegt

Prüfen ob er läuft:

```bash
docker ps
```

Du solltest `stockpilot-db` in der Liste sehen.

---

## Schritt 5: Connection String für lokale Entwicklung

Öffne `src/StockPilot.API/appsettings.json` und ergänze:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=StockPilot;User Id=sa;Password=StockPilot_Dev123!;TrustServerCertificate=True"
  }
}
```

**Wichtig:** Die `appsettings.Development.json` steht in der `.gitignore` – dort kannst du lokale Secrets sicher ablegen ohne sie versehentlich zu pushen.

---

## Schritt 6: Projekt builden und testen

```bash
dotnet build
dotnet test
```

Beide Befehle sollten ohne Fehler durchlaufen. Der Test-Output zeigt "0 tests passed" – das ist okay, wir haben noch keine Tests geschrieben.

---

## Schritt 7: API lokal starten

```bash
dotnet run --project src/StockPilot.API
```

Öffne im Browser: `http://localhost:5000/swagger`

Du siehst die Swagger UI mit dem Default-Endpoint `GET /weatherforecast` – das ist der Platzhalter den .NET automatisch erstellt. Den löschen wir in Woche 2.

---

## Was jetzt kommt: Woche 2

Sobald alles oben läuft, geht es mit dem Domain-Model weiter.

Du wirst erstellen:
- `Product` Entity (Artikel mit SKU, Name, Kategorie, Bestand)
- `Warehouse` Entity (Lager/Mandant)
- `StockMovement` Entity (Warenbewegung: Eingang/Ausgang)
- Dazu den ersten CQRS Command: `CreateProductCommand` + `CreateProductCommandHandler`

**Nächste Datei zum Lesen:** Sobald Woche 2 startet, wird hier eine `WEEK2.md` verlinkt.

---

## Installierte NuGet-Pakete (Übersicht)

| Projekt | Paket | Zweck |
|---|---|---|
| Application | `MediatR 12.4.1` | CQRS Dispatcher |
| Application | `FluentValidation 11.11.0` | Input-Validierung |
| Infrastructure | `Microsoft.EntityFrameworkCore.SqlServer 8.0.13` | Datenbankzugriff |
| Infrastructure | `Microsoft.EntityFrameworkCore.Design 8.0.13` | EF Migrations |
| API | `MediatR 12.4.1` | Commands/Queries aus Controllern dispatchen |
| API | `Microsoft.EntityFrameworkCore.Design 8.0.13` | Migration-Tool-Support |

---

## Häufige Probleme & Lösungen

**Docker startet nicht:**
```bash
sudo systemctl start docker
```

**Port 1433 bereits belegt:**
```bash
sudo lsof -i :1433
```
→ Prozess beenden oder in `docker-compose.yml` den Port ändern (z.B. `"1434:1433"`)

**Build-Fehler nach `dotnet build`:**
```bash
dotnet restore
dotnet build
```

**EF Migration funktioniert nicht:**
Stelle sicher dass du den Befehl aus dem Root-Verzeichnis ausführst und `--project` sowie `--startup-project` angibst (kommt in Woche 3).

---

## Glossar (Begriffe die in diesem Projekt vorkommen)

| Begriff | Erklärung |
|---|---|
| **Clean Architecture** | Schichtenarchitektur wo Domain im Zentrum steht und nichts nach außen zeigt |
| **CQRS** | Command Query Responsibility Segregation – Schreib- und Lesevorgänge getrennt |
| **MediatR** | NuGet-Paket das Commands/Queries an ihre Handler weiterleitet |
| **EF Core** | Entity Framework Core – ORM für Datenbankzugriff (kennst du bereits) |
| **Multi-Tenant** | Mehrere Firmen/Kunden nutzen dieselbe App-Instanz, aber sehen nur ihre Daten |
| **Docker Compose** | Tool um mehrere Container (API + DB) gemeinsam zu starten |
| **Azure App Service** | Azure-Hosting für Web-Apps, kein Server-Management nötig |

---

*Zuletzt aktualisiert: 03.03.2026*
