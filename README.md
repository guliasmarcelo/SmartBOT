# SmartBOT Solution

## Overview

SmartBOT is a solution that includes:
1. A Web API for managing the bot logic and communication with external services.
2. A Console Application to interact with the bot.

This guide provides a step-by-step guide to set up and run the project using Docker.

---

## Prerequisites

1. **Docker**: Install [Docker Desktop](https://www.docker.com/products/docker-desktop/) for your platform.
2. **Docker Compose**: Ensure Docker Compose is included with your Docker installation.

---

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
