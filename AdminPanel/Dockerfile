FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AdminPanel.csproj", "./"]
RUN dotnet restore "AdminPanel.csproj"
COPY . .
RUN dotnet build "AdminPanel.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AdminPanel.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app
COPY . .
ENTRYPOINT ["dotnet", "watch", "run", "--project", "AdminPanel.csproj"]