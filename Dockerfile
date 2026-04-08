FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["VinhKhanhTour.CMS/VinhKhanhTour.CMS.csproj", "VinhKhanhTour.CMS/"]
COPY ["VinhKhanhTour.Shared/VinhKhanhTour.Shared.csproj", "VinhKhanhTour.Shared/"]
RUN dotnet restore "./VinhKhanhTour.CMS/VinhKhanhTour.CMS.csproj"
COPY . .
WORKDIR "/src/VinhKhanhTour.CMS"
RUN dotnet build "./VinhKhanhTour.CMS.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./VinhKhanhTour.CMS.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VinhKhanhTour.CMS.dll"]
