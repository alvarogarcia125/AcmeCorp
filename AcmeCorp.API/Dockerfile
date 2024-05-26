#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AcmeCorp.API/AcmeCorp.API.csproj", "AcmeCorp.API/"]
RUN dotnet restore "AcmeCorp.API/AcmeCorp.API.csproj"
COPY . .
WORKDIR "/src/AcmeCorp.API"
RUN dotnet build "AcmeCorp.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AcmeCorp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AcmeCorp.API.dll"]