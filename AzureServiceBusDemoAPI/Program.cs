using Azure.Messaging.ServiceBus;

var builder = WebApplication.CreateBuilder(args);

// Load Service Bus settings from appsettings.json
var serviceBusConnectionString = builder.Configuration["ServiceBus:ConnectionString"];
var queueName = builder.Configuration["ServiceBus:QueueName"];

// Add services to the container.
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Endpoint to send a message to Azure Service Bus
app.MapPost("/send", async (string message) =>
{
    var sender = new ServiceBusSenderHelper(serviceBusConnectionString, queueName);
    await sender.SendMessageAsync(message);
    return Results.Ok($"Message sent: {message}");
})
.WithName("SendMessage");

app.Run();

// ServiceBusSenderHelper.cs remains as a separate file
