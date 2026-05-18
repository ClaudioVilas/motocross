import { create } from 'zustand';
import type { Session, TrackingPoint, Lap } from '../types/session.types';

interface SessionStore {
  // State
  currentSession: Session | null;
  trackingPoints: TrackingPoint[];
  isTracking: boolean;
  connectionStatus: 'disconnected' | 'connecting' | 'connected' | 'error';

  // Actions
  setCurrentSession: (session: Session | null) => void;
  updateSessionStatus: (status: string) => void;
  addTrackingPoint: (point: TrackingPoint) => void;
  addLap: (lap: Lap) => void;
  setIsTracking: (isTracking: boolean) => void;
  setConnectionStatus: (status: 'disconnected' | 'connecting' | 'connected' | 'error') => void;
  clearTrackingPoints: () => void;
  reset: () => void;
}

export const useSessionStore = create<SessionStore>((set) => ({
  // Initial state
  currentSession: null,
  trackingPoints: [],
  isTracking: false,
  connectionStatus: 'disconnected',

  // Actions
  setCurrentSession: (session) => set({ currentSession: session }),

  updateSessionStatus: (status) =>
    set((state) =>
      state.currentSession
        ? {
            currentSession: {
              ...state.currentSession,
              status: status as any,
            },
          }
        : {}
    ),

  addTrackingPoint: (point) =>
    set((state) => ({
      trackingPoints: [...state.trackingPoints, point],
    })),

  addLap: (lap) =>
    set((state) =>
      state.currentSession
        ? {
            currentSession: {
              ...state.currentSession,
              laps: [...state.currentSession.laps, lap],
              totalLaps: state.currentSession.totalLaps + 1,
              bestLap:
                !state.currentSession.bestLap ||
                lap.durationSeconds < state.currentSession.bestLap.durationSeconds
                  ? lap
                  : state.currentSession.bestLap,
            },
          }
        : {}
    ),

  setIsTracking: (isTracking) => set({ isTracking }),

  setConnectionStatus: (status) => set({ connectionStatus: status }),

  clearTrackingPoints: () => set({ trackingPoints: [] }),

  reset: () =>
    set({
      currentSession: null,
      trackingPoints: [],
      isTracking: false,
      connectionStatus: 'disconnected',
    }),
}));
