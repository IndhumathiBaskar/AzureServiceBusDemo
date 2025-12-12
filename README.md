az login >> Install Package : curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

# Azure Service Bus .NET API Learning

This repository demonstrates how to integrate a simple .NET API with **Azure Service Bus** for sending and receiving messages.
We explore **Queues** and **Topics & Subscriptions**, using **.NET 10 Web API**, tested in **GitHub Codespaces**, and deployed temporarily for learning.

---

## **Project Overview**

* Simple .NET API with endpoints to **send** and **receive** messages from Azure Service Bus.
* Supports both **Queue-based messaging** and **Pub/Sub with Topics & Subscriptions**.
* Tested in **GitHub Codespaces** — temporary environment for experimentation.
* Learnings include:

  * Creating and configuring **Service Bus queues and topics**
  * Sending messages from API
  * Receiving messages via API
  * Understanding connection strings and Service Bus configuration

---

## **Prerequisites**

* GitHub account & Codespaces
* Azure subscription
* .NET 10 SDK
* Azure CLI (az)
* Basic understanding of REST APIs

---

## **Steps Completed**

### **Step 1: Create GitHub Repository**

1. Create a new repository: `AzureServiceBusDemoAPI`.
2. Open repository in **GitHub Codespaces**.
3. Initialize a new .NET Web API project:

```bash
dotnet new webapi -n AzureServiceBusDemoAPI
cd AzureServiceBusDemoAPI
git init
git add .
git commit -m "Initial commit - .NET API for Service Bus"
git push origin main
```

---

### **Step 2: Add Azure Service Bus Configuration**

Create `appsettings.json`:

```json
{
  "ServiceBus": {
    "ConnectionString": "<YOUR_SERVICE_BUS_CONNECTION_STRING>",
    "QueueName": "myqueue",
    "TopicName": "mytopic",
    "SubscriptionName": "mysubscription"
  }
}
```

* **ConnectionString**: From Azure Service Bus namespace → Shared Access Policies → RootManageSharedAccessKey
* **QueueName**: Name of your queue
* **TopicName**: Name of your topic (for pub/sub)
* **SubscriptionName**: Name of your subscription under the topic

---

### **Step 3: Create Helper Class**

`ServiceBusSenderHelper.cs`:

```csharp
using Azure.Messaging.ServiceBus;

public class ServiceBusSenderHelper
{
    private readonly string connectionString;
    private readonly string entityName;

    public ServiceBusSenderHelper(string connectionString, string entityName)
    {
        this.connectionString = connectionString;
        this.entityName = entityName;
    }

    public async Task SendMessageAsync(string messageBody)
    {
        await using var client = new ServiceBusClient(connectionString);
        ServiceBusSender sender = client.CreateSender(entityName);

        ServiceBusMessage message = new ServiceBusMessage(messageBody);
        await sender.SendMessageAsync(message);

        Console.WriteLine($"Sent message: {messageBody}");
    }
}
```

---

### **Step 4: Update `Program.cs`**

```csharp
using Azure.Messaging.ServiceBus;

var builder = WebApplication.CreateBuilder(args);

// Load Service Bus settings
var serviceBusConnectionString = builder.Configuration["ServiceBus:ConnectionString"];
var queueName = builder.Configuration["ServiceBus:QueueName"];
var topicName = builder.Configuration["ServiceBus:TopicName"];
var subscriptionName = builder.Configuration["ServiceBus:SubscriptionName"];

// Add OpenAPI for testing
builder.Services.AddOpenApi();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Endpoint to send a message to Queue
app.MapPost("/sendqueue", async (string message) =>
{
    var sender = new ServiceBusSenderHelper(serviceBusConnectionString, queueName);
    await sender.SendMessageAsync(message);
    return Results.Ok($"Message sent to queue: {message}");
});

// Endpoint to send a message to Topic
app.MapPost("/sendtopic", async (string message) =>
{
    var sender = new ServiceBusSenderHelper(serviceBusConnectionString, topicName);
    await sender.SendMessageAsync(message);
    return Results.Ok($"Message sent to topic: {message}");
});

// Endpoint to receive a message from Queue
app.MapGet("/receivequeue", async () =>
{
    await using var client = new ServiceBusClient(serviceBusConnectionString);
    var receiver = client.CreateReceiver(queueName);
    var message = await receiver.ReceiveMessageAsync();
    if (message != null)
    {
        await receiver.CompleteMessageAsync(message);
        return Results.Ok($"Received message from queue: {message.Body}");
    }
    return Results.Ok("No messages in queue");
});

app.Run();
```

---

### **Step 5: Run & Test API in Codespaces**

```bash
dotnet run
```

* Send a message to the queue:

```bash
curl -X POST https://<YOUR_CODESPACE_URL>/sendqueue -H "Content-Type: application/json" -d "\"Hello from Queue!\""
```

* Send a message to the topic:

```bash
curl -X POST https://<YOUR_CODESPACE_URL>/sendtopic -H "Content-Type: application/json" -d "\"Hello from Topic!\""
```

* Receive a message from the queue:

```bash
curl https://<YOUR_CODESPACE_URL>/receivequeue
```

---

### **Step 6: Verify in Azure**

* Navigate to your **Service Bus namespace** → Queues / Topics
* Check **Metrics** and **Message Count** to see messages arriving.
* For topics, verify the **Subscription** receives messages.

---

### **Step 7: Cleanup Tips**

* Delete **queues, topics, subscriptions** after learning to avoid costs.
* Codespaces environment is temporary — running API and testing works only while active.

---

## **Key Learning Points**

* **Queue**: Point-to-point messaging.
* **Topic & Subscription**: Pub/Sub messaging.
* **Azure Service Bus**: Handles reliable messaging for distributed systems.
* **.NET API**: Can send and receive messages programmatically.
* **GitHub Codespaces**: Temporary dev environment for learning.
* **Configuration**: Connection strings, queue/topic names stored in `appsettings.json`.

---


Do you want me to do that?

