using Godot;

using System;
using System.Collections.Generic;

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

    #endregion

    private bool _isGameLiftServer;

    public bool IsServer => Multiplayer.IsServer();

    public bool IsDedicatedServer => ClientManager.Instance.UniqueId == 1;

    #region Godot Lifecycle

    public override void _EnterTree()
    {
        base._EnterTree();

        // give the server it's own API so we can run a parallel client
        // per https://github.com/godotengine/godot/issues/80604 custom multiplayer should be set in _enter_tree
        GD.Print($"Creating custom server multiplayer API at {GetPath()}");
        GetTree().SetMultiplayer(MultiplayerApi.CreateDefaultInterface(), GetPath());
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
            return StartServer(port, maxPlayers);
        }
    }

    private bool StartGameLiftServer(int port, int maxPlayers)
    {
        GD.Print("Starting GameLift game server ...");

        var initSDKOutcome = GameLiftServerAPI.InitSDK();
        if(initSDKOutcome.Success) {
            _isGameLiftServer = true;

            var processParameters = new ProcessParameters(
                // OnStartGameSession
                (GameSession gameSession) => {
                    GD.Print("OnStartGameSession");

                    // TODO: what if this fails?
                    StartServer(port, maxPlayers);

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
                new LogParameters(new List<string>()
                {
                    // TODO:
                    "/local/game/logs/myserver.log"
                }));

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

        return StartServer(port, maxPlayers);
    }

    private bool StartServer(int port, int maxPlayers)
    {
        GD.Print($"Starting game server on {port} ...");

        var peer = new ENetMultiplayerPeer();

        var result = peer.CreateServer(port, maxPlayers);
        if(result != Error.Ok) {
            GD.PrintErr($"Failed to create server: {result}");
            return false;
        }

        Multiplayer.MultiplayerPeer = peer;

        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;

        return true;
    }

    public void StopServer()
    {
        GD.Print("Stopping game server ...");

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
    }

    #endregion

    #region Server RPC

    [Rpc(TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void LoadLevel()
    {
        GD.PrintErr("Server received server RPC");
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
