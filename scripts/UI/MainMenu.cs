using Godot;

public partial class MainMenu : Node
{
    [Export]
    private PackedScene _joinGameScene;

    [Export]
    private PackedScene _joiningGameScene;

    #region Signals

    private void _on_create_pressed()
    {
        GD.Print("Creating game session ...");

        if(!ServerManager.Instance.StartLocalServer(GameManager.Instance.ListenPort, GameManager.Instance.MaxPlayers)) {
            GD.PrintErr("Failed to start local game server");
            return;
        }

        var scene = _joiningGameScene.Instantiate<JoiningGame>();
        GetTree().Root.AddChild(scene);
        scene.JoinLocalGameSession();

        QueueFree();
    }

    private void _on_join_pressed()
    {
        var scene = _joinGameScene.Instantiate();
        GetTree().Root.AddChild(scene);

        QueueFree();
    }

    #endregion


}
