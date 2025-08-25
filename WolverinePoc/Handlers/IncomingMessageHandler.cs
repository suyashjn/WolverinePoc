using WolverinePOC.Messages;

namespace WolverinePoc.Handlers;

public class IncomingMessageHandler
{
    private readonly ILogger<IncomingMessageHandler> _logger;

    public IncomingMessageHandler(ILogger<IncomingMessageHandler> logger)
    {
        _logger = logger;
    }

    // Handle the message normally; this example throws to force DLQ behavior
    public Task Handle(MyMessage message)
    {
        _logger.LogInformation("Received: {MessageText}", message.Text);

        // Simulate failure for messages containing the word "fail"
        if (message.Text?.Contains("fail", StringComparison.OrdinalIgnoreCase) == true)
        {
            _logger.LogWarning("Simulating failure -> will cause retries / DLQ");
            throw new InvalidOperationException("Simulated processing failure");
        }

        _logger.LogInformation("Processed successfully");
        return Task.CompletedTask;
    }
}
