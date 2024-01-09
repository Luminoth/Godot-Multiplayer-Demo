using Godot;

using System;

public partial class JoiningGame : Node
{
    [Export]
    private PackedScene _mainMenuScene;

    [Export]
    private PackedScene _levelScene;

    #region Godot Lifecycle

    public override void _ExitTree()
    {
        ClientManager.Instance.ConnectedToServerEvent -= OnConnectedToServer;
        ClientManager.Instance.ConnectionFailedEvent -= OnConnectionFailed;
        ClientManager.Instance.LoadLevelEvent -= OnLoadLevel;
    }

    #endregion

    public void JoinLocalGameSession()
    {
        ClientManager.Instance.ConnectedToServerEvent += OnConnectedToServer;
        ClientManager.Instance.ConnectionFailedEvent += OnConnectionFailed;
        ClientManager.Instance.LoadLevelEvent += OnLoadLevel;
        ClientManager.Instance.BeginJoinLocalGameSession();
    }

    public void JoinGameSession(string address)
    {
        ClientManager.Instance.ConnectedToServerEvent += OnConnectedToServer;
        ClientManager.Instance.ConnectionFailedEvent += OnConnectionFailed;
        ClientManager.Instance.LoadLevelEvent += OnLoadLevel;
        ClientManager.Instance.BeginJoinGameSession(address);
    }

    #region Event Handlers

    private void OnConnectedToServer(object sender, EventArgs e)
    {
        ClientManager.Instance.ConnectedToServerEvent -= OnConnectedToServer;
        ClientManager.Instance.ConnectionFailedEvent -= OnConnectionFailed;

        GD.Print("Connected to server!");
    }

    private void OnConnectionFailed(object sender, EventArgs e)
    {
        ClientManager.Instance.ConnectedToServerEvent -= OnConnectedToServer;
        ClientManager.Instance.ConnectionFailedEvent -= OnConnectionFailed;
        ClientManager.Instance.ConnectionFailedEvent -= OnLoadLevel;

        GD.PrintErr($"Failed to connect to server!");

        var scene = _mainMenuScene.Instantiate();
        GetTree().Root.AddChild(scene);

        QueueFree();
    }

    private void OnLoadLevel(object sender, EventArgs args)
    {
        GD.Print("Client loading level ...");

        ClientManager.Instance.LoadLevelEvent -= OnLoadLevel;

        var scene = _levelScene.Instantiate();
        GetTree().Root.AddChild(scene);

        QueueFree();
    }

    #endregion
}
