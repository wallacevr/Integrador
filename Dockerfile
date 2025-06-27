# Etapa 1: base de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Etapa 2: imagem de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia todos os arquivos
COPY . .

# Restaura pacotes NuGet (melhor performance em cache de build)
RUN dotnet restore ./HelpIn/HelpIn.csproj

# Publica o projeto em modo Release
WORKDIR /src/HelpIn
RUN dotnet publish -c Release -o /app/publish

# Etapa 3: imagem final
FROM base AS final
WORKDIR /app

# Copia os arquivos publicados da etapa anterior
COPY --from=build /app/publish .

# Comando para iniciar o app
ENTRYPOINT ["dotnet", "HelpIn.dll"]
