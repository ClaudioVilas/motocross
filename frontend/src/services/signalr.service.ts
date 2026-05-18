import * as signalR from '@microsoft/signalr';
import type { TrackingPoint, Lap } from '../types/session.types';

export type TrackingUpdateCallback = (trackingPoint: TrackingPoint) => void;
export type LapCompletedCallback = (lap: Lap) => void;
export type SessionStatusChangedCallback = (status: string) => void;

class SignalRService {
  private connection: signalR.HubConnection | null = null;
  private  currentSessionId: string | null = null;

  async connect(): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      return;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(import.meta.env.VITE_SIGNALR_HUB_URL)
      .withAutomaticReconnect()
      .build();

    try {
      await this.connection.start();
      console.log('SignalR Connected');
    } catch (error) {
      console.error('SignalR Connection Error:', error);
      throw error;
    }
  }

  async disconnect(): Promise<void> {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
      this.currentSessionId = null;
      console.log('SignalR Disconnected');
    }
  }

  async joinSession(sessionId: string): Promise<void> {
    if (!this.connection) {
      throw new Error('SignalR connection not established');
    }

    if (this.currentSessionId && this.currentSessionId !== sessionId) {
      await this.leaveSession(this.currentSessionId);
    }

    await this.connection.invoke('JoinSession', sessionId);
    this.currentSessionId = sessionId;
    console.log(`Joined session: ${sessionId}`);
  }

  async leaveSession(sessionId: string): Promise<void> {
    if (!this.connection) {
      return;
    }

    await this.connection.invoke('LeaveSession', sessionId);
    this.currentSessionId = null;
    console.log(`Left session: ${sessionId}`);
  }

  onTrackingUpdate(callback: TrackingUpdateCallback): void {
    if (!this.connection) {
      throw new Error('SignalR connection not established');
    }

    this.connection.on('ReceiveTrackingUpdate', callback);
  }

  onLapCompleted(callback: LapCompletedCallback): void {
    if (!this.connection) {
      throw new Error('SignalR connection not established');
    }

    this.connection.on('ReceiveLapCompleted', callback);
  }

  onSessionStatusChanged(callback: SessionStatusChangedCallback): void {
    if (!this.connection) {
      throw new Error('SignalR connection not established');
    }

    this.connection.on('ReceiveSessionStatusChanged', callback);
  }

  removeAllListeners(): void {
    if (this.connection) {
      this.connection.off('ReceiveTrackingUpdate');
      this.connection.off('ReceiveLapCompleted');
      this.connection.off('ReceiveSessionStatusChanged');
    }
  }

  getConnectionState(): signalR.HubConnectionState | null {
    return this.connection?.state ?? null;
  }

  isConnected(): boolean {
    return this.connection?.state === signalR.HubConnectionState.Connected;
  }
}

export const signalRService = new SignalRService();
