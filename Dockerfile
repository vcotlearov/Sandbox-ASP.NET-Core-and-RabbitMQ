# ============================
# Base runtime image (shared)
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
# optional user, keep as in original
USER $APP_UID

# ============================
# SDK image for build
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files and restore
COPY ["Training.WebAPI/Training.WebAPI.csproj", "Training.WebAPI/"]
COPY ["Training.Console/Training.Console.csproj", "Training.Console/"]
COPY ["Training.RabbitMqConnector/Training.RabbitMqConnector.csproj", "Training.RabbitMqConnector/"]
RUN dotnet restore "./Training.WebAPI/Training.WebAPI.csproj"
RUN dotnet restore "./Training.Console/Training.Console.csproj"

# Copy all files
COPY . .

# ============================
# WebAPI build & publish
# ============================
FROM build AS webapi-build
WORKDIR "/src/Training.WebAPI"
RUN dotnet build "./Training.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "./Training.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ============================
# Console build & publish
# ============================
FROM build AS console-build
WORKDIR "/src/Training.Console"
RUN dotnet build "./Training.Console.csproj" -c $BUILD_CONFIGURATION -o /worker/build
RUN dotnet publish "./Training.Console.csproj" -c $BUILD_CONFIGURATION -o /worker/publish /p:UseAppHost=false

# ============================
# Final WebAPI image
# ============================
FROM base AS webapi-final
WORKDIR /app
COPY --from=webapi-build /app/publish .
ENTRYPOINT ["dotnet", "Training.WebAPI.dll"]

# ============================
# Final Console image
# ============================
FROM base AS console-final
WORKDIR /app
COPY --from=console-build /worker/publish .
ENTRYPOINT ["dotnet", "Training.Console.dll"]