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

# Install the agent
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
&& echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
&& wget https://download.newrelic.com/548C16BF.gpg \
&& apt-key add 548C16BF.gpg \
&& apt-get update \
&& apt-get install -y 'newrelic-dotnet-agent' \
&& rm -rf /var/lib/apt/lists/*

# Enable the agent
ENV CORECLR_ENABLE_PROFILING=1 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so \
NEW_RELIC_LICENSE_KEY=55411abd809a3157b0d87e24a824291fFFFFNRAL \
NEW_RELIC_APP_NAME="fcg-newrelic"