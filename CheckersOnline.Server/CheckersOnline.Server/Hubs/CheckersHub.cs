using CheckersOnline.Server.Models;
using CheckersOnline.Server.Services;
using Microsoft.AspNetCore.SignalR;
using System.Drawing;
using System.Text.Json;

namespace CheckersOnline.Server.Hubs;

public class CheckersHub : Hub
{
    private const string defaultGroupName = "default";
    private readonly GameEngine _gameEngine;

    public CheckersHub(GameEngine gameEngine)
    {
        _gameEngine = gameEngine;
    }

    public async override Task OnConnectedAsync()
    {
        Console.WriteLine("New client connected.");
        await Groups.AddToGroupAsync(Context.ConnectionId, defaultGroupName);
        _gameEngine.currentGroupPlayers.Add(Context.ConnectionId);

        if (_gameEngine.currentGroupPlayers.Count == 2)
        {
            await SetupGame();
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var id = Context.ConnectionId;
        _gameEngine.currentGroupPlayers.Remove(Context.ConnectionId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, defaultGroupName);

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SetupGame()
    {
        _gameEngine.StartNewGame(_gameEngine.currentGame);
        var players = _gameEngine.SetPlayersAndTurn(_gameEngine.currentGame);

        foreach (var player in players)
        {
            await Clients.Client(player.Key).SendAsync("SetColor", player.Value);
        }

        await Clients.Group(defaultGroupName).SendAsync("GameStarted", _gameEngine.currentGame);
    }


    public async Task MakeMove(Move move)
    {
        if (!_gameEngine.TryApplyMove(_gameEngine.currentGame, move))
            return;

        _gameEngine.MovePiece(_gameEngine.currentGame, move);

        await Clients.Group(defaultGroupName).SendAsync("MoveMade", _gameEngine.currentGame);
    }
}
