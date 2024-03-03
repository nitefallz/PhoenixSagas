# Use the .NET 7 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Add ARG for NuGet source URL
ARG NUGET_SOURCE

# Copy csproj and restore any dependencies (via NuGet)
COPY ["TCPServer/PhoenixSagas.TCPServer.csproj", "TCPServer/"]
RUN dotnet nuget add source "${NUGET_SOURCE}" --name CustomSource && \
    dotnet restore "TCPServer/PhoenixSagas.TCPServer.csproj"

# Copy the project files and build our release
COPY . ./
RUN dotnet publish "TCPServer/PhoenixSagas.TCPServer.csproj" -c Release -o out

# Generate the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "PhoenixSagas.TCPServer.dll"]
