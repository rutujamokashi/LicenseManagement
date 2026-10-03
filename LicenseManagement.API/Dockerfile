FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["LicenseManagement.API.csproj", "."]

RUN dotnet restore "LicenseManagement.API.csproj"

COPY . .

RUN dotnet publish "LicenseManagement.API.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Docker

ENTRYPOINT ["dotnet", "LicenseManagement.API.dll"]