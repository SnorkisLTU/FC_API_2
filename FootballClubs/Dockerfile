FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["FootballClubs.csproj", ""]
RUN dotnet restore "FootballClubs.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "FootballClubs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FootballClubs.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FootballClubs.dll"]
