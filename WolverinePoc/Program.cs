using JasperFx.Core;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Http;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;
using WolverinePoc.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var rabbitUrl = builder.Configuration.GetSection("RabbitMq").GetValue<string>("Url")!;
var pgConn = builder.Configuration.GetConnectionString("DefaultConnection")!;

// Add Wolverine HTTP endpoints (Dead Letter REST API)
builder.Services.AddWolverineHttp();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Wolverine and transports
builder.Host.UseWolverine(opts =>
{
    // persistence for built-in dead letter storage
    opts.PersistMessagesWithPostgresql(pgConn);

    // RabbitMQ transport
    opts.UseRabbitMq(new Uri(rabbitUrl))
        .DisableSystemRequestReplyQueueDeclaration()
        .DeclareQueue("my_queue", q =>
        {
            q.Arguments["x-max-length"] = 1000; // limit to 1000 messages
            q.Arguments["x-overflow"] = "reject-publish"; // reject new messages when full
        })

        .DisableDeadLetterQueueing()
        .AutoProvision();

    // Incoming queue: retries -> RabbitMQ DLQ; circuit breaker; queue size limit
    opts.ListenToRabbitQueue("my_queue")
        // Circuit breaker
        .CircuitBreaker(cb =>
        {
            // Configure based on your traffic characteristics
            cb.MinimumThreshold = 10;               // only evaluate once this many messages processed
            cb.TrackingPeriod = TimeSpan.FromMinutes(1);
            cb.FailurePercentageThreshold = 15;     // trip if >15% failures
            cb.PauseTime = TimeSpan.FromMinutes(5); // pause consumption
        });

    opts.OnException<Exception>()
        .RetryWithCooldown(
            1.Seconds(),
            2.Seconds()
        )
        .Then.MoveToErrorQueue()
        .Then.Requeue()
            .AndPauseProcessing(10.Minutes());


    //DLQ queue that a handler will read and store into Wolverine's dead letter store
    //opts.ListenToRabbitQueue("wolverine-dead-letter-queue")
    //    .DisableDeadLetterQueueing()

    // Discovery so handlers are found
    opts.Discovery.IncludeAssembly(typeof(IncomingMessageHandler).Assembly);
});

var app = builder.Build();

// Enable Swagger middleware in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapWolverineEndpoints();

// Map Wolverine's built-in dead letter REST endpoints
app.MapDeadLettersEndpoints();

await app.RunAsync();
