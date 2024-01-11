using Godot;

public partial class MainMenu : Node
{
    #region Signals

    private void _on_create_pressed()
    {
        GD.Print("Creating game session ...");

        if(!GameManager.Instance.StartLocalServer()) {
            return;
        }

        var scene = UIManager.Instance.JoiningGameScene.Instantiate<JoiningGame>();
        GetTree().Root.AddChild(scene);
        scene.JoinLocalGameSession();

        QueueFree();
    }

    private void _on_join_pressed()
    {
        var scene = UIManager.Instance.JoinGameScene.Instantiate();
        GetTree().Root.AddChild(scene);

        QueueFree();
    }

    #endregion


}
