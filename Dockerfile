# Imagem base de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Imagem para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia todos os arquivos do repositório
COPY . .

# Entra na pasta onde está o .csproj e publica o projeto
WORKDIR /src/HelpIn
RUN dotnet publish -c Release -o /app/publish

# Imagem final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HelpIn.dll"]
