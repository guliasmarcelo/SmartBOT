# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia os arquivos de projeto e restaura as dependências
COPY *.csproj ./
RUN dotnet restore

# Copia o restante do código e publica a aplicação
COPY . ./
RUN dotnet publish -c Release -o /publish

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /publish .

# Define a variável de ambiente ASPNETCORE_ENVIRONMENT
ENV ASPNETCORE_ENVIRONMENT=Development

# Expõe a porta definida no launchSettings.json
EXPOSE 5053

# Define o comando de entrada
ENTRYPOINT ["dotnet", "SmartBOT.WebAPI.dll"]
