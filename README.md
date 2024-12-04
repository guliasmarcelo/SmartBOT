# SmartBOT 

SmartBOT is an innovative bot that simulates a Tesla helpdesk and provides answers to questions about Tesla Motors.


## Overview

SmartBOT is a solution that includes:
1. A Web API for managing the bot logic and communication with external services.
2. A Console Application to interact with the bot.

This guide provides a step-by-step guide to set up and run the project.


## Prerequisites

Before you start, make sure you have the following installed in your environment:

1. **.NET SDK 9**: [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
2. **Editor de Código/IDE** (opcional):
   - Visual Studio (recomendado)
   - Visual Studio Code
   - Rider
3. **Banco de Dados**:
   - O projeto utiliza SQLite, que não requer instalação adicional. O arquivo do banco será criado automaticamente ao executar a WebAPI.



## **Project Setup**

### 1. Clone the Repository
```bash
git clone https://github.com/your-username/smartbot.git
cd smartbot
```
### 2. Configure the API Keys
Before running the solution, ensure that the API keys and connection strings are properly set in the appsettings.json file located in the SmartBOT.WebAPI folder.

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

### 3. Navigate to the WebAPI folder:
```bash
cd SmartBOT.WebAPI
```

### 4. Navigate to the WebAPI folder:
```bash
dotnet run
```

### 5. The API will start. You can access Swagger at:
```bash
http://localhost:5053/swagger
```

### 6. Open a new terminal and navigate to the Console App folder:
```bash
cd SmartBOT.ConsoleApp
```

### 7. Run the project:
```bash
dotnet run
```

### 7.Interaja com o assistente virtual SmartBOT.
```bash
dotnet run
```



## Testando a API

### Endpoints Disponíveis
1. Enviar Mensagem para o Assistente
   - POST /api/chat/{helpdeskId}
   - Envia uma mensagem para o assistente e retorna a resposta.
2. Obter Histórico da Conversa
   - GET /api/chat/history/{helpdeskId}
   - Retorna o histórico de mensagens associado ao helpdeskId.


## Testing Using curl
1. Send a Message:
```bash
curl -X 'POST' \
  'http://localhost:5053/api/chat/123456' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "userMessage": "quanto tempo de garantia a bateria tem?"
}'

```
How to access the SQLite database?

The chat_history.db database file will be created in the WebAPI directory.
SmartBOT/SmartBOT.WebAPI/chat_history.db



## Folder Structure
   - SmartBOT/: Solution Folder 
   - SmartBOT.WebAPI/: Contains the WebAPI application.
   - SmartBOT.ConsoleApp/: Contains the console application.
   



    
## Contato
If you have any questions or need support, please contact:
   - E-mail: guliasmarcelo@gmail.com


















