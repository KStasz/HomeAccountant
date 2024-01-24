FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./Frontend/HomeAccountant/HomeAccountant.csproj", "Frontend/HomeAccountant/"]
COPY ["./Frontend/HomeAccountant.Core/HomeAccountant.Core.csproj", "Frontend/HomeAccountant.Core/"]
RUN dotnet restore "Frontend/HomeAccountant/HomeAccountant.csproj"
COPY . .
WORKDIR "/src/Frontend/HomeAccountant"
RUN dotnet clean "./HomeAccountant.csproj"
RUN dotnet build "./HomeAccountant.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./HomeAccountant.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /app
EXPOSE 8081
ARG Environment
COPY ["./Frontend/HomeAccountant/nginx.conf", "/etc/nginx/nginx.conf"]
RUN sed -i "s/replaceme/${Environment}/" /etc/nginx/nginx.conf
COPY --from=publish /app/publish/wwwroot /usr/share/nginx/html