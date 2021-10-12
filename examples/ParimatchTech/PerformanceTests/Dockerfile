#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["PerformanceTests/PerformanceTests.csproj", "PerformanceTests/"]
RUN dotnet restore "PerformanceTests/PerformanceTests.csproj"
COPY . .
WORKDIR "/src/PerformanceTests"
RUN dotnet build "PerformanceTests.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PerformanceTests.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PerformanceTests.dll"]