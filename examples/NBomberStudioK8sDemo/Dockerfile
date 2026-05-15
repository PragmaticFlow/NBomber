FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["examples/NBomberStudioK8sDemo/NBomberStudioK8sDemo.csproj", "examples/NBomberStudioK8sDemo/"]
RUN dotnet restore "examples/NBomberStudioK8sDemo/NBomberStudioK8sDemo.csproj"
COPY . .
WORKDIR "/src/examples/NBomberStudioK8sDemo"
RUN dotnet build "./NBomberStudioK8sDemo.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NBomberStudioK8sDemo.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NBomberStudioK8sDemo.dll"]
