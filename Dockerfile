# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only the .csproj files first so the restore layer is cached
# and skipped on later builds unless a project file changes.
COPY ShiftLess/*.csproj ShiftLess/
COPY ShiftLess.Application/*.csproj ShiftLess.Application/
COPY ShiftLess.Domain/*.csproj ShiftLess.Domain/
COPY ShiftLess.Infrastructure/*.csproj ShiftLess.Infrastructure/
COPY ShiftLess.Persistence/*.csproj ShiftLess.Persistence/

RUN dotnet restore ShiftLess/*.csproj

COPY . .
WORKDIR /src/ShiftLess
RUN dotnet publish -c Release -o /app/publish -p:UseAppHost=false

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

USER app

ENTRYPOINT ["dotnet", "ShiftLessAPI.dll"]