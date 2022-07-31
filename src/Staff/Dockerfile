#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["src/Shared/Shared.csproj", "Shared/"]
COPY ["src/Staff/Core/Staff.Core.csproj", "Staff/Core/"]
COPY ["src/Staff/Data/Staff.Data.csproj", "Staff/Data/"]
COPY ["src/Staff/API/Staff.API.csproj", "Staff/API/"]

RUN dotnet restore "Staff/API/Staff.API.csproj"

COPY ["src/Shared/", "Shared/"]
COPY ["src/Staff/Core", "Staff/Core/"]
COPY ["src/Staff/Data", "Staff/Data/"]
COPY ["src/Staff/API", "Staff/API/"]
RUN dotnet publish "Staff/API/Staff.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Staff.API.dll"]