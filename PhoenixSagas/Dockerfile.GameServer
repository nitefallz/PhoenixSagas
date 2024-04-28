# Use the .NET 7 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Add ARG for secondary NuGet source URL
ARG NUGET_SOURCE2=https://gomezdev.hopto.org:8090/NugetServer/nuget

# Add secondary NuGet source
RUN dotnet nuget add source "${NUGET_SOURCE2}" --name SecondSource

# Copy csproj and restore any dependencies (via NuGet)
COPY ["GameServer/PhoenixSagas.GameServer.csproj", "GameServer/"]
RUN dotnet restore "GameServer/PhoenixSagas.GameServer.csproj"

# Copy the project files and build our release
COPY GameServer/ ./GameServer/
RUN dotnet publish "GameServer/PhoenixSagas.GameServer.csproj" -c Release -o out

# Generate the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "PhoenixSagas.GameServer.dll"]
