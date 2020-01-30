#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1.1-buster-slim-arm32v7 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1.101-buster AS build
WORKDIR /src
COPY ["AgentV/AgentV.csproj", "AgentV/"]
RUN dotnet restore "AgentV/AgentV.csproj"
COPY . .
WORKDIR "/src/AgentV"
RUN dotnet build "AgentV.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AgentV.csproj" -c Release -o /app/publish -r linux-arm

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AgentV.dll"]