using Godot;

using System;

public partial class JoiningGame : Node
{
    [Export]
    private PackedScene _mainMenuScene;

    public void JoinLocalGameSession()
    {
        ClientManager.Instance.ConnectedToServerEvent += OnConnectedToServer;
        ClientManager.Instance.ConnectionFailedEvent += OnConnectionFailed;
        ClientManager.Instance.BeginJoinLocalGameSession();
    }

    public void JoinGameSession(string address)
    {
        ClientManager.Instance.ConnectedToServerEvent += OnConnectedToServer;
        ClientManager.Instance.ConnectionFailedEvent += OnConnectionFailed;
        ClientManager.Instance.BeginJoinGameSession(address);
    }

    #region Event Handlers

    private void OnConnectedToServer(object sender, EventArgs e)
    {
        ClientManager.Instance.ConnectedToServerEvent -= OnConnectedToServer;
        ClientManager.Instance.ConnectionFailedEvent -= OnConnectionFailed;

        GD.Print("Connected to server!");

        QueueFree();
    }

    private void OnConnectionFailed(object sender, EventArgs e)
    {
        ClientManager.Instance.ConnectedToServerEvent -= OnConnectedToServer;
        ClientManager.Instance.ConnectionFailedEvent -= OnConnectionFailed;

        GD.PrintErr($"Failed to connect to server!");

        var scene = _mainMenuScene.Instantiate();
        GetTree().Root.AddChild(scene);

        QueueFree();
    }

    #endregion
}
