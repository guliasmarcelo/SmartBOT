# SmartBOT Solution

## Overview

SmartBOT is a solution that includes:
1. A Web API for managing the bot logic and communication with external services.
2. A Console Application to interact with the bot.

This guide provides a step-by-step guide to set up and run the project using Docker.

---

## Prerequisites


Antes de começar, certifique-se de ter os seguintes itens instalados no seu ambiente:

1. **.NET SDK 9**: [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
2. **Editor de Código/IDE** (opcional):
   - Visual Studio (recomendado)
   - Visual Studio Code
   - Rider
3. **Banco de Dados**:
   - O projeto utiliza SQLite, que não requer instalação adicional. O arquivo do banco será criado automaticamente ao executar a WebAPI.

---

## Configuration

Before running the solution, ensure that the API keys and connection strings are set correctly in the `appsettings.json` file under the `SmartBOT.WebAPI` folder.

\```json
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
\```

---


## **Project Setup**

### **1. Clone the Repository**

Clone the repository to your local machine:

\```bash
git clone https://github.com/your-username/smartbot.git
cd smartbot
\```
