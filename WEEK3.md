# Woche 3 – Azure SQL + EF Core DbContext + Erste Migration

> Ziel: Die Datenbank-Schicht fertigstellen. Am Ende dieser Woche läuft die API
> mit einer echten Datenbank (lokal Docker, Cloud Azure SQL).

---

## Voraussetzungen

- [x] Azure-Account vorhanden (https://azure.microsoft.com/free)
- [ ] Azure CLI installiert (`az --version`)
- [ ] EF Core Tools installiert (`dotnet ef --version`)

### Azure CLI installieren (Ubuntu)

```bash
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
az --version
```

### EF Core Tools installieren

```bash
dotnet tool install --global dotnet-ef --version 8.0.13
dotnet ef --version
```

---

## Schritt 1: Azure-Account einrichten

1. Geh auf https://portal.azure.com und melde dich an
2. Suche oben nach **"Subscriptions"** → du solltest eine kostenlose Subscription sehen
3. Merke dir die **Subscription ID** (brauchst du gleich)

---

## Schritt 2: Azure SQL Datenbank anlegen

### Option A: Über das Azure Portal (empfohlen zum Lernen)

1. Im Portal oben suchen: **"SQL databases"** → **"Create"**
2. Fülle aus:
   - **Resource Group:** Neu erstellen → `stockpilot-rg`
   - **Database name:** `stockpilot-db`
   - **Server:** Neuen Server erstellen
     - Server name: `stockpilot-server` (muss global eindeutig sein, z.B. `stockpilot-server-DEINNAME`)
     - Location: `West Europe`
     - Authentication: **SQL Authentication**
     - Admin login: `stockpilot-admin`
     - Password: Sicheres Passwort (notieren!)
   - **Compute + storage:** **"Configure database"** → **"Basic"** (5 DTU, ~5€/Monat oder kostenlos im Free Tier)
3. **"Review + create"** → **"Create"**

### Option B: Über Azure CLI (zum Lernen empfohlen)

```bash
az login

az group create \
  --name stockpilot-rg \
  --location westeurope

az sql server create \
  --name stockpilot-server-DEINNAME \
  --resource-group stockpilot-rg \
  --location westeurope \
  --admin-user stockpilot-admin \
  --admin-password "DeinSicheresPasswort123!"

az sql db create \
  --resource-group stockpilot-rg \
  --server stockpilot-server-DEINNAME \
  --name stockpilot-db \
  --service-objective Basic
```

---

## Schritt 3: Firewall-Regel für deine IP setzen

Damit du von deinem Rechner auf die Azure SQL DB zugreifen kannst:

### Im Portal:
1. Geh zur SQL Server Ressource → **"Networking"**
2. **"Add your client IPv4 address"** → **"Save"**

### Per CLI:
```bash
az sql server firewall-rule create \
  --resource-group stockpilot-rg \
  --server stockpilot-server-DEINNAME \
  --name AllowMyIP \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0
```

*(0.0.0.0 erlaubt alle Azure-Services – für Produktion einschränken!)*

---

## Schritt 4: Connection String ermitteln

Im Portal: SQL Database → **"Connection strings"** → ADO.NET

Sieht so aus:
```
Server=tcp:stockpilot-server-DEINNAME.database.windows.net,1433;Initial Catalog=stockpilot-db;Persist Security Info=False;User ID=stockpilot-admin;Password=DeinPasswort;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

Trage diesen String in `src/StockPilot.API/appsettings.Development.json` ein:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:stockpilot-server-DEINNAME.database.windows.net,1433;..."
  }
}
```

**Niemals in appsettings.json (die ins Git kommt)!**

---

## Schritt 5: EF Core DbContext in Infrastructure implementieren

Cascade führt dich durch die Code-Änderungen wenn du bereit bist.

Folgende Dateien werden erstellt:
- `src/StockPilot.Infrastructure/Persistence/ApplicationDbContext.cs`
- `src/StockPilot.Infrastructure/Persistence/Configurations/ProductConfiguration.cs`
- `src/StockPilot.Infrastructure/Persistence/Configurations/WarehouseConfiguration.cs`
- `src/StockPilot.Infrastructure/Persistence/Configurations/StockMovementConfiguration.cs`
- `src/StockPilot.Infrastructure/DependencyInjection.cs`

---

## Schritt 6: Erste EF Migration erstellen

```bash
cd /home/pingi/RiderProjects/StockPilot

dotnet ef migrations add InitialCreate \
  --project src/StockPilot.Infrastructure \
  --startup-project src/StockPilot.API \
  --output-dir Persistence/Migrations
```

---

## Schritt 7: Migration auf Datenbank anwenden

### Lokal (Docker):
```bash
docker compose up db -d

dotnet ef database update \
  --project src/StockPilot.Infrastructure \
  --startup-project src/StockPilot.API
```

### Azure SQL:
Connection String in `appsettings.Development.json` auf Azure umstellen, dann:
```bash
dotnet ef database update \
  --project src/StockPilot.Infrastructure \
  --startup-project src/StockPilot.API
```

---

## Schritt 8: Verifizieren

```bash
dotnet build
dotnet run --project src/StockPilot.API
```

Im Browser `http://localhost:5000/swagger` öffnen – die API läuft, aber die Endpoints kommen erst in Woche 4.

Im Azure Portal → SQL Database → **"Query editor"** → Tabellen prüfen:
```sql
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'
```

Du solltest sehen: `Products`, `Warehouses`, `StockMovements`

---

## Was du in Woche 3 lernst

| Konzept | Erklärung |
|---|---|
| **Resource Group** | Container für alle Azure-Ressourcen eines Projekts – einfach alles auf einmal löschen |
| **Azure SQL Server vs. Database** | Server ist der Hosting-Container, Database ist die eigentliche DB |
| **Firewall Rules** | Azure blockiert standardmäßig alle Verbindungen von außen |
| **EF Migrations** | Versionskontrolle für dein Datenbankschema – kennst du vom Prinzip |
| **IEntityTypeConfiguration** | EF-Konfiguration sauber ausgelagert statt Data Annotations |
| **appsettings.Development.json** | Lokale Overrides die nicht ins Git kommen – Secrets sicher halten |

---

## Kosten-Hinweis

- **Azure SQL Basic Tier:** ~5€/Monat
- **Free Trial:** Neuer Account bekommt 200$ Guthaben (30 Tage)
- **Kostenkontrolle:** Im Portal → "Cost Management" → Budget-Alert setzen (z.B. 10€/Monat)
- **Wenn nicht mehr gebraucht:** Ressource Group löschen löscht alles auf einmal

---

*Zuletzt aktualisiert: 03.03.2026*
*Voraussetzung: Woche 2 abgeschlossen ✅*
