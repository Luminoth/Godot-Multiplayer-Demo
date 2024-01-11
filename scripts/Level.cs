using Godot;

using System.Collections.Generic;

public partial class Level : Node
{
    [Export]
    private PackedScene _playerScene;

    [Export]
    private Node _spawnRoot;

    private Dictionary<long, Player> _players = new Dictionary<long, Player>();

    #region Godot Lifecycle

    public override void _Ready()
    {
        GD.Print("Level ready!");
        GD.Print($"Client ID: {ClientManager.Instance.UniqueId}");
        GD.Print($"Server Is Server: {ServerManager.Instance.IsServer}");
        GD.Print($"Is Dedicated Server: {ServerManager.Instance.IsDedicatedServer}");
        GD.Print($"Is Multiplayer Authority: {IsMultiplayerAuthority()}");

        if(ServerManager.Instance.IsServer) {
            ServerManager.Instance.PeerDisconnectedEvent += OnPeerDisconnected;
            ServerManager.Instance.LevelLoadedEvent += OnPeerLevelLoaded;
        } else {
            // TODO: duplicated in JoiningGame.cs
            GD.Print("Client signaling level loaded");
            ClientManager.Instance.Rpc(nameof(ClientManager.Instance.LevelLoaded), ClientManager.Instance.UniqueId);
        }
    }

    public override void _ExitTree()
    {
        ServerManager.Instance.PeerDisconnectedEvent -= OnPeerDisconnected;
        ServerManager.Instance.LevelLoadedEvent -= OnPeerLevelLoaded;
    }

    #endregion

    #region Event Handlers

    private void OnPeerDisconnected(object sender, ServerManager.PeerEventArgs e)
    {
        GD.Print($"Client {e.Id} disconnected, despawning ...");

        if(_players.Remove(e.Id, out Player player)) {
            player.QueueFree();
        }
    }

    private void OnPeerLevelLoaded(object sender, ServerManager.PeerEventArgs e)
    {
        GD.Print($"Client {e.Id} level loaded, spawning ...");

        var player = _playerScene.Instantiate<Player>();
        player.Name = $"Player {e.Id}";
        player.ClientId = e.Id;
        _spawnRoot.AddChild(player);
        _players.Add(e.Id, player);
    }

    #endregion
}
