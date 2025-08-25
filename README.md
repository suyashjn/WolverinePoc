# WolverinePoc

A proof-of-concept .NET 8 application demonstrating message-driven architecture using [Wolverine](https://wolverine.netlify.app/), RabbitMQ, and PostgreSQL. The project features robust error handling, dead letter queue (DLQ) management, and HTTP endpoints for message publishing and dead letter inspection.

## Features

- **Wolverine Integration:** Durable messaging and background processing.
- **RabbitMQ Transport:** Reliable message queuing with circuit breaker and queue size limits.
- **PostgreSQL Persistence:** Stores Wolverine's dead letter messages.
- **Dead Letter REST API:** Inspect and manage failed messages via HTTP.
- **Swagger UI:** Interactive API documentation and testing.
- **Docker Compose:** Local development with RabbitMQ, PostgreSQL, and pgAdmin.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/products/docker-desktop)

## Getting Started

### 1. Clone the Repository
### 2. Start Infrastructure with Docker Compose
This will start:
- RabbitMQ (management UI at [localhost:15672](http://localhost:15672), default user/pass: guest/guest)
- PostgreSQL (port 5432, user: postgres, password: secret, db: wolverine_demo)
- pgAdmin (web UI at [localhost:8080](http://localhost:8080), user: admin@admin.com, password: admin)

### 3. Run the Application
### 4. Explore the API

- **Swagger UI:** [http://localhost:5000/swagger](http://localhost:5000/swagger) (or the port your app runs on)
- **Dead Letter Endpoints:** Provided by Wolverine for inspecting failed messages.

## Configuration

Connection strings and RabbitMQ URLs are read from `appsettings.json` or environment variables:
## Message Publishing

You can publish messages to RabbitMQ via the provided HTTP endpoint (see Swagger for details). Messages are processed by handlers in `WolverinePOC.Handlers`.

## Error Handling & Dead Letters

- Circuit breaker and retry policies are configured for robust message processing.
- Failed messages are moved to the dead letter store in PostgreSQL and can be inspected via REST endpoints.

## Development Notes

- The project targets C# 12 and .NET 8.
- All infrastructure data (PostgreSQL) is persisted using Docker volumes.

## License

MIT

---

**References:**
- [Wolverine Documentation](https://wolverine.netlify.app/)
- [RabbitMQ Documentation](https://www.rabbitmq.com/)
- [PostgreSQL Documentation](https://www.postgresql.org/)
