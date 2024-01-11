using Godot;

using System;

public partial class JoiningGame : Node
{
    private bool _joiningLocal;

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
        _joiningLocal = true;

        ClientManager.Instance.ConnectedToServerEvent += OnConnectedToServer;
        ClientManager.Instance.ConnectionFailedEvent += OnConnectionFailed;
        ClientManager.Instance.LoadLevelEvent += OnLoadLevel;
        ClientManager.Instance.BeginJoinLocalGameSession();
    }

    public void JoinGameSession(string address)
    {
        _joiningLocal = false;

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

        var scene = UIManager.Instance.MainMenuScene.Instantiate();
        GetTree().Root.AddChild(scene);

        QueueFree();
    }

    private void OnLoadLevel(object sender, EventArgs args)
    {
        ClientManager.Instance.LoadLevelEvent -= OnLoadLevel;

        if(_joiningLocal) {
            // TODO: duplicated in Level.cs
            GD.Print("Client signaling level loaded");
            ClientManager.Instance.Rpc(nameof(ClientManager.Instance.LevelLoaded), ClientManager.Instance.UniqueId);
        } else {
            GD.Print("Client loading level ...");

            var scene = GameManager.Instance.LevelScene.Instantiate();
            ClientManager.Instance.AddChild(scene);
        }

        QueueFree();
    }

    #endregion
}
