# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj files and restore dependencies
COPY src/Domain/Motocross.Domain.csproj ./src/Domain/
COPY src/Application/Motocross.Application.csproj ./src/Application/
COPY src/Infrastructure/Motocross.Infrastructure.csproj ./src/Infrastructure/
COPY src/Api/Motocross.Api.csproj ./src/Api/
COPY Motocross.sln ./

RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR /app/src/Api
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published app
COPY --from=build /app/publish .

# Cloud Run uses PORT environment variable
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "Motocross.Api.dll"]
