using Godot;

using System;
using System.Collections.Generic;
using System.IO;

using Aws.GameLift.Server;
using Aws.GameLift.Server.Model;

public partial class ServerManager : SingletonNode<ServerManager>
{
    public class PeerEventArgs : EventArgs
    {
        public long Id { get; set; }
    }

    #region Events

    public event EventHandler<PeerEventArgs> PeerConnectedEvent;
    public event EventHandler<PeerEventArgs> PeerDisconnectedEvent;

    public event EventHandler<PeerEventArgs> LevelLoadedEvent;

    #endregion

    [Export]
    private ENetConnection.CompressionMode _compressionMode = ENetConnection.CompressionMode.RangeCoder;

    public bool IsServer => Multiplayer.MultiplayerPeer == null ? false : Multiplayer.IsServer();

    private bool _isDedicatedServer;

    public bool IsDedicatedServer => _isDedicatedServer;

    private bool _isGameLiftServer;

    #region Godot Lifecycle

    public override void _EnterTree()
    {
        base._EnterTree();

        // give the server it's own API so we can run a parallel client
        // per https://github.com/godotengine/godot/issues/80604 custom multiplayer should be set in _enter_tree
        GD.Print($"Creating custom server multiplayer API at {GetPath()}");
        var api = MultiplayerApi.CreateDefaultInterface();
        api.MultiplayerPeer = null; // clear the default "offline" peer
        GetTree().SetMultiplayer(api, GetPath());
    }

    public override void _ExitTree()
    {
        StopServer();

        base._ExitTree();
    }

    #endregion

    public bool StartDedicatedServer(int port, int maxPlayers, bool useGameLift)
    {
        GD.Print("Starting dedicated game server ...");

        if(useGameLift) {
            return StartGameLiftServer(port, maxPlayers);
        } else {
            return StartServer(port, maxPlayers, true);
        }
    }

    private bool StartGameLiftServer(int port, int maxPlayers)
    {
        GD.Print("Starting GameLift game server ...");

        // TODO: pass all this garbage in and ONLY if running with Anywhere
        var webSocketUrl = "wss://us-west-2.api.amazongamelift.com";
        var processId = "myProcess";
        var hostId = "myHost";
        var fleetId = "myFleet";
        var authToken = "myAuthToken";
        var serverParameters = new ServerParameters(webSocketUrl, processId, hostId, fleetId, authToken);

        var initSDKOutcome = GameLiftServerAPI.InitSDK(serverParameters);
        if(initSDKOutcome.Success) {
            GD.Print("InitSDK success, readying ...");

            _isGameLiftServer = true;

            var logPath = Path.GetDirectoryName(EngineManager.Instance.LogPath);
            GD.Print($"Adding log path {logPath}");

            var processParameters = new ProcessParameters(
                // OnStartGameSession
                (GameSession gameSession) => {
                    GD.Print("OnStartGameSession");

                    // TODO: what if this fails?
                    StartServer(port, maxPlayers, true);

                    // TODO: need to setup server callbacks

                    GameLiftServerAPI.ActivateGameSession();
                },
                // OnUpdateGameSession
                (UpdateGameSession updateGameSession) => {
                    GD.Print("OnUpdateGameSession");
                },
                // OnProcessTerminate
                () => {
                    GD.Print("OnProcessTerminate");

                    GameLiftServerAPI.ProcessEnding();

                    EngineManager.Instance.Quit();
                },
                // OnHealthCheck
                () => {
                    GD.Print("OnHealthCheck");
                    return true;
                },
                port,
                new LogParameters(new List<string>() {
                    logPath,
                })
            );

            var processReadyOutcome = GameLiftServerAPI.ProcessReady(processParameters);
            if(processReadyOutcome.Success) {
                GD.Print("ProcessReady success.");
                return true;
            } else {
                GD.Print("ProcessReady failure : " + processReadyOutcome.Error.ToString());
                return false;
            }
        } else {
            GD.PrintErr("InitSDK failure : " + initSDKOutcome.Error.ToString());
            return false;
        }
    }

    public bool StartLocalServer(int port, int maxPlayers)
    {
        GD.Print($"Starting local game server ...");

        return StartServer(port, maxPlayers, false);
    }

    private bool StartServer(int port, int maxPlayers, bool isDedicatedServer)
    {
        GD.Print($"Starting game server on {port} ...");

        var peer = new ENetMultiplayerPeer();

        var result = peer.CreateServer(port, maxPlayers);
        if(result != Error.Ok) {
            GD.PrintErr($"Failed to create server: {result}");
            return false;
        }

        peer.Host.Compress(_compressionMode);

        Multiplayer.MultiplayerPeer = peer;

        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;

        _isDedicatedServer = isDedicatedServer;

        return true;
    }

    public void StopServer()
    {
        if(Multiplayer.MultiplayerPeer == null) {
            return;
        }

        GD.Print("Stopping game server ...");

        _isDedicatedServer = false;

        Multiplayer.PeerConnected -= OnPeerConnected;
        Multiplayer.PeerDisconnected -= OnPeerDisconnected;

        Multiplayer.MultiplayerPeer = null;

        if(_isGameLiftServer) {
            GameLiftServerAPI.Destroy();
            _isGameLiftServer = false;
        }
    }

    // NOTE: RPC definitions duplicated in ClientManager.cs

    #region Client RPC

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void LevelLoaded(long id)
    {
        GD.Print($"Client {id} says level loaded");

        LevelLoadedEvent?.Invoke(this, new PeerEventArgs { Id = id });
    }

    #endregion

    #region Server RPC

    [Rpc(TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void LoadLevel()
    {
        GD.PrintErr("Server received load level");
    }

    #endregion

    #region Event Handlers

    private void OnPeerConnected(long id)
    {
        GD.Print($"Peer {id} connected");

        PeerConnectedEvent?.Invoke(this, new PeerEventArgs { Id = id });
    }

    private void OnPeerDisconnected(long id)
    {
        GD.Print($"Peer {id} disconnected");

        PeerDisconnectedEvent?.Invoke(this, new PeerEventArgs { Id = id });
    }

    #endregion
}
