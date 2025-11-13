# Docker Compose Setup

This project uses Docker Compose for local development and orchestration of all microservices.

## Prerequisites

- Docker Desktop or Docker Engine
- Docker Compose V2

## Architecture

The application consists of:
- **User API** (Port 5001) - User authentication and management
- **Chat API** (Port 5002) - Chat and messaging functionality
- **Presence API** (Port 5003) - Real-time user presence tracking
- **Webhook API** (Port 5004) - Webhook management
- **Archive API** (Port 5005) - Message archiving
- **Chat UI** (Port 5000) - Blazor web interface
- **PostgreSQL** (Port 5432) - Database for User and Chat services
- **Redis** (Port 6379) - Caching and session management

## Getting Started

### Build and Run All Services

```bash
docker-compose up --build
```

### Run Services in Detached Mode

```bash
docker-compose up -d
```

### Stop All Services

```bash
docker-compose down
```

### Stop and Remove Volumes (Clean Slate)

```bash
docker-compose down -v
```

## Service URLs

Once running, the services are available at:

- Chat UI: http://localhost:5000
- User API: http://localhost:5001
  - Swagger: http://localhost:5001/swagger
- Chat API: http://localhost:5002
  - Swagger: http://localhost:5002/swagger
- Presence API: http://localhost:5003
  - Swagger: http://localhost:5003/swagger
- Webhook API: http://localhost:5004
  - Swagger: http://localhost:5004/swagger
- Archive API: http://localhost:5005
  - Swagger: http://localhost:5005/swagger

## Database Management

The PostgreSQL database is available at:
- Host: localhost
- Port: 5432
- Username: postgres
- Password: postgres

Two databases are created automatically:
- `userdb` - User service database
- `chatdb` - Chat service database

EF Core migrations are applied automatically when the services start.

## Visual Studio Integration

The solution includes a `docker-compose.dcproj` file that integrates with Visual Studio:

1. Open the solution in Visual Studio
2. Set `docker-compose` as the startup project
3. Press F5 to build and run all services
4. Visual Studio will automatically open the Chat UI in your browser

## Development Workflow

### Rebuilding a Single Service

```bash
docker-compose up --build user-api
```

### Viewing Logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f user-api
```

### Accessing a Service Shell

```bash
docker-compose exec user-api /bin/bash
```

## Troubleshooting

### Database Connection Issues

If services can't connect to the database:
1. Ensure PostgreSQL container is healthy: `docker-compose ps`
2. Check PostgreSQL logs: `docker-compose logs postgres`
3. Restart the database: `docker-compose restart postgres`

### Port Conflicts

If you get port binding errors:
1. Check if ports are already in use: `netstat -an | grep <port>`
2. Stop conflicting services
3. Or modify ports in `docker-compose.yml`

### Clean Rebuild

For a complete clean rebuild:
```bash
docker-compose down -v
docker-compose build --no-cache
docker-compose up
```

## Migration from Aspire

This project was previously using .NET Aspire for orchestration. The migration to Docker Compose includes:

- ✅ Removed all Aspire dependencies and configuration
- ✅ Added standard PostgreSQL connection strings to services
- ✅ Created Dockerfiles for all services
- ✅ Created PostgreSQL migrations for User and Chat services
- ✅ Configured inter-service networking
- ✅ Added Visual Studio integration via docker-compose.dcproj

## Production Considerations

For production deployment, consider:
- Use environment-specific configuration files
- Store secrets in a secure vault (Azure Key Vault, AWS Secrets Manager, etc.)
- Use a managed database service instead of containerized PostgreSQL
- Implement proper health checks and monitoring
- Configure resource limits in docker-compose.yml
- Use a reverse proxy (nginx, Traefik) for SSL termination
- Implement rate limiting and API throttling
- Set up proper logging and observability
