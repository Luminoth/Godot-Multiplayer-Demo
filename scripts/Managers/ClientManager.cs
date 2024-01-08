using Godot;

using System;
using System.Threading;

public partial class ClientManager : SingletonNode<ClientManager>
{
    #region Events

    public event EventHandler ConnectedToServerEvent;
    public event EventHandler ConnectionFailedEvent;
    public event EventHandler ServerDisconnectedEvent;

    public event EventHandler LoadLevelEvent;

    #endregion

    public int UniqueId => Multiplayer.GetUniqueId();

    #region Godot Lifecycle

    public override void _ExitTree()
    {
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

        Multiplayer.ConnectedToServer += OnConnectedToServer;
        Multiplayer.ConnectionFailed += OnConnectionFailed;
        Multiplayer.ServerDisconnected += OnServerDisconnected;
    }

    public void Disconnect()
    {
        GD.Print("Disconnecting from game session ...");

        Multiplayer.ConnectedToServer -= OnConnectedToServer;
        Multiplayer.ConnectionFailed -= OnConnectionFailed;
        Multiplayer.ServerDisconnected -= OnServerDisconnected;

        Multiplayer.MultiplayerPeer = null;
    }

    #region RPC

    [Rpc(TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void LoadLevel()
    {
        GD.Print("server says load level");
        LoadLevelEvent?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Event Handlers

    private void OnConnectedToServer()
    {
        ConnectedToServerEvent?.Invoke(this, EventArgs.Empty);
    }

    private void OnConnectionFailed()
    {
        ConnectionFailedEvent?.Invoke(this, EventArgs.Empty);
    }

    private void OnServerDisconnected()
    {
        GD.Print("Server disconnected!");

        ServerDisconnectedEvent?.Invoke(this, EventArgs.Empty);
    }

    #endregion
}
