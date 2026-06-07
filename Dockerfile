# =========================
# Stage 1 - Build da aplicação
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder

# Define o diretório de trabalho para o build
WORKDIR /source

# Copia o arquivo de projeto e restaura as dependências (otimização de cache do Docker)
COPY *.csproj .
RUN dotnet restore

# Copia o restante do código-fonte
COPY . .

# Compila e publica a aplicação em modo Release
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# =========================
# Stage 2 - Runtime enxuto
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime

# Define o diretório de trabalho da aplicação
WORKDIR /app

# Copia os binários compilados do estágio anterior
COPY --from=builder /app/publish .

# Define pelo menos uma variável de ambiente conforme exigido
ENV ASPNETCORE_ENVIRONMENT=Production
# Define a porta em que a aplicação vai escutar
ENV ASPNETCORE_URLS=http://+:5000

# Expõe a porta para acesso externo
EXPOSE 5000

# As imagens oficiais do .NET (a partir do .NET 8) já criam um usuário não-root chamado 'app'
# Abaixo garantimos que a aplicação rodará com este usuário não privilegiado
USER app

# Define o comando de inicialização do container
ENTRYPOINT ["dotnet", "HyDrata.GestaoApi.dll"]
