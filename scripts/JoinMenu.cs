using Godot;

using System;

public partial class JoinMenu : Node
{
    [Export]
    private PackedScene _levelScene;

    private LineEdit _addressInput;

    public override void _Ready()
    {
        _addressInput = GetNode<LineEdit>("VBoxContainer/HBoxContainer/LineEdit");
        _addressInput.PlaceholderText = EngineManager.Instance.DefaultAddress;
    }

    #region Signals

    private void _on_join_pressed()
    {
        var address = _addressInput.Text;
        if(string.IsNullOrWhiteSpace(address)) {
            address = EngineManager.Instance.DefaultAddress;
        }

        ClientManager.Instance.ConnectedToServer += OnConnectedToServer;
        ClientManager.Instance.ConnectionFailed += OnConnectionFailed;
        ClientManager.Instance.BeginJoinGameSession(address);
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
    }

    #endregion
}
