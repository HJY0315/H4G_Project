# Use the official .NET 6 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY H4G_Project/H4G_Project.csproj H4G_Project/
RUN dotnet restore H4G_Project/H4G_Project.csproj

# Copy everything else and build
COPY . .
WORKDIR /src/H4G_Project
RUN dotnet build H4G_Project.csproj -c Release -o /app/build
RUN dotnet publish H4G_Project.csproj -c Release -o /app/publish

# Use runtime image for final stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 10000

ENTRYPOINT ["dotnet", "H4G_Project.dll"]