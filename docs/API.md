# 📡 API Documentation

## Base URL

- **Local:** `http://localhost:5000`
- **Production:** `https://your-api.onrender.com`

## Authentication

Currently, the API doesn't require authentication. This can be added in future iterations using JWT tokens.

## Sessions API

### GET /api/sessions

Get all sessions with pagination.

**Query Parameters:**
- `pageNumber` (optional, default: 1)
- `pageSize` (optional, default: 20)

**Response:** `200 OK`
```json
[
  {
    "id": "guid",
    "name": "string",
    "startTime": "ISO8601",
    "endTime": "ISO8601 | null",
    "status": "Created | Active | Paused | Completed | Cancelled",
    "totalLaps": number
  }
]
```

### GET /api/sessions/active

Get the currently active session.

**Response:** `200 OK` or `404 Not Found`
```json
{
  "id": "guid",
  "name": "string",
  "description": "string | null",
  "startTime": "ISO8601",
  "endTime": "ISO8601 | null",
  "status": "Active",
  "startFinishLine": {
    "latitude": number,
    "longitude": number
  } | null,
  "startFinishLineRadius": number,
  "totalLaps": number,
  "bestLap": { /* Lap object */ } | null,
  "totalDuration": "string | null",
  "totalDistanceMeters": number,
  "laps": [ /* Array of Lap objects */ ]
}
```

### GET /api/sessions/{id}

Get a specific session by ID.

**Response:** `200 OK` or `404 Not Found`
```json
{
  "id": "guid",
  "name": "string",
  "description": "string | null",
  // ... full session details
}
```

### POST /api/sessions

Create a new session.

**Request Body:**
```json
{
  "name": "string (required)",
  "description": "string (optional)"
}
```

**Response:** `201 Created`
```json
{
  "id": "guid",
  // ... full session details
}
```

### POST /api/sessions/{id}/start

Start a session.

**Response:** `200 OK`
```json
{
  "id": "guid",
  "status": "Active",
  // ... full session details
}
```

### POST /api/sessions/{id}/complete

Complete a session.

**Response:** `200 OK`
```json
{
  "id": "guid",
  "status": "Completed",
  "endTime": "ISO8601",
  // ... full session details
}
```

### POST /api/sessions/{id}/start-finish-line

Set the start/finish line for lap detection.

**Request Body:**
```json
{
  "latitude": number (required),
  "longitude": number (required),
  "radiusMeters": number (optional, default: 20)
}
```

**Response:** `204 No Content`

### POST /api/sessions/{id}/tracking-points

Record a GPS tracking point.

**Request Body:**
```json
{
  "latitude": number (required),
  "longitude": number (required),
  "speedKmh": number (required),
  "timestamp": "ISO8601 (required)",
  "altitude": number (optional),
  "accuracy": number (optional),
  "heading": number (optional)
}
```

**Response:** `204 No Content`

**Note:** This endpoint also triggers real-time updates via SignalR.

### GET /api/sessions/{id}/laps

Get all laps for a specific session.

**Response:** `200 OK`
```json
[
  {
    "id": "guid",
    "sessionId": "guid",
    "lapNumber": number,
    "startTime": "ISO8601",
    "endTime": "ISO8601",
    "duration": "string (MM:SS.mmm)",
    "durationSeconds": number,
    "averageSpeed": {
      "kilometersPerHour": number,
      "metersPerSecond": number,
      "milesPerHour": number
    },
    "maxSpeed": { /* Speed object */ },
    "distanceMeters": number
  }
]
```

## SignalR Hub

### Connection

**Hub URL:** `/hubs/tracking`

**Connection:**
```typescript
import * as signalR from '@microsoft/signalr';

const connection = new signalR.HubConnectionBuilder()
  .withUrl('https://your-api.onrender.com/hubs/tracking')
  .withAutomaticReconnect()
  .build();

await connection.start();
```

### Methods

#### Client → Server

**JoinSession**
```typescript
await connection.invoke('JoinSession', sessionId: string);
```

