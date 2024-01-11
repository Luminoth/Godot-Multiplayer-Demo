using Godot;

public partial class JoinMenu : Node
{
    [Export]
    private PackedScene _joiningGameScene;

    [Export]
    private PackedScene _mainMenuScene;

    [Export]
    private LineEdit _addressInput;

    #region Godot Lifecycle

    public override void _Ready()
    {
        _addressInput.PlaceholderText = GameManager.Instance.DefaultAddress;
    }

    #endregion

    #region Signals

    private void _on_join_pressed()
    {
        var address = _addressInput.Text;
        if(string.IsNullOrWhiteSpace(address)) {
            address = GameManager.Instance.DefaultAddress;
        }

        var scene = _joiningGameScene.Instantiate<JoiningGame>();
        GetTree().Root.AddChild(scene);
        scene.JoinGameSession(address);

        QueueFree();
    }

    private void _on_back_pressed()
    {
        var scene = _mainMenuScene.Instantiate<MainMenu>();
        GetTree().Root.AddChild(scene);

        QueueFree();
    }

    #endregion
}
