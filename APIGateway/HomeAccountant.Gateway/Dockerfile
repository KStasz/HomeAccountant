#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["APIGateway/HomeAccountant.Gateway/HomeAccountant.Gateway.csproj", "APIGateway/HomeAccountant.Gateway/"]
RUN dotnet restore "APIGateway/HomeAccountant.Gateway/HomeAccountant.Gateway.csproj"
COPY . .
WORKDIR "/src/APIGateway/HomeAccountant.Gateway"
RUN dotnet clean "HomeAccountant.Gateway.csproj"
RUN dotnet build "HomeAccountant.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HomeAccountant.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HomeAccountant.Gateway.dll"]