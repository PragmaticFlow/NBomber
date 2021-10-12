#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SimpleWebApp/SimpleWebApp.csproj", "SimpleWebApp/"]
RUN dotnet restore "SimpleWebApp/SimpleWebApp.csproj"
COPY . .
WORKDIR "/src/SimpleWebApp"
RUN dotnet build "SimpleWebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleWebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY build-version.txt .
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleWebApp.dll"]