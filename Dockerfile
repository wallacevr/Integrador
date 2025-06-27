# Etapa 1: base de runtime com .NET 9
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS base
WORKDIR /app
EXPOSE 80

# Etapa 2: imagem de build com .NET 9 SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src

# Copia todos os arquivos
COPY . .

# Restaura pacotes NuGet
RUN dotnet restore ./HelpIn/HelpIn.csproj

# Publica o projeto
WORKDIR /src/HelpIn
RUN dotnet publish -c Release -o /app/publish

# Etapa final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HelpIn.dll"]

