using Azure.Messaging.ServiceBus;

public class ServiceBusSenderHelper
{
    private readonly string connectionString;
    private readonly string queueName;

    public ServiceBusSenderHelper(string connectionString, string queueName)
    {
        this.connectionString = connectionString;
        this.queueName = queueName;
    }

    public async Task SendMessageAsync(string messageBody)
    {
        await using var client = new ServiceBusClient(connectionString);
        ServiceBusSender sender = client.CreateSender(queueName);

        ServiceBusMessage message = new ServiceBusMessage(messageBody);
        await sender.SendMessageAsync(message);

        Console.WriteLine($"Sent message: {messageBody}");
    }
}
