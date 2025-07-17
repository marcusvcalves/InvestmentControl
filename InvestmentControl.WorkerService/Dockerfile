FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Packages.props", "."]
COPY ["InvestmentControl.sln", "."]

COPY InvestmentControl.WorkerService/. ./InvestmentControl.WorkerService/
COPY InvestmentControl.Domain/. ./InvestmentControl.Domain/
COPY InvestmentControl.Infrastructure/. ./InvestmentControl.Infrastructure/

WORKDIR "/src/InvestmentControl.WorkerService"

RUN dotnet restore "./InvestmentControl.WorkerService.csproj"

RUN dotnet build "./InvestmentControl.WorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./InvestmentControl.WorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InvestmentControl.WorkerService.dll"]