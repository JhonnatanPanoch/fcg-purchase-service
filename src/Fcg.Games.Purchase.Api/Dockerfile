# Estágio de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["./src/Fcg.Games.Purchase.Api/Fcg.Games.Purchase.Api.csproj", "src/Fcg.Games.Purchase.Api/"]
COPY ["./src/Fcg.Games.Purchase.Application/Fcg.Games.Purchase.Application.csproj", "src/Fcg.Games.Purchase.Application/"]
COPY ["./src/Fcg.Games.Purchase.Domain/Fcg.Games.Purchase.Domain.csproj", "src/Fcg.Games.Purchase.Domain/"]
COPY ["./src/Fcg.Games.Purchase.Infra/Fcg.Games.Purchase.Infra.csproj", "src/Fcg.Games.Purchase.Infra/"]

# Restaurar dependências
RUN dotnet restore "./src/Fcg.Games.Purchase.Api/Fcg.Games.Purchase.Api.csproj"

# Copiar tudo
COPY . .

# Build
WORKDIR "/src/src/Fcg.Games.Purchase.Api"
RUN dotnet build "Fcg.Games.Purchase.Api.csproj" -c Release -o /app/build

# Estágio de publicação
FROM build AS publish
RUN dotnet publish "Fcg.Games.Purchase.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fcg.Games.Purchase.Api.dll"]