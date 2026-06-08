# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["TechMove/TechMove.csproj", "TechMove/"]
RUN dotnet restore "TechMove/TechMove.csproj"

COPY . .
WORKDIR "/src/TechMove"
RUN dotnet publish "TechMove.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "TechMove.dll"]