using Godot;

using System.Collections.Generic;

using Aws.GameLift.Server;
using Aws.GameLift.Server.Model;

public partial class ServerManager : SingletonNode<ServerManager>
{
    [Export]
    private int _listeningPort = 7777;

    private bool _isGameLiftServer;

    #region Godot Lifecycle

    public override void _ExitTree()
    {
        StopServer();
    }

    #endregion

    public bool StartServer(bool gamelift)
    {
        if(gamelift) {
            return StartGameLiftServer();
        } else {
            return StartLocalServer();
        }
    }

    private bool StartGameLiftServer()
    {
        var initSDKOutcome = GameLiftServerAPI.InitSDK();
        if(initSDKOutcome.Success) {
            var processParameters = new ProcessParameters(
                // OnStartGameSession
                (GameSession gameSession) => {
                    GD.Print("OnStartGameSession");
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
                },
                // OnHealthCheck
                () => {
                    GD.Print("OnHealthCheck");
                    return true;
                },
                _listeningPort,
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
            GD.PushError("InitSDK failure : " + initSDKOutcome.Error.ToString());
            return false;
        }
    }

    private bool StartLocalServer()
    {
        return false;
    }

    public void StopServer()
    {
        GameLiftServerAPI.Destroy();
    }
}
