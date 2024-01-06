using Godot;

using System;

public partial class ClientManager : SingletonNode<ClientManager>
{
    #region Events

    public event EventHandler ConnectedToServer;
    public event EventHandler ConnectionFailed;
    public event EventHandler ServerDisconnected;

    #endregion

    #region Godot Lifecycle

    public override void _Ready()
    {
        Multiplayer.ConnectedToServer += OnConnectedToServer;
        Multiplayer.ConnectionFailed += OnConnectionFailed;
        Multiplayer.ServerDisconnected += OnServerDisconnected;
    }

    public override void _ExitTree()
    {
        Multiplayer.ConnectedToServer -= OnConnectedToServer;
        Multiplayer.ConnectionFailed -= OnConnectionFailed;
        Multiplayer.ServerDisconnected -= OnServerDisconnected;

        Disconnect();
    }

    #endregion

    public void BeginJoinLocalGameSession()
    {
        BeginJoinGameSession(EngineManager.Instance.DefaultAddress);
    }

    public void BeginJoinGameSession(string address)
    {
        var parts = address.Split(':');
        BeginJoinGameSession(parts[0], int.Parse(parts[1]));
    }

    public void BeginJoinGameSession(string address, int port)
    {
        GD.Print($"Joining game session at {address}:{port} ...");

        var peer = new ENetMultiplayerPeer();

        var result = peer.CreateClient(address, port);
        if(result != Error.Ok) {
            GD.PrintErr($"Failed to create client: {result}");
            OnConnectionFailed();
            return;
        }

        Multiplayer.MultiplayerPeer = peer;
    }

    public void Disconnect()
    {
        Multiplayer.MultiplayerPeer = null;
    }

    #region Event Handlers

    private void OnConnectedToServer()
    {
        ConnectedToServer?.Invoke(this, EventArgs.Empty);
    }

    private void OnConnectionFailed()
    {
        ConnectionFailed?.Invoke(this, EventArgs.Empty);
    }

    private void OnServerDisconnected()
    {
        GD.Print("Server disconnected.!");

        ServerDisconnected?.Invoke(this, EventArgs.Empty);
    }

    #endregion
}
