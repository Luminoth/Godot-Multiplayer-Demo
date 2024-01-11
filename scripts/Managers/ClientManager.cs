using Godot;

using System;

public partial class ClientManager : SingletonNode<ClientManager>
{
    #region Events

    public event EventHandler ConnectedToServerEvent;
    public event EventHandler ConnectionFailedEvent;
    public event EventHandler ServerDisconnectedEvent;

    #region RPC Events

    public event EventHandler LoadLevelEvent;

    #endregion

    #endregion

    public int UniqueId => Multiplayer.MultiplayerPeer == null ? 0 : Multiplayer.GetUniqueId();

    #region Godot Lifecycle

    public override void _EnterTree()
    {
        base._EnterTree();

        // give the client it's own API to make RPCs work with the custom API server
        // per https://github.com/godotengine/godot/issues/80604 custom multiplayer should be set in _enter_tree
        GD.Print($"Creating custom client multiplayer API at {GetPath()}");
        var api = MultiplayerApi.CreateDefaultInterface();
        api.MultiplayerPeer = null; // clear the default "offline" peer
        GetTree().SetMultiplayer(api, GetPath());
    }

    public override void _ExitTree()
    {
        Disconnect();

        base._ExitTree();
    }

    #endregion

    public void BeginJoinLocalGameSession()
    {
        BeginJoinGameSession(GameManager.Instance.DefaultAddress);
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

    public void Disconnect(bool silent = false)
    {
        if(Multiplayer.MultiplayerPeer == null) {
            return;
        }

        if(!silent) {
            GD.Print("Disconnecting from game session ...");
        }

        Multiplayer.ConnectedToServer -= OnConnectedToServer;
        Multiplayer.ConnectionFailed -= OnConnectionFailed;
        Multiplayer.ServerDisconnected -= OnServerDisconnected;

        Multiplayer.MultiplayerPeer = null;
    }

    // NOTE: RPC definitions duplicated in ServerManager.cs

    #region Client RPC

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void LevelLoaded(long id)
    {
        GD.PrintErr($"Client received level loaded from {id}");
    }

    #endregion

    #region Server RPC

    [Rpc(TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void LoadLevel()
    {
        GD.Print("Server says load level");

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

        Disconnect(true);
    }

    #endregion
}
