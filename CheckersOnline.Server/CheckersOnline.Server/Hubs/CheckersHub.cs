using CheckersOnline.Server.Models;
using Microsoft.AspNetCore.SignalR;

namespace CheckersOnline.Server.Hubs;

public class CheckersHub : Hub
{
    private static Dictionary<string, GameState> Games = new();

    public override Task OnConnectedAsync()
    {
        Console.WriteLine("new client connected.");
        return base.OnConnectedAsync();
    }

    public async Task CreateGame(string gameId)
    {
        Games[gameId] = new GameState();
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
    }

    public async Task JoinGame(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        await Clients.Group(gameId).SendAsync("PlayerJoined");
    }

    public async Task MakeMove(string gameId, Move move)
    {
        var game = Games[gameId];

        if (!game.IsValidMove(move))
            return;

        game.ApplyMove(move);

        await Clients.Group(gameId).SendAsync("MoveMade", move, game);
    }
}
