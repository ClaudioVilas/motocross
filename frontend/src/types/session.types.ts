// Session types
export interface SessionSummary {
  id: string;
  name: string;
  startTime: string;
  endTime?: string;
  status: SessionStatus;
  totalLaps: number;
}

export interface Session {
  id: string;
  name: string;
  description?: string;
  startTime: string;
  endTime?: string;
  status: SessionStatus;
  startFinishLine?: Coordinate;
  startFinishLineRadius: number;
  totalLaps: number;
  bestLap?: Lap;
  totalDuration?: string;
  totalDistanceMeters: number;
  laps: Lap[];
}

export type SessionStatus = 'Created' | 'Active' | 'Paused' | 'Completed' | 'Cancelled';

// Coordinate types
export interface Coordinate {
  latitude: number;
  longitude: number;
}

// Speed types
export interface Speed {
  kilometersPerHour: number;
  metersPerSecond: number;
  milesPerHour: number;
}

// Tracking point types
export interface TrackingPoint {
  id: string;
  sessionId: string;
  coordinate: Coordinate;
  speed: Speed;
  timestamp: string;
  type: TrackingPointType;
  altitude?: number;
  accuracy?: number;
  heading?: number;
}

export type TrackingPointType = 'Normal' | 'LapStart' | 'LapFinish' | 'SessionStart' | 'SessionEnd';

// Lap types
export interface Lap {
  id: string;
  sessionId: string;
  lapNumber: number;
  startTime: string;
  endTime: string;
  duration: string;
  durationSeconds: number;
  averageSpeed: Speed;
  maxSpeed: Speed;
  distanceMeters: number;
}

// Command types (for API requests)
export interface CreateSessionCommand {
  name: string;
  description?: string;
}

export interface SetStartFinishLineCommand {
  latitude: number;
  longitude: number;
  radiusMeters?: number;
}

export interface RecordTrackingPointCommand {
  latitude: number;
  longitude: number;
  speedKmh: number;
  timestamp: string;
  altitude?: number;
  accuracy?: number;
  heading?: number;
}
