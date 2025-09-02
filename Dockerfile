# base package
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# dotnet sdk
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# copy project files and restore
COPY ["Training.WebAPI/Training.WebAPI.csproj", "Training.WebAPI/"]
COPY ["Training.RabbitMqConnector/Training.RabbitMqConnector.csproj", "Training.RabbitMqConnector/"]
RUN dotnet restore "./Training.WebAPI/Training.WebAPI.csproj"

# copy everything and BUILD
COPY . .
WORKDIR "/src/Training.WebAPI"
RUN dotnet build "./Training.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# publish time
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Training.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Training.WebAPI.dll"]