# Clean Architecture Overview

## 🏛️ Architecture Layers

This project follows Clean Architecture principles with four distinct layers:

```
┌─────────────────────────────────────────┐
│           API Layer (Web)               │
│  Controllers, Middleware, Hubs          │
└───────────────┬─────────────────────────┘
                │
┌───────────────▼─────────────────────────┐
│      Application Layer (Use Cases)      │
│  Commands, Queries, DTOs, Interfaces    │
└───────────────┬─────────────────────────┘
                │
┌───────────────▼─────────────────────────┐
│       Domain Layer (Business Logic)     │
│  Entities, Value Objects, Services      │
└───────────────┬─────────────────────────┘
                │
┌───────────────▼─────────────────────────┐
│    Infrastructure Layer (External)      │
│  Database, SignalR, External APIs       │
└─────────────────────────────────────────┘
```

## 📦 Layer Responsibilities

### 1. Domain Layer
**Purpose:** Pure business logic, independent of frameworks

**Contains:**
- **Entities:** Core business objects (Session, TrackingPoint, Lap, Rider)
- **Value Objects:** Immutable types (Coordinate, Speed, Duration)
- **Domain Services:** Business logic that doesn't fit in entities (LapDetectionService)
- **Abstractions:** Interfaces for tracking providers (ITrackingProvider, IPositionSource)
- **Domain Events:** Events that represent business occurrences

**Rules:**
- No dependencies on other layers
- No framework dependencies
- Pure C# / .NET types only
- Rich domain models with behavior

**Example:**
```csharp
public class Session
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public SessionStatus Status { get; private set; }
    
    private readonly List<TrackingPoint> _trackingPoints = new();
    public IReadOnlyCollection<TrackingPoint> TrackingPoints => _trackingPoints;
    
    public void AddTrackingPoint(Coordinate coordinate, Speed speed, DateTime timestamp)
    {
        if (Status != SessionStatus.Active)
            throw new InvalidOperationException("Cannot add points to inactive session");
            
        _trackingPoints.Add(new TrackingPoint(coordinate, speed, timestamp));
    }
    
    public void Complete()
    {
        if (Status != SessionStatus.Active)
            throw new InvalidOperationException("Session is not active");
            
        EndTime = DateTime.UtcNow;
        Status = SessionStatus.Completed;
    }
}
```

### 2. Application Layer
**Purpose:** Orchestrate business logic, implement use cases

**Contains:**
- **Commands:** Write operations (CreateSessionCommand, RecordTrackingPointCommand)
- **Queries:** Read operations (GetActiveSessionQuery, GetSessionHistoryQuery)
- **DTOs:** Data transfer objects for API communication
- **Interfaces:** Abstractions for infrastructure (ISessionRepository, IRealtimePublisher)
- **Services:** Application services orchestrating domain logic

**Rules:**
- Depends only on Domain layer
- No framework-specific code
- Implements use cases
- Defines interfaces for infrastructure

**Example:**
```csharp
public interface ISessionService
{
    Task<SessionDto> CreateSessionAsync(CreateSessionCommand command);
    Task<SessionDto> GetActiveSessionAsync();
    Task RecordTrackingPointAsync(RecordTrackingPointCommand command);
    Task CompleteSessionAsync(Guid sessionId);
}

public class SessionService : ISessionService
{
    private readonly ISessionRepository _repository;
    private readonly ILapDetectionService _lapDetection;
    private readonly IRealtimePublisher _realtimePublisher;
    
    public async Task RecordTrackingPointAsync(RecordTrackingPointCommand command)
    {
        var session = await _repository.GetActiveSessionAsync();
        
        session.AddTrackingPoint(
            new Coordinate(command.Latitude, command.Longitude),
            new Speed(command.SpeedKmh),
            command.Timestamp
        );
        
        // Check for lap completion
        var lap = _lapDetection.DetectLap(session);
        if (lap != null)
        {
            session.CompleteLap(lap);
        }
        
        await _repository.UpdateAsync(session);
        
        // Publish real-time update
        await _realtimePublisher.PublishTrackingUpdate(session.Id, command);
    }
}
```

### 3. Infrastructure Layer
**Purpose:** Implement technical concerns and external dependencies

**Contains:**
- **Persistence:** EF Core DbContext, Repository implementations
- **Real-time:** SignalR hub implementations
- **External Services:** GPS provider implementations, BLE integrations
- **Configuration:** Database migrations, service registrations

