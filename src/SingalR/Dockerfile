#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Shared/Shared.csproj", "Shared/"]
COPY ["/src/SingalR/SingalR.API.csproj", "API/"]

RUN dotnet restore "API/SingalR.API.csproj"

COPY ["src/Shared/", "Shared/"]
COPY ["/src/SingalR/", "API/"]
RUN dotnet publish "API/SingalR.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SingalR.API.dll"]