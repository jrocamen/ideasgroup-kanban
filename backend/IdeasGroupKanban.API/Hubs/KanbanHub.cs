using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace IdeasGroupKanban.API.Hubs;

[Authorize]
public class KanbanHub : Hub
{
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> _connectionUserMap = new();
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, HashSet<string>> _projectConnections = new();

    public async Task SubscribeToProject(string projectId, string userName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Project_{projectId}");
        _connectionUserMap[Context.ConnectionId] = userName;
        
        var connections = _projectConnections.GetOrAdd(projectId, _ => new HashSet<string>());
        lock (connections) 
        { 
            connections.Add(Context.ConnectionId); 
        }

        await BroadcastProjectUsers(projectId);
    }

    public async Task UnsubscribeFromProject(string projectId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Project_{projectId}");
        
        if (_projectConnections.TryGetValue(projectId, out var connections))
        {
            lock (connections)
            {
                connections.Remove(Context.ConnectionId);
            }
            await BroadcastProjectUsers(projectId);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (_connectionUserMap.TryRemove(Context.ConnectionId, out _))
        {
            foreach (var kvp in _projectConnections)
            {
                bool removed = false;
                lock (kvp.Value)
                {
                    removed = kvp.Value.Remove(Context.ConnectionId);
                }
                if (removed)
                {
                    await BroadcastProjectUsers(kvp.Key);
                }
            }
        }
        await base.OnDisconnectedAsync(exception);
    }

    private async Task BroadcastProjectUsers(string projectId)
    {
        if (_projectConnections.TryGetValue(projectId, out var connections))
        {
            List<string> userNames;
            lock (connections)
            {
                userNames = connections.Select(c => _connectionUserMap.TryGetValue(c, out var name) ? name : null)
                                     .Where(n => n != null)
                                     .Distinct()
                                     .ToList()!;
            }
            await Clients.Group($"Project_{projectId}").SendAsync("ActiveUsersUpdated", userNames);
        }
    }
}
