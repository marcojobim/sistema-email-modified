# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["shared/Gerenciamento.Shared.csproj", "shared/"]
COPY ["gerenciamento/GerenciamentoApi/GerenciamentoApi.csproj", "gerenciamento/GerenciamentoApi/"]
COPY ["healthcheck-client/HealthCheckClient.csproj", "healthcheck-client/"]

RUN dotnet restore "gerenciamento/GerenciamentoApi/GerenciamentoApi.csproj"

COPY . .

RUN dotnet publish "gerenciamento/GerenciamentoApi/GerenciamentoApi.csproj" -c Release -o /app/publish_api

RUN dotnet publish "healthcheck-client/HealthCheckClient.csproj" -c Release -o /app/publish_healthcheck

# Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish_api .
COPY --from=build /app/publish_healthcheck .
RUN chmod +x ./HealthCheckClient
EXPOSE 8080
ENTRYPOINT ["dotnet", "GerenciamentoApi.dll"]