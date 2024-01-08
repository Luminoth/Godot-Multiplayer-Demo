using Godot;

using System;

public partial class JoinMenu : Node
{
    [Export]
    private PackedScene _joiningGameScene;

    [Export]
    private NodePath _addressInputPath;

    private LineEdit _addressInput;

    #region Godot Lifecycle

    public override void _Ready()
    {
        _addressInput = GetNode<LineEdit>(_addressInputPath);
        _addressInput.PlaceholderText = EngineManager.Instance.DefaultAddress;
    }

    #endregion

    #region Signals

    private void _on_join_pressed()
    {
        var address = _addressInput.Text;
        if(string.IsNullOrWhiteSpace(address)) {
            address = EngineManager.Instance.DefaultAddress;
        }

        var scene = _joiningGameScene.Instantiate<JoiningGame>();
        GetTree().Root.AddChild(scene);
        scene.JoinGameSession(address);

        QueueFree();
    }

    #endregion
}
