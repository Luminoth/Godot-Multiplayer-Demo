using Godot;

using System;

public partial class JoinMenu : Node
{
    [Export]
    private PackedScene _levelScene;

    private LineEdit _addressInput;



    public override void _Ready()
    {
        _addressInput = GetNode<LineEdit>("VBoxContainer/HBoxContainer/LineEdit");
        _addressInput.PlaceholderText = EngineManager.Instance.DefaultAddress;
    }

    #region Signals

    private void _on_join_pressed()
    {
        var address = _addressInput.Text;
        if(string.IsNullOrWhiteSpace(address)) {
            address = EngineManager.Instance.DefaultAddress;
        }

        if(!ClientManager.Instance.JoinGameSession(address)) {
            GD.PushError("Failed to start game client");
            return;
        }

        var scene = _levelScene.Instantiate();
        GetTree().Root.AddChild(scene);

        QueueFree();
    }

    #endregion
}
