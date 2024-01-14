using System;

using Godot;

public partial class GameManager : SingletonNode<GameManager>
{
    [Export]
    private int _listeningPort = 7777;

    public int ListenPort => _listeningPort;

    public string DefaultAddress => $"127.0.0.1:{ListenPort}";

    [Export]
    private int _maxPlayers = 4;

    public int MaxPlayers => _maxPlayers;

    [Export]
    private PackedScene _levelScene;

    public PackedScene LevelScene => _levelScene;

    [Export]
    private PackedScene _playerScene;

    public PackedScene PlayerScene => _playerScene;

    #region Godot Lifecycle

    public override void _ExitTree()
    {
        ServerManager.Instance.ServerStartedEvent -= OnServerStarted;
        ServerManager.Instance.PeerConnectedEvent -= OnPeerConnected;

        base._ExitTree();
    }

    #endregion

    public bool StartLocalServer()
    {
        ServerManager.Instance.ServerStartedEvent += OnServerStarted;

        if(!ServerManager.Instance.StartLocalServer(ListenPort, MaxPlayers)) {
            GD.PrintErr("Failed to start local game server");
            return false;
        }

        return true;
    }

    public bool StartDedicatedServer(bool useGameLift)
    {
        ServerManager.Instance.ServerStartedEvent += OnServerStarted;

        if(!ServerManager.Instance.StartDedicatedServer(ListenPort, MaxPlayers, useGameLift)) {
            GD.PrintErr("Failed to start dedicated game server");
            return false;
        }

        return true;
    }

    #region Event Handlers

    private void OnServerStarted(object sender, EventArgs args)
    {
        ServerManager.Instance.ServerStartedEvent -= OnServerStarted;
        ServerManager.Instance.PeerConnectedEvent += OnPeerConnected;

        GD.Print("Server started, loading level ...");

        var scene = _levelScene.Instantiate();
        //ServerManager.Instance.AddChild(scene);
        GetTree().Root.AddChild(scene);
    }

    private void OnPeerConnected(object sender, ServerManager.PeerEventArgs e)
    {
        GD.Print($"Client {e.Id} connected, signaling load level");

        ServerManager.Instance.RpcId(e.Id, nameof(ServerManager.Instance.LoadLevel));
    }

    #endregion
}
