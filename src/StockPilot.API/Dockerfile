FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/StockPilot.API/StockPilot.API.csproj", "src/StockPilot.API/"]
COPY ["src/StockPilot.Application/StockPilot.Application.csproj", "src/StockPilot.Application/"]
COPY ["src/StockPilot.Domain/StockPilot.Domain.csproj", "src/StockPilot.Domain/"]
COPY ["src/StockPilot.Infrastructure/StockPilot.Infrastructure.csproj", "src/StockPilot.Infrastructure/"]
RUN dotnet restore "src/StockPilot.API/StockPilot.API.csproj"
COPY . .
RUN dotnet build "src/StockPilot.API/StockPilot.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/StockPilot.API/StockPilot.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StockPilot.API.dll"]
