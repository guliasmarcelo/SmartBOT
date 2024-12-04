# SmartBOT 

SmartBOT is an innovative bot designed to simulate a Tesla helpdesk. It provides answers to questions about Tesla Motors and its products using OpenAI and Azure AI Search APIs.

---

## Overview

SmartBOT is a solution that includes:
1. A Web API for managing the bot logic and communication with external services.
2. A Console Application to interact with the bot.

---

## **Instructions to Run the Code**

### **1. Prerequisites**

Ensure the following are installed in your environment:

1. **.NET SDK 9**: [Download Here](https://dotnet.microsoft.com/download/dotnet/9.0)
2. **Code Editor/IDE** (Optional):
   - Visual Studio (recommended)
   - Visual Studio Code
   - Rider
3. **Database**:
   - The project uses SQLite. No additional installation is required. The database file will be created automatically upon running the WebAPI.

---


### **2. Setup**

#### Clone the Repository
```bash
git clone https://github.com/your-username/smartbot.git
cd smartbot
```

#### Configure the API Keys
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



### **3. Running the Project**

#### 1. Run the WebAPI:
```bash
cd SmartBOT.WebAPI
dotnet run
```
The WebAPI will start at http://localhost:5053.


#### 2. Access Swagger. Visit:
```bash
http://localhost:5053/swagger

```


#### 3. Run the Console App: Open a new terminal, navigate to the ConsoleApp directory, and run::
```bash
cd SmartBOT.ConsoleApp
dotnet run

```

### **2. Main Technical Decisions**
1. SQLite for Persistence:
    - Chosen for simplicity and portability.
    - Requires no additional setup or installation.
    - Automatically creates the chat_history.db file on first run.
      
2. Stateless WebAPI:
    - The WebAPI is stateless, ensuring scalability and making it suitable for a cloud environment.


3. Service-Oriented Architecture:
    - Clean separation between Chat, Embedding, and Vector Search services.
    - Dependency injection is used to manage service lifetimes efficiently.

  
### **4. Testing the API**

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

2. Retrieve Chat History
    - Endpoint: GET /api/chat/history/{helpdeskId}
    - Description: Retrieves the chat history for a specific helpdeskId.

#### Testing with curl
1. Send a Message:

```bash
curl -X POST http://localhost:5053/api/chat/{helpdeskId} \
-H "Content-Type: application/json" \
-d '{"userMessage": "How does Tesla Autopilot work?"}'
```


### **4. Folder Structure**
SmartBOT/
├── SmartBOT.WebAPI/         # WebAPI implementation
├── SmartBOT.ConsoleApp/     # Console application
├── README.md                # Project documentation
└── SmartBOT.WebAPI/chat_history.db          # SQLite database file (auto-created)







### 7.Interact with the virtual assistant SmartBOT:
```bash
dotnet run
```



    
## Contato
For questions or support:
    - Email: guliasmarcelo@gmail.com
    


















