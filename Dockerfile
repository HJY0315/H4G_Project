# Use the official .NET 6 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 10000

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["H4G_Project/H4G_Project.csproj", "H4G_Project/"]
RUN dotnet restore "H4G_Project/H4G_Project.csproj"
COPY . .
WORKDIR "/src/H4G_Project"
RUN dotnet build "H4G_Project.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "H4G_Project.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:10000
ENTRYPOINT ["dotnet", "H4G_Project.dll"]