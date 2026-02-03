# RUN FROM ROOT
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln .
COPY . .
WORKDIR /app/src/Web
RUN dotnet restore

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/src/Web/out ./

# Create data directory for SQLite
RUN mkdir -p /data && chmod 777 /data

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV ConnectionStrings__CatalogConnection="Data Source=/data/catalog.db"
ENV ConnectionStrings__IdentityConnection="Data Source=/data/identity.db"

EXPOSE 8080

ENTRYPOINT ["dotnet", "Web.dll"]
