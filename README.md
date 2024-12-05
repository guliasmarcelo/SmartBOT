# SmartBOT 

SmartBOT is an innovative bot designed to simulate a Tesla helpdesk. It provides answers to questions about Tesla Motors and its products using OpenAI and Azure AI Search APIs.


## Overview

SmartBOT is a solution that includes:
1. A Web API for managing the bot logic and communication with external services.
2. A Console Application to interact with the bot.
    


## Prerequisites

Ensure the following are installed in your environment:

1. **.NET SDK 9**: [Download Here](https://dotnet.microsoft.com/download/dotnet/9.0)
2. **Code Editor/IDE** (Optional):
   - Visual Studio (recommended)
   - Visual Studio Code
   - Rider
3. **Database**:
   - The project uses SQLite. No additional installation is required. The database file will be created automatically upon running the WebAPI.



## Setup

#### 1. Clone the Repository
```bash
git clone https://github.com/your-username/smartbot.git
cd smartbot
```

#### 2. Configure the API Keys
Edit the appsettings.json file in the SmartBOT.WebAPI directory to include your API keys:

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



## Running the Project

#### 1. Run the WebAPI:
```bash
cd SmartBOT.WebAPI
dotnet run
```
The WebAPI will start at http://localhost:5053.


#### 2. Access Swagger:
```bash
http://localhost:5053/swagger

```


#### 3. Run the Console App: Open a new terminal, navigate to the ConsoleApp directory, and run::
```bash
cd SmartBOT.ConsoleApp
dotnet run

```



  
## Testing the API

#### Available Endpoints
1. Send a Message
    - Endpoint: POST /api/chat/{helpdeskId}
    - Description: Sends a user message to the assistant and retrieves a response.
    - Sample Payload:
```json
{
  "userMessage": "What are the available Tesla car models?"
}
```


#### Testing with curl
1. Send a Message:

```bash
curl -X POST http://localhost:5053/api/chat/{helpdeskId} \
-H "Content-Type: application/json" \
-d '{"userMessage": "How does Tesla Autopilot work?"}'
```


## Folder Structure
```bash
SmartBOT/
├── SmartBOT.WebAPI/         # WebAPI implementation
├── SmartBOT.ConsoleApp/     # Console application
├── README.md                # Project documentation
└── SmartBOT.WebAPI/chat_history.db          # SQLite database file (auto-created)
```


## Main Technical Decisions
1. SQLite for Persistence:
    - Chosen for simplicity and portability.
    - Requires no additional setup or installation.
    - Automatically creates the chat_history.db file on first run.

      
2. Stateless WebAPI:
    - The WebAPI is stateless, ensuring scalability and making it suitable for a cloud environment.


3. Service-Oriented Architecture:
    - Clean separation between Chat, Embedding, and Vector Search services.
    - Dependency injection is used to manage service lifetimes efficiently.


## Relevant Comments About the Project
1. Clean and Modular Architecture
    - The system is designed with a clear separation of responsibilities, adhering to the Single Responsibility Principle (SRP).
    - Each class focuses on a single responsibility, such as managing chat history, interacting with external APIs, or orchestrating workflows.
    - For example:
        - OpenAIChatService: Handles communication with OpenAI's chat API.
        - OpenAIEmbeddingsService: Manages the generation of embeddings using OpenAI's embedding API.
        - AzureAISearchService: Interfaces with Azure AI Search for vector-based search functionalities.
        - TeslaHelpDeskService: Orchestrates workflows between various services for Tesla helpdesk operations.
        - SqLiteChatHistoryRepository: Manages the persistence of chat history using SQLite.



2. Reusable Services
    - Classes like OpenAIChatService and AzureAISearchService are designed to be reusable across different domains and use cases.
    - These services are not tightly coupled to the Tesla use case and can be reused in other applications requiring OpenAI or Azure AI Search integrations.
    
3. Dependency Injection and Extensibility
    - All services are abstracted behind interfaces, following the Dependency Inversion Principle (DIP).
    - Examples:
        - IChatService for chat operations.
        - IEmbeddingsService for embedding generation.
        - IVectorSearchService for vector-based search functionalities.
        - IChatHistoryRepository for chat history management.
    - This makes the system easily extensible and allows for mocking during testing or swapping implementations if needed.


4. Flexibility and Replaceability
    - The use of interfaces ensures that implementations can be replaced without affecting the rest of the system. For example:
        - If OpenAI or Azure services need to be replaced, alternative implementations can be provided by simply creating new classes that implement the corresponding interfaces.
        - Example Scenario: Replacing OpenAIChatService with a CustomAIChatService.

            
5. Portable and Lightweight Persistence
    - SQLite was chosen as the persistence layer for its simplicity and portability.
        - It does not require additional setup and creates the database automatically when the WebAPI runs.
        - This ensures a lightweight and hassle-free experience during development and deployment.

6. Integration-Oriented Design
    - The TeslaHelpDeskService acts as an orchestration layer, integrating the reusable services to deliver specific functionality:
        - Loads chat history from SqLiteChatHistoryRepository.
        - Uses OpenAIEmbeddingsService to generate embeddings for user queries.
        - Leverages AzureAISearchService for vector-based knowledge base searches.
        - Utilizes OpenAIChatService to interact with the assistant API.
        - Combines these steps into a cohesive workflow.

  
8. Future-Proof Design
    - The modularity and use of interfaces make the system future-proof, allowing for:
        - Scaling to handle multiple bots or domains.
        - Adding new features, such as support for multiple languages or different domains.
        - Migration to cloud-native architectures with minimal changes.


    - 
## Contato
For questions or support:
    - Email: guliasmarcelo@gmail.com
    


















