using Godot;

public partial class Boot : Node
{
    [Export]
    private PackedScene _dedicatedScene;

    public override void _Process(double delta)
    {
        if(EngineManager.Instance.CommandLineArgs.ContainsKey("--dedicated") || EngineManager.Instance.CommandLineArgs.ContainsKey("--gamelift")) {
            GD.Print("Starting dedicated server ...");

            var scene = _dedicatedScene.Instantiate();
            GetTree().Root.AddChild(scene);
        } else {
            GD.Print("Starting client ...");

            var scene = UIManager.Instance.MainMenuScene.Instantiate();
            GetTree().Root.AddChild(scene);
        }

        QueueFree();
    }
}
