FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["AntiFraudService/AntiFraudService.csproj", "AntiFraudService/"]
COPY ["ControlKernel/ControlKernel.csproj", "ControlKernel/"]

RUN dotnet restore "AntiFraudService/AntiFraudService.csproj"
RUN dotnet restore "ControlKernel/ControlKernel.csproj"

COPY . .

WORKDIR "/src/AntiFraudService"
RUN dotnet build "AntiFraudService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AntiFraudService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AntiFraudService.dll"]