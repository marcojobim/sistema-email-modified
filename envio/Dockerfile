# BUILD
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["shared/Gerenciamento.Shared.csproj", "shared/"]
COPY ["envio/EnvioWorker.csproj", "envio/"]

RUN dotnet restore "envio/EnvioWorker.csproj"

COPY . .

RUN dotnet publish "envio/EnvioWorker.csproj" -c Release -o /app/publish

# RUN
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EnvioWorker.dll"]