using Godot;

public partial class MainMenu : Node
{
    [Export]
    private PackedScene _levelScene;

    [Export]
    private PackedScene _joinGameScene;

    #region Signals

    private void _on_create_pressed()
    {
        GD.Print("Creating game session ...");

        if(!ServerManager.Instance.StartLocalServer()) {
            GD.PushError("Failed to start local game server");
            return;
        }

        if(!ClientManager.Instance.JoinLocalGameSession()) {
            GD.PushError("Failed to start game client");
            return;
        }

        var scene = _levelScene.Instantiate();
        GetTree().Root.AddChild(scene);

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
