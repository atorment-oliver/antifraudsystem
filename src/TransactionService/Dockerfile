FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["TransactionService/TransactionService.csproj", "TransactionService/"]
COPY ["ControlKernel/ControlKernel.csproj", "ControlKernel/"]
RUN dotnet restore "TransactionService/TransactionService.csproj"

COPY . .
WORKDIR "/src/TransactionService"
RUN dotnet build "TransactionService.csproj" -c Release -o /app/build

RUN dotnet publish "TransactionService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "TransactionService.dll"]