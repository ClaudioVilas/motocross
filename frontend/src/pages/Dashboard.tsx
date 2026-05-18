import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { apiClient } from '../services/api.service';
import type { CreateSessionCommand } from '../types/session.types';

export const Dashboard: React.FC = () => {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [sessionName, setSessionName] = useState('');
  const [showCreateForm, setShowCreateForm] = useState(false);

  const { data: activeSession, isLoading } = useQuery({
    queryKey: ['active-session'],
    queryFn: () => apiClient.getActiveSession(),
    refetchInterval: 5000,
  });

  const createSessionMutation = useMutation({
    mutationFn: (command: CreateSessionCommand) => apiClient.createSession(command),
    onSuccess: async (session) => {
      await queryClient.invalidateQueries({ queryKey: ['active-session'] });
      navigate(`/session/${session.id}`);
    },
  });

  const handleCreateSession = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!sessionName.trim()) return;

    createSessionMutation.mutate({
      name: sessionName,
      description: `Training session created on ${new Date().toLocaleDateString()}`,
    });
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-xl">Loading...</div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="max-w-2xl mx-auto">
        <h1 className="text-4xl font-bold mb-8 text-center">
          🏁 Motocross Tracking
        </h1>

        {activeSession ? (
          <div className="bg-gray-800 rounded-lg p-6 shadow-lg">
            <h2 className="text-2xl font-semibold mb-4">Active Session</h2>
            <div className="space-y-3">
              <div>
                <span className="text-gray-400">Name:</span>
                <span className="ml-2 font-medium">{activeSession.name}</span>
              </div>
              <div>
                <span className="text-gray-400">Status:</span>
                <span className="ml-2">
                  <span className={`px-3 py-1 rounded-full text-sm ${
                    activeSession.status === 'Active'
                      ? 'bg-green-600'
                      : 'bg-gray-600'
                  }`}>
                    {activeSession.status}
                  </span>
                </span>
              </div>
              <div>
                <span className="text-gray-400">Laps:</span>
                <span className="ml-2 font-medium">{activeSession.totalLaps}</span>
              </div>
              <div>
                <span className="text-gray-400">Distance:</span>
                <span className="ml-2 font-medium">
                  {(activeSession.totalDistanceMeters / 1000).toFixed(2)} km
                </span>
              </div>
            </div>
            <button
              onClick={() => navigate(`/session/${activeSession.id}`)}
              className="mt-6 w-full bg-blue-600 hover:bg-blue-700 px-6 py-3 rounded-lg font-semibold transition"
            >
              View Session
            </button>
          </div>
        ) : (
          <div className="space-y-6">
            <div className="bg-gray-800 rounded-lg p-8 shadow-lg text-center">
              <p className="text-gray-300 mb-6">No active session</p>
              
              {!showCreateForm ? (
                <button
                  onClick={() => setShowCreateForm(true)}
                  className="bg-blue-600 hover:bg-blue-700 px-8 py-3 rounded-lg font-semibold transition"
                >
                  Create New Session
                </button>
              ) : (
                <form onSubmit={handleCreateSession} className="space-y-4">
                  <input
                    type="text"
                    value={sessionName}
                    onChange={(e) => setSessionName(e.target.value)}
                    placeholder="Session name"
                    className="w-full px-4 py-3 bg-gray-700 rounded-lg border border-gray-600 focus:border-blue-500 focus:outline-none"
                    autoFocus
                  />
                  <div className="flex gap-3">
                    <button
                      type="submit"
                      disabled={!sessionName.trim() || createSessionMutation.isPending}
                      className="flex-1 bg-blue-600 hover:bg-blue-700 disabled:bg-gray-600 px-6 py-3 rounded-lg font-semibold transition"
                    >
                      {createSessionMutation.isPending ? 'Creating...' : 'Create'}
                    </button>
                    <button
                      type="button"
                      onClick={() => setShowCreateForm(false)}
                      className="flex-1 bg-gray-700 hover:bg-gray-600 px-6 py-3 rounded-lg font-semibold transition"
                    >
                      Cancel
                    </button>
                  </div>
                </form>
              )}
            </div>

            <button
              onClick={() => navigate('/history')}
              className="w-full bg-gray-800 hover:bg-gray-700 px-6 py-3 rounded-lg font-semibold transition"
            >
              View Session History
            </button>
          </div>
        )}
      </div>
    </div>
  );
};
