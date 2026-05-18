using Microsoft.AspNetCore.SignalR;
using Motocross.Application.DTOs;
using Motocross.Application.Interfaces;

namespace Motocross.Infrastructure.Realtime;

public class TrackingHub : Hub
{
    public async Task JoinSession(string sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        await Clients.Caller.SendAsync("JoinedSession", sessionId);
    }

    public async Task LeaveSession(string sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
        await Clients.Caller.SendAsync("LeftSession", sessionId);
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}

public class RealtimeTrackingPublisher : IRealtimePublisher
{
    private readonly IHubContext<TrackingHub> _hubContext;

    public RealtimeTrackingPublisher(IHubContext<TrackingHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task PublishTrackingUpdateAsync(Guid sessionId, TrackingPointDto trackingPoint)
    {
        await _hubContext.Clients
            .Group(sessionId.ToString())
            .SendAsync("ReceiveTrackingUpdate", trackingPoint);
    }

    public async Task PublishLapCompletedAsync(Guid sessionId, LapDto lap)
    {
        await _hubContext.Clients
            .Group(sessionId.ToString())
            .SendAsync("ReceiveLapCompleted", lap);
    }

    public async Task PublishSessionStatusChangedAsync(Guid sessionId, string status)
    {
        await _hubContext.Clients
            .Group(sessionId.ToString())
            .SendAsync("ReceiveSessionStatusChanged", status);
    }
}
