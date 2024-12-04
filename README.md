# SmartBOT 

SmartBOT is an innovative bot that simulates a Tesla helpdesk and provides answers to questions about Tesla Motors.


## Overview

SmartBOT is a solution that includes:
1. A Web API for managing the bot logic and communication with external services.
2. A Console Application to interact with the bot.

This guide provides a step-by-step guide to set up and run the project.


## Prerequisites

Antes de começar, certifique-se de ter os seguintes itens instalados no seu ambiente:

1. **.NET SDK 9**: [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
2. **Editor de Código/IDE** (opcional):
   - Visual Studio (recomendado)
   - Visual Studio Code
   - Rider
3. **Banco de Dados**:
   - O projeto utiliza SQLite, que não requer instalação adicional. O arquivo do banco será criado automaticamente ao executar a WebAPI.



## Configuration

Before running the solution, ensure that the API keys and connection strings are set correctly in the `appsettings.json` file under the `SmartBOT.WebAPI` folder.

```json
{
  "OpenAI": {
    "ApiKey": "<Your OpenAI API Key>",
    "BaseAddress": "https://api.openai.com/v1/"
  },
  "AzureAISearch": {
    "ApiKey": "<Your Azure AI Search API Key>",
    "BaseAddress": "https://your-search-service.search.windows.net/"
  },
  "ConnectionStrings": {
    "SqLiteConnectionString": "Data Source=chat_history.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```
Replace **\<Your OpenAI API Key\>** and **\<Your Azure AI Search API Key\>** with your respective API keys.


---


## **Project Setup**

### 1. Clone the Repository
```bash
git clone https://github.com/your-username/smartbot.git
cd smartbot
```

### 2. Navegue até a pasta da WebAPI:
```bash
cd SmartBOT.WebAPI
```
### 3. Execute o projeto:
```bash
dotnet run
```
### 4. A API será iniciada. Você pode acessar o Swagger 
```bash
http://localhost:5053/swagger
```
### 5. Abra um novo terminal e navegue até a pasta do Console App: 
```bash
cd SmartBOT.ConsoleApp
```

### 6. Execute o projeto:
```bash
dotnet run
```

### 7.Interaja com o assistente virtual SmartBOT.
```bash
dotnet run
```





















