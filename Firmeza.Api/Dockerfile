# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY ["Firmeza.Api/Firmeza.Api.csproj", "Firmeza.Api/"]
COPY ["Firmeza.Core/Firmeza.Core.csproj", "Firmeza.Core/"]
COPY ["Firmeza.Application/Firmeza.Application.csproj", "Firmeza.Application/"]
COPY ["Firmeza.Infrastructure/Firmeza.Infrastructure.csproj", "Firmeza.Infrastructure/"]

RUN dotnet restore "Firmeza.Api/Firmeza.Api.csproj"

# Copiar el resto de los archivos y compilar
COPY . .
WORKDIR "/src/Firmeza.Api"
RUN dotnet build "Firmeza.Api.csproj" -c Release -o /app/build

# Etapa de publicación
FROM build AS publish
RUN dotnet publish "Firmeza.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Firmeza.Api.dll"]