**Rules:**
- Implements interfaces defined in Application layer
- Contains all external dependencies
- EF Core, SignalR, HTTP clients live here

**Example:**
```csharp
public class SessionRepository : ISessionRepository
{
    private readonly MotocrossDbContext _context;
    
    public async Task<Session> GetActiveSessionAsync()
    {
        return await _context.Sessions
            .Include(s => s.TrackingPoints)
            .Include(s => s.Laps)
            .FirstOrDefaultAsync(s => s.Status == SessionStatus.Active);
    }
    
    public async Task UpdateAsync(Session session)
    {
        _context.Sessions.Update(session);
        await _context.SaveChangesAsync();
    }
}

public class RealtimeTrackingPublisher : IRealtimePublisher
{
    private readonly IHubContext<TrackingHub> _hubContext;
    
    public async Task PublishTrackingUpdate(Guid sessionId, TrackingPointDto data)
    {
        await _hubContext.Clients.Group(sessionId.ToString())
            .SendAsync("ReceiveTrackingUpdate", data);
    }
}
```

### 4. API Layer
**Purpose:** Expose HTTP endpoints and handle web concerns

**Contains:**
- **Controllers:** Thin controllers delegating to application services
- **Middleware:** Error handling, logging, CORS
- **SignalR Hubs:** Real-time communication endpoints
- **Configuration:** Dependency injection, pipeline setup

**Rules:**
- Minimal logic in controllers
- Dependency injection of application services
- HTTP concerns only (routing, status codes, validation)

**Example:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly ISessionService _sessionService;
    
    [HttpPost]
    public async Task<ActionResult<SessionDto>> CreateSession(CreateSessionCommand command)
    {
        var session = await _sessionService.CreateSessionAsync(command);
        return CreatedAtAction(nameof(GetSession), new { id = session.Id }, session);
    }
    
    [HttpPost("{id}/tracking-points")]
    public async Task<IActionResult> RecordTrackingPoint(
        Guid id, 
        [FromBody] RecordTrackingPointCommand command)
    {
        command.SessionId = id;
        await _sessionService.RecordTrackingPointAsync(command);
        return NoContent();
    }
}
```

## 🔄 Data Flow

### Write Operation (Command)
```
HTTP Request → Controller → Application Service → Domain Entity → Repository → Database
                                    ↓
                            Realtime Publisher → SignalR → Clients
```

### Read Operation (Query)
```
HTTP Request → Controller → Application Service → Repository → Database
                                    ↓
                                Map to DTO → Return
```

## 🎯 Key Benefits

1. **Testability:** Domain logic isolated, easy to unit test
2. **Independence:** Business rules don't depend on frameworks
3. **Flexibility:** Easy to swap infrastructure (database, messaging)
4. **Maintainability:** Clear separation of concerns
5. **Scalability:** Each layer can evolve independently

## 📐 CQRS-Lite Approach

We use a lightweight CQRS pattern:

- **Commands:** Modify state (CreateSession, RecordTrackingPoint)
- **Queries:** Read state (GetActiveSession, GetSessionHistory)
- **Separation:** Different models for read vs write can be introduced later

This is "lite" because:
- Commands and queries share the same database
- No event sourcing (yet)
- Simple enough for MVP, extensible for future needs

## 🔌 Tracking Provider Abstraction

The system is designed to support multiple tracking sources:

```csharp
public interface ITrackingProvider
{
    string ProviderName { get; }
    Task<bool> IsAvailableAsync();
    IAsyncEnumerable<PositionUpdate> StreamPositionsAsync(CancellationToken ct);
}

public interface IPositionSource
{
    Task<Coordinate> GetCurrentPositionAsync();
}
```

**Implementations:**
- `MobileGpsProvider` - Browser geolocation API
- `BleTagProvider` - Bluetooth Low Energy tags (future)
- `ExternalGpsProvider` - Dedicated GPS devices (future)

## 🚀 Best Practices

1. **Keep controllers thin:** Delegate to application services immediately
2. **Rich domain models:** Put behavior in entities, not services
3. **Use value objects:** For concepts like Coordinate, Speed
4. **Interface segregation:** Small, focused interfaces
5. **Async all the way:** Use async/await consistently
6. **No business logic in infrastructure:** Only technical implementations

## 📚 Further Reading

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
