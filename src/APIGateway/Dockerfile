#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Shared/Shared.csproj", "Shared/"]
COPY ["src/APIGateway/APIGateway.csproj", "API/"]

RUN dotnet restore "API/APIGateway.csproj"

COPY ["src/Shared/", "Shared/"]
COPY ["src/APIGateway", "API/"]
RUN dotnet publish "API/APIGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "APIGateway.dll"]