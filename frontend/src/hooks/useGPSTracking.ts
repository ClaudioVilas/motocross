import { useState, useEffect, useCallback } from 'react';
import type { Coordinate } from '../types/session.types';

interface GeolocationPosition {
  coords: {
    latitude: number;
    longitude: number;
    altitude: number | null;
    accuracy: number;
    altitudeAccuracy: number | null;
    heading: number | null;
    speed: number | null; // meters per second
  };
  timestamp: number;
}

interface UseGPSTrackingOptions {
  enableHighAccuracy?: boolean;
  timeout?: number;
  maximumAge?: number;
}

export function useGPSTracking(options: UseGPSTrackingOptions = {}) {
  const [position, setPosition] = useState<GeolocationPosition | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isAvailable, setIsAvailable] = useState(false);
  const [isTracking, setIsTracking] = useState(false);

  useEffect(() => {
    setIsAvailable('geolocation' in navigator);
  }, []);

  const startTracking = useCallback(() => {
    if (!isAvailable) {
      setError('Geolocation is not available in this browser');
      return null;
    }

    setIsTracking(true);
    setError(null);

    const watchId = navigator.geolocation.watchPosition(
      (pos) => {
        setPosition(pos as GeolocationPosition);
        setError(null);
      },
      (err) => {
        setError(err.message);
        console.error('Geolocation error:', err);
      },
      {
        enableHighAccuracy: options.enableHighAccuracy ?? true,
        timeout: options.timeout ?? 5000,
        maximumAge: options.maximumAge ?? 0,
      }
    );

    return watchId;
  }, [isAvailable, options]);

  const stopTracking = useCallback((watchId: number | null) => {
    if (watchId !== null) {
      navigator.geolocation.clearWatch(watchId);
    }
    setIsTracking(false);
  }, []);

  const getCurrentPosition = useCallback(async (): Promise<Coordinate | null> => {
    if (!isAvailable) {
      setError('Geolocation is not available');
      return null;
    }

    return new Promise((resolve) => {
      navigator.geolocation.getCurrentPosition(
        (pos) => {
          resolve({
            latitude: pos.coords.latitude,
            longitude: pos.coords.longitude,
          });
        },
        (err) => {
          setError(err.message);
          resolve(null);
        },
        {
          enableHighAccuracy: true,
          timeout: 10000,
          maximumAge: 0,
        }
      );
    });
  }, [isAvailable]);

  return {
    position,
    error,
    isAvailable,
    isTracking,
    startTracking,
    stopTracking,
    getCurrentPosition,
  };
}