**LeaveSession**
```typescript
await connection.invoke('LeaveSession', sessionId: string);
```

#### Server → Client

**ReceiveTrackingUpdate**

Fired when a new tracking point is recorded.

```typescript
connection.on('ReceiveTrackingUpdate', (trackingPoint) => {
  console.log('New tracking point:', trackingPoint);
  // trackingPoint: TrackingPointDto
});
```

**ReceiveLapCompleted**

Fired when a lap is completed.

```typescript
connection.on('ReceiveLapCompleted', (lap) => {
  console.log('Lap completed:', lap);
  // lap: LapDto
});
```

**ReceiveSessionStatusChanged**

Fired when session status changes.

```typescript
connection.on('ReceiveSessionStatusChanged', (status) => {
  console.log('Session status:', status);
  // status: string
});
```

## Health Check

### GET /health

Check API health status.

**Response:** `200 OK`
```json
{
  "status": "healthy",
  "timestamp": "ISO8601"
}
```

## Error Responses

All endpoints may return the following error responses:

**400 Bad Request**
```json
{
  "message": "Error description",
  "errors": { /* Validation errors */ }
}
```

**404 Not Found**
```json
{
  "message": "Resource not found"
}
```

**500 Internal Server Error**
```json
{
  "message": "Internal server error"
}
```

## Rate Limiting

Currently, no rate limiting is implemented. Consider adding rate limiting in production.

## CORS

The API is configured to accept requests from:
- `http://localhost:5173` (local dev)
- `http://localhost:3000`
- `https://*.vercel.app` (production frontend)

## Data Types

### SessionStatus
- `Created`: Session created but not started
- `Active`: Session is currently running
- `Paused`: Session temporarily paused
- `Completed`: Session finished successfully
- `Cancelled`: Session cancelled

### TrackingPointType
- `Normal`: Regular GPS point
- `LapStart`: Point marking lap start
- `LapFinish`: Point marking lap finish
- `SessionStart`: First point of session
- `SessionEnd`: Last point of session

### Coordinate
```typescript
{
  latitude: number;   // -90 to 90
  longitude: number;  // -180 to 180
}
```

### Speed
```typescript
{
  kilometersPerHour: number;
  metersPerSecond: number;
  milesPerHour: number;
}
```

## Usage Examples

### Complete Workflow

```typescript
// 1. Create a session
const session = await fetch('https://api.example.com/api/sessions', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    name: 'Morning Practice',
    description: 'Training session'
  })
});

// 2. Connect to SignalR
const connection = new signalR.HubConnectionBuilder()
  .withUrl('https://api.example.com/hubs/tracking')
  .build();

await connection.start();
await connection.invoke('JoinSession', session.id);

// 3. Listen for updates
connection.on('ReceiveTrackingUpdate', (point) => {
  console.log('New point:', point);
});

connection.on('ReceiveLapCompleted', (lap) => {
  console.log('Lap completed:', lap);
});

// 4. Set start/finish line
await fetch(`https://api.example.com/api/sessions/${session.id}/start-finish-line`, {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    latitude: 40.7128,
    longitude: -74.0060,
    radiusMeters: 20
  })
});

// 5. Start the session
await fetch(`https://api.example.com/api/sessions/${session.id}/start`, {
  method: 'POST'
});

// 6. Record tracking points
const sendPosition = async (lat, lon, speed) => {
  await fetch(`https://api.example.com/api/sessions/${session.id}/tracking-points`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      latitude: lat,
      longitude: lon,
      speedKmh: speed,
      timestamp: new Date().toISOString()
    })
  });
};

// 7. Complete the session
await fetch(`https://api.example.com/api/sessions/${session.id}/complete`, {
  method: 'POST'
});

// 8. Disconnect SignalR
await connection.stop();
```

## OpenAPI/Swagger

When running in development mode, Swagger UI is available at:
- `http://localhost:5000/swagger`

This provides an interactive API documentation interface.
