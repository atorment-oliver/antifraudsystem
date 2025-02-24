# Usar la imagen base de .NET 6 SDK para compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copiar el archivo del proyecto y restaurar las dependencias
COPY ["/AntiFraudSystem.csproj", "AntiFraudSystem/"]
WORKDIR "/src/AntiFraudSystem"
RUN dotnet restore "AntiFraudSystem.csproj"

# Copiar todo el código fuente
WORKDIR "/src"
COPY . .

# Compilar la aplicación
WORKDIR "/src/AntiFraudSystem"
RUN dotnet build "AntiFraudSystem.csproj" -c Release -o /app/build

# Publicar la aplicación
RUN dotnet publish "AntiFraudSystem.csproj" -c Release -o /app/publish

# Usar la imagen base de .NET 6 Runtime para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# Copiar los archivos publicados desde la etapa de compilación
COPY --from=build /app/publish .

# Exponer el puerto en el que la aplicación escucha
EXPOSE 80

# Definir el comando para ejecutar la aplicación
ENTRYPOINT ["dotnet", "AntiFraudSystem.dll"]