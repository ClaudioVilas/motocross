import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { apiClient } from '../services/api.service';
import { useSignalR } from '../hooks/useSignalR';
import { useGPSTracking } from '../hooks/useGPSTracking';
import { useSessionStore } from '../stores/sessionStore';
import type { RecordTrackingPointCommand } from '../types/session.types';

export const SessionView: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [trackingWatchId, setTrackingWatchId] = useState<number | null>(null);

  const {
    currentSession,
    trackingPoints,
    isTracking: storeIsTracking,
    setCurrentSession,
    setIsTracking: setStoreIsTracking,
    reset,
  } = useSessionStore();

  const { data: session, isLoading } = useQuery({
    queryKey: ['session', id],
    queryFn: () => apiClient.getSession(id!),
    enabled: !!id,
  });

  const { error: signalRError } = useSignalR(id);

  const {
    position,
    error: gpsError,
    isAvailable: isGPSAvailable,
    startTracking,
    stopTracking,
  } = useGPSTracking();

  const recordPointMutation = useMutation({
    mutationFn: (command: RecordTrackingPointCommand) =>
      apiClient.recordTrackingPoint(id!, command),
  });

  const startSessionMutation = useMutation({
    mutationFn: () => apiClient.startSession(id!),
    onSuccess: (updatedSession) => {
      setCurrentSession(updatedSession);
      queryClient.invalidateQueries({ queryKey: ['session', id] });
    },
  });

  const completeSessionMutation = useMutation({
    mutationFn: () => apiClient.completeSession(id!),
    onSuccess: () => {
      if (trackingWatchId !== null) {
        stopTracking(trackingWatchId);
        setTrackingWatchId(null);
      }
      setStoreIsTracking(false);
      queryClient.invalidateQueries({ queryKey: ['session', id] });
      reset();
      navigate('/');
    },
  });

  useEffect(() => {
    if (session) {
      setCurrentSession(session);
    }
  }, [session, setCurrentSession]);

  useEffect(() => {
    if (position && storeIsTracking) {
      const speedKmh = position.coords.speed
        ? position.coords.speed * 3.6
        : 0;

      const command: RecordTrackingPointCommand = {
        latitude: position.coords.latitude,
        longitude: position.coords.longitude,
        speedKmh,
        timestamp: new Date(position.timestamp).toISOString(),
        altitude: position.coords.altitude ?? undefined,
        accuracy: position.coords.accuracy,
        heading: position.coords.heading ?? undefined,
      };

      recordPointMutation.mutate(command);
    }
  }, [position]);

  const handleStartTracking = () => {
    if (!isGPSAvailable) {
      alert('GPS is not available on this device');
      return;
    }

    if (currentSession?.status === 'Created') {
      startSessionMutation.mutate();
    }

    const watchId = startTracking();
    if (watchId !== null) {
      setTrackingWatchId(watchId);
      setStoreIsTracking(true);
    }
  };

  const handleStopTracking = () => {
    if (trackingWatchId !== null) {
      stopTracking(trackingWatchId);
      setTrackingWatchId(null);
    }
    setStoreIsTracking(false);
  };

  const handleCompleteSession = () => {
    if (confirm('Are you sure you want to complete this session?')) {
      completeSessionMutation.mutate();
    }
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-xl">Loading session...</div>
      </div>
    );
  }

  if (!currentSession) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-xl text-red-500">Session not found</div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-6">
      <div className="mb-6">
        <button
          onClick={() => navigate('/')}
          className="text-blue-400 hover:text-blue-300"
        >
          ← Back to Dashboard
        </button>
      </div>

      <div className="max-w-4xl mx-auto space-y-6">
        <div className="bg-gray-800 rounded-lg p-6">
          <h1 className="text-3xl font-bold mb-4">{currentSession.name}</h1>
          
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-6">
            <div>
              <div className="text-gray-400 text-sm">Status</div>
              <div className={`font-semibold ${
                currentSession.status === 'Active' ? 'text-green-400' : ''
              }`}>
                {currentSession.status}
              </div>
            </div>
            <div>
              <div className="text-gray-400 text-sm">Laps</div>
              <div className="font-semibold">{currentSession.totalLaps}</div>
            </div>
            <div>
              <div className="text-gray-400 text-sm">Distance</div>
              <div className="font-semibold">
                {(currentSession.totalDistanceMeters / 1000).toFixed(2)} km
              </div>
            </div>
            <div>
              <div className="text-gray-400 text-sm">Points</div>
              <div className="font-semibold">{trackingPoints.length}</div>
            </div>
          </div>

          <div className="space-y-3">
            {!storeIsTracking ? (
              <button
                onClick={handleStartTracking}
                disabled={currentSession.status === 'Completed'}
                className="w-full bg-green-600 hover:bg-green-700 disabled:bg-gray-600 px-6 py-3 rounded-lg font-semibold transition"
              >
                Start Tracking
              </button>
            ) : (
              <button
                onClick={handleStopTracking}
                className="w-full bg-yellow-600 hover:bg-yellow-700 px-6 py-3 rounded-lg font-semibold transition"
              >
                Stop Tracking
              </button>
            )}

            <button
              onClick={handleCompleteSession}
              disabled={currentSession.status === 'Completed'}
              className="w-full bg-red-600 hover:bg-red-700 disabled:bg-gray-600 px-6 py-3 rounded-lg font-semibold transition"
            >
              Complete Session
            </button>
          </div>

          {(gpsError || signalRError) && (
            <div className="mt-4 p-4 bg-red-900/50 border border-red-700 rounded">
              {gpsError && <div>GPS Error: {gpsError}</div>}
              {signalRError && <div>Connection Error: {signalRError.message}</div>}
            </div>
          )}
        </div>

        {position && (
          <div className="bg-gray-800 rounded-lg p-6">
            <h2 className="text-xl font-semibold mb-3">Current Position</h2>
            <div className="grid grid-cols-2 gap-3 text-sm">
              <div>
                <span className="text-gray-400">Latitude:</span>
                <span className="ml-2">{position.coords.latitude.toFixed(6)}</span>
              </div>
              <div>
                <span className="text-gray-400">Longitude:</span>
                <span className="ml-2">{position.coords.longitude.toFixed(6)}</span>
              </div>
              <div>
                <span className="text-gray-400">Speed:</span>
                <span className="ml-2">
                  {position.coords.speed
                    ? (position.coords.speed * 3.6).toFixed(1)
                    : '0.0'}{' '}
                  km/h
                </span>
              </div>
              <div>
                <span className="text-gray-400">Accuracy:</span>
                <span className="ml-2">{position.coords.accuracy.toFixed(1)} m</span>
              </div>
            </div>
          </div>
        )}

        {currentSession.laps.length > 0 && (
          <div className="bg-gray-800 rounded-lg p-6">
            <h2 className="text-xl font-semibold mb-3">Laps</h2>
            <div className="space-y-2">
              {currentSession.laps.map((lap) => (
                <div key={lap.id} className="flex justify-between items-center p-3 bg-gray-700 rounded">
                  <span className="font-semibold">Lap {lap.lapNumber}</span>
                  <div className="text-right">
                    <div>{lap.duration}</div>
                    <div className="text-sm text-gray-400">
                      {lap.maxSpeed.kilometersPerHour.toFixed(1)} km/h max
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
};
