# Estágio de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["./src/Fcg.Games.Purchasing.Worker/Fcg.Games.Purchasing.Worker.csproj", "src/Fcg.Games.Purchasing.Worker/"]
COPY ["./src/Fcg.Games.Purchase.Domain/Fcg.Games.Purchase.Domain.csproj", "src/Fcg.Games.Purchase.Domain/"]
COPY ["./src/Fcg.Games.Purchase.Infra/Fcg.Games.Purchase.Infra.csproj", "src/Fcg.Games.Purchase.Infra/"]

# Restaurar dependencias
RUN dotnet restore "./src/Fcg.Games.Purchasing.Worker/Fcg.Games.Purchasing.Worker.csproj"

# Copiar tudo
COPY . .

# Build
WORKDIR "/src/src/Fcg.Games.Purchasing.Worker"
RUN dotnet build "Fcg.Games.Purchasing.Worker.csproj" -c Release -o /app/build

# Estágio de publicacao
FROM build AS publish
RUN dotnet publish "Fcg.Games.Purchasing.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fcg.Games.Purchasing.Worker.dll"]