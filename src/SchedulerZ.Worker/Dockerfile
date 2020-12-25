#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/SchedulerZ.Worker/SchedulerZ.Worker.csproj", "src/SchedulerZ.Worker/"]
COPY ["src/Component/SchedulerZ.Store.MySQL/SchedulerZ.Store.MySQL.csproj", "src/Component/SchedulerZ.Store.MySQL/"]
COPY ["src/SchedulerZ/SchedulerZ.csproj", "src/SchedulerZ/"]
COPY ["src/Component/SchedulerZ.Scheduler.QuartzNet/SchedulerZ.Scheduler.QuartzNet.csproj", "src/Component/SchedulerZ.Scheduler.QuartzNet/"]
COPY ["src/Component/SchedulerZ.Route.Consul/SchedulerZ.Route.Consul.csproj", "src/Component/SchedulerZ.Route.Consul/"]
COPY ["src/Component/SchedulerZ.Remoting.gRPC/SchedulerZ.Remoting.gRPC.csproj", "src/Component/SchedulerZ.Remoting.gRPC/"]
RUN dotnet restore "src/SchedulerZ.Worker/SchedulerZ.Worker.csproj"
COPY . .
WORKDIR "/src/src/SchedulerZ.Worker"
RUN dotnet build "SchedulerZ.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SchedulerZ.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SchedulerZ.Worker.dll"]