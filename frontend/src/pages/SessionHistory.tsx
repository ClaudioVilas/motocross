import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { apiClient } from '../services/api.service';

export const SessionHistory: React.FC = () => {
  const navigate = useNavigate();

  const { data: sessions, isLoading } = useQuery({
    queryKey: ['sessions'],
    queryFn: () => apiClient.getSessions(1, 50),
  });

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-xl">Loading history...</div>
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

      <div className="max-w-4xl mx-auto">
        <h1 className="text-3xl font-bold mb-6">Session History</h1>

        {!sessions || sessions.length === 0 ? (
          <div className="bg-gray-800 rounded-lg p-8 text-center text-gray-400">
            No sessions yet
          </div>
        ) : (
          <div className="space-y-3">
            {sessions.map((session) => (
              <div
                key={session.id}
                onClick={() => navigate(`/session/${session.id}`)}
                className="bg-gray-800 hover:bg-gray-700 rounded-lg p-5 cursor-pointer transition"
              >
                <div className="flex justify-between items-start mb-2">
                  <h3 className="text-xl font-semibold">{session.name}</h3>
                  <span
                    className={`px-3 py-1 rounded-full text-sm ${
                      session.status === 'Completed'
                        ? 'bg-green-600'
                        : session.status === 'Active'
                        ? 'bg-blue-600'
                        : 'bg-gray-600'
                    }`}
                  >
                    {session.status}
                  </span>
                </div>

                <div className="grid grid-cols-3 gap-4 text-sm">
                  <div>
                    <div className="text-gray-400">Start</div>
                    <div>{new Date(session.startTime).toLocaleDateString()}</div>
                  </div>
                  <div>
                    <div className="text-gray-400">Laps</div>
                    <div className="font-semibold">{session.totalLaps}</div>
                  </div>
                  <div>
                    <div className="text-gray-400">Duration</div>
                    <div>
                      {session.endTime
                        ? `${Math.round(
                            (new Date(session.endTime).getTime() -
                              new Date(session.startTime).getTime()) /
                              60000
                          )} min`
                        : 'In progress'}
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};
