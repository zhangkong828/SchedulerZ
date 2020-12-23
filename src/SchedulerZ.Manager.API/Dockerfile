#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 11000
ENV ASPNETCORE_URLS http://+:11000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/SchedulerZ.Manager.API/SchedulerZ.Manager.API.csproj", "src/SchedulerZ.Manager.API/"]
COPY ["src/Component/SchedulerZ.Caching.Redis/SchedulerZ.Caching.Redis.csproj", "src/Component/SchedulerZ.Caching.Redis/"]
COPY ["src/SchedulerZ/SchedulerZ.csproj", "src/SchedulerZ/"]
COPY ["src/Component/SchedulerZ.Store.MySQL/SchedulerZ.Store.MySQL.csproj", "src/Component/SchedulerZ.Store.MySQL/"]
COPY ["src/Component/SchedulerZ.Remoting.gRPC.Client/SchedulerZ.Remoting.gRPC.Client.csproj", "src/Component/SchedulerZ.Remoting.gRPC.Client/"]
COPY ["src/Component/SchedulerZ.Route.Consul/SchedulerZ.Route.Consul.csproj", "src/Component/SchedulerZ.Route.Consul/"]
RUN dotnet restore "src/SchedulerZ.Manager.API/SchedulerZ.Manager.API.csproj"
COPY . .
WORKDIR "/src/src/SchedulerZ.Manager.API"
RUN dotnet build "SchedulerZ.Manager.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SchedulerZ.Manager.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["dotnet", "SchedulerZ.Manager.API.dll"]
