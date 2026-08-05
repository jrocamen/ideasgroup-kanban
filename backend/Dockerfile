# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project files and restore dependencies
COPY IdeasGroupKanban.API/*.csproj ./IdeasGroupKanban.API/
COPY IdeasGroupKanban.Application/*.csproj ./IdeasGroupKanban.Application/
COPY IdeasGroupKanban.Domain/*.csproj ./IdeasGroupKanban.Domain/
COPY IdeasGroupKanban.Infrastructure/*.csproj ./IdeasGroupKanban.Infrastructure/
RUN dotnet restore ./IdeasGroupKanban.API/IdeasGroupKanban.API.csproj

# Copy the remaining source code and build
COPY . .
WORKDIR /app/IdeasGroupKanban.API
RUN dotnet publish -c Release -o /app/out

# Use the ASP.NET runtime image for the final container (Ubuntu Jammy to avoid Debian mirror issues)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS runtime
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "IdeasGroupKanban.API.dll"]
