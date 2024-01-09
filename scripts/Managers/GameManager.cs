using Godot;

using System;

public partial class GameManager : SingletonNode<GameManager>
{
    #region Events

    public event EventHandler LoadLevelEvent;

    #endregion

    [Export]
    private int _listeningPort = 7777;

    public int ListenPort => _listeningPort;

    public string DefaultAddress => $"127.0.0.1:{ListenPort}";

    [Export]
    private int _maxPlayers = 4;

    public int MaxPlayers => _maxPlayers;

    [Export]
    private PackedScene _levelScene;

    #region Godot Lifecycle

    public override void _ExitTree()
    {
        ServerManager.Instance.PeerConnectedEvent -= OnPeerConnected;
    }

    #endregion

    public bool StartLocalServer()
    {
        if(!ServerManager.Instance.StartLocalServer(ListenPort, MaxPlayers)) {
            GD.PrintErr("Failed to start local game server");
            return false;
        }

        FinishStartServer();

        return true;
    }

    public bool StartDedicatedServer(bool useGameLift)
    {
        if(!ServerManager.Instance.StartDedicatedServer(ListenPort, MaxPlayers, useGameLift)) {
            GD.PrintErr("Failed to start dedicated game server");
            return false;
        }

        FinishStartServer();

        return true;
    }

    private void FinishStartServer()
    {
        ServerManager.Instance.PeerConnectedEvent += OnPeerConnected;

        var scene = _levelScene.Instantiate();
        GetTree().Root.AddChild(scene);
    }

    #region Client RPC

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void LevelLoaded(long id)
    {
        GD.Print($"Client {id} says level loaded");
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

    private void OnPeerConnected(object sender, ServerManager.PeerEventArgs e)
    {
        GD.Print($"Peer {e.Id} connected, am authority: {IsMultiplayerAuthority()}");
        if(IsMultiplayerAuthority()) {
            GD.Print("rpcing to client");
            RpcId(e.Id, nameof(LoadLevel));
        }
    }

    #endregion
}
