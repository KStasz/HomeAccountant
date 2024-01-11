#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./Frontend/HomeAccountant/HomeAccountant.csproj", "Frontend/HomeAccountant/"]
COPY ["Frontend/HomeAccountant.Core/HomeAccountant.Core.csproj", "Frontend/HomeAccountant.Core/"]
RUN dotnet restore "Frontend/HomeAccountant/HomeAccountant.csproj"
COPY . .
WORKDIR "/src/Frontend/HomeAccountant"
RUN dotnet build "./HomeAccountant.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HomeAccountant.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf