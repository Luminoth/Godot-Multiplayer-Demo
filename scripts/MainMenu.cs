using Godot;

using System;

public partial class MainMenu : Node
{
    [Export]
    private PackedScene _levelScene;

    [Export]
    private PackedScene _joinGameScene;

    #region Signals

    private void _on_create_pressed()
    {
        GD.Print("Creating game session ...");

        if(!ServerManager.Instance.StartLocalServer()) {
            GD.PrintErr("Failed to start local game server");
            return;
        }

        ClientManager.Instance.ConnectedToServer += OnConnectedToServer;
        ClientManager.Instance.ConnectionFailed += OnConnectionFailed;
        ClientManager.Instance.BeginJoinLocalGameSession();

        // TODO: disable buttons
    }

    private void _on_join_pressed()
    {
        var scene = _joinGameScene.Instantiate();
        GetTree().Root.AddChild(scene);

        QueueFree();
    }

    #endregion

    #region Event Handlers

    private void OnConnectedToServer(object sender, EventArgs e)
    {
        ClientManager.Instance.ConnectedToServer -= OnConnectedToServer;
        ClientManager.Instance.ConnectionFailed -= OnConnectionFailed;

        GD.Print("Connected to server!");

        var scene = _levelScene.Instantiate();
        GetTree().Root.AddChild(scene);

        QueueFree();
    }

    private void OnConnectionFailed(object sender, EventArgs e)
    {
        ClientManager.Instance.ConnectedToServer -= OnConnectedToServer;
        ClientManager.Instance.ConnectionFailed -= OnConnectionFailed;

        GD.PrintErr($"Failed to connect to server!");

        // TODO: enable buttons
    }

    #endregion
}
