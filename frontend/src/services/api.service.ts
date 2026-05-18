import axios from 'axios';
import type { AxiosInstance } from 'axios';
import type {
  Session,
  SessionSummary,
  CreateSessionCommand,
  SetStartFinishLineCommand,
  RecordTrackingPointCommand,
  Lap,
} from '../types/session.types';

class ApiClient {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: import.meta.env.VITE_API_BASE_URL,
      headers: {
        'Content-Type': 'application/json',
      },
    });
  }

  // Session endpoints
  async getSessions(pageNumber = 1, pageSize = 20): Promise<SessionSummary[]> {
    const response = await this.client.get('/api/sessions', {
      params: { pageNumber, pageSize },
    });
    return response.data;
  }

  async getActiveSession(): Promise<Session | null> {
    try {
      const response = await this.client.get('/api/sessions/active');
      return response.data;
    } catch (error: any) {
      if (error.response?.status === 404) {
        return null;
      }
      throw error;
    }
  }

  async getSession(id: string): Promise<Session> {
    const response = await this.client.get(`/api/sessions/${id}`);
    return response.data;
  }

  async createSession(command: CreateSessionCommand): Promise<Session> {
    const response = await this.client.post('/api/sessions', command);
    return response.data;
  }

  async startSession(id: string): Promise<Session> {
    const response = await this.client.post(`/api/sessions/${id}/start`);
    return response.data;
  }

  async completeSession(id: string): Promise<Session> {
    const response = await this.client.post(`/api/sessions/${id}/complete`);
    return response.data;
  }

  async setStartFinishLine(
    id: string,
    command: SetStartFinishLineCommand
  ): Promise<void> {
    await this.client.post(`/api/sessions/${id}/start-finish-line`, command);
  }

  async recordTrackingPoint(
    id: string,
    command: RecordTrackingPointCommand
  ): Promise<void> {
    await this.client.post(`/api/sessions/${id}/tracking-points`, command);
  }

  async getSessionLaps(id: string): Promise<Lap[]> {
    const response = await this.client.get(`/api/sessions/${id}/laps`);
    return response.data;
  }

  // Health check
  async healthCheck(): Promise<{ status: string; timestamp: string }> {
    const response = await this.client.get('/health');
    return response.data;
  }
}

export const apiClient = new ApiClient();
