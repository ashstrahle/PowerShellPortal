#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PowerShellPortal/PowerShellPortal.csproj", "PowerShellPortal/"]
RUN dotnet restore "PowerShellPortal/PowerShellPortal.csproj"
COPY . .
WORKDIR "/src/PowerShellPortal"
RUN dotnet build "PowerShellPortal.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PowerShellPortal.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PowerShellPortal.dll"]
