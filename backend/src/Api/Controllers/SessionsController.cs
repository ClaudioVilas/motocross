using Microsoft.AspNetCore.Mvc;
using Motocross.Application.Commands;
using Motocross.Application.DTOs;
using Motocross.Application.Interfaces;
using Motocross.Application.Queries;

namespace Motocross.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly ISessionService _sessionService;
    private readonly ILogger<SessionsController> _logger;

    public SessionsController(
        ISessionService sessionService,
        ILogger<SessionsController> _logger)
    {
        _sessionService = sessionService;
        this._logger = _logger;
    }

    /// <summary>
    /// Get all sessions with pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<SessionSummaryDto>>> GetSessions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetSessionHistoryQuery(pageNumber, pageSize);
        var sessions = await _sessionService.GetSessionHistoryAsync(query);
        return Ok(sessions);
    }

    /// <summary>
    /// Get active session
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<SessionDto>> GetActiveSession()
    {
        var session = await _sessionService.GetActiveSessionAsync();
        
        if (session == null)
            return NotFound(new { message = "No active session found" });

        return Ok(session);
    }

    /// <summary>
    /// Get session by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SessionDto>> GetSession(Guid id)
    {
        var query = new GetSessionQuery(id);
        var session = await _sessionService.GetSessionAsync(query);

        if (session == null)
            return NotFound(new { message = $"Session {id} not found" });

        return Ok(session);
    }

    /// <summary>
    /// Create a new session
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SessionDto>> CreateSession([FromBody] CreateSessionCommand command)
    {
        var session = await _sessionService.CreateSessionAsync(command);
        return CreatedAtAction(nameof(GetSession), new { id = session.Id }, session);
    }

    /// <summary>
    /// Start a session
    /// </summary>
    [HttpPost("{id}/start")]
    public async Task<ActionResult<SessionDto>> StartSession(Guid id)
    {
        var command = new StartSessionCommand(id);
        var session = await _sessionService.StartSessionAsync(command);
        return Ok(session);
    }

    /// <summary>
    /// Complete a session
    /// </summary>
    [HttpPost("{id}/complete")]
    public async Task<ActionResult<SessionDto>> CompleteSession(Guid id)
    {
        var command = new CompleteSessionCommand(id);
        var session = await _sessionService.CompleteSessionAsync(command);
        return Ok(session);
    }

    /// <summary>
    /// Set start/finish line for a session
    /// </summary>
    [HttpPost("{id}/start-finish-line")]
    public async Task<IActionResult> SetStartFinishLine(
        Guid id,
        [FromBody] SetStartFinishLineCommand command)
    {
        command = command with { SessionId = id };
        await _sessionService.SetStartFinishLineAsync(command);
        return NoContent();
    }

    /// <summary>
    /// Record a tracking point
    /// </summary>
    [HttpPost("{id}/tracking-points")]
    public async Task<IActionResult> RecordTrackingPoint(
        Guid id,
        [FromBody] RecordTrackingPointCommand command)
    {
        command = command with { SessionId = id };
        await _sessionService.RecordTrackingPointAsync(command);
        return NoContent();
    }

    /// <summary>
    /// Get laps for a session
    /// </summary>
    [HttpGet("{id}/laps")]
    public async Task<ActionResult<List<LapDto>>> GetSessionLaps(Guid id)
    {
        var query = new GetSessionLapsQuery(id);
        var laps = await _sessionService.GetSessionLapsAsync(query);
        return Ok(laps);
    }
}
