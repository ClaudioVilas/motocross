import { useEffect, useState } from 'react';
import { signalRService } from '../services/signalr.service';
import { useSessionStore } from '../stores/sessionStore';

export function useSignalR(sessionId?: string) {
  const [error, setError] = useState<Error | null>(null);
  const { setConnectionStatus, addTrackingPoint, addLap, updateSessionStatus } = useSessionStore();

  useEffect(() => {
    let mounted = true;

    const connect = async () => {
      try {
        setConnectionStatus('connecting');
        await signalRService.connect();
        
        if (!mounted) return;
        
        setConnectionStatus('connected');
        setError(null);

        // Set up event listeners
        signalRService.onTrackingUpdate((trackingPoint) => {
          if (mounted) {
            addTrackingPoint(trackingPoint);
          }
        });

        signalRService.onLapCompleted((lap) => {
          if (mounted) {
            addLap(lap);
          }
        });

        signalRService.onSessionStatusChanged((status) => {
          if (mounted) {
            updateSessionStatus(status);
          }
        });

        // Join session if provided
        if (sessionId) {
          await signalRService.joinSession(sessionId);
        }
      } catch (err) {
        if (mounted) {
          setConnectionStatus('error');
          setError(err as Error);
          console.error('SignalR connection error:', err);
        }
      }
    };

    connect();

    return () => {
      mounted = false;
      signalRService.removeAllListeners();
      if (sessionId) {
        signalRService.leaveSession(sessionId);
      }
    };
  }, [sessionId, setConnectionStatus, addTrackingPoint, addLap, updateSessionStatus]);

  return { error, isConnected: signalRService.isConnected() };
}
