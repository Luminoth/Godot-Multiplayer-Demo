using Godot;

using System;

public partial class JoinMenu : Node
{
    private LineEdit _addressInput;

    public string DefaultAddress => $"127.0.0.1:{ServerManager.Instance.ListenPort}";

    public override void _Ready()
    {
        _addressInput = GetNode<LineEdit>("VBoxContainer/HBoxContainer/LineEdit");
        _addressInput.PlaceholderText = DefaultAddress;
    }

    #region Signals

    private void _on_join_pressed()
    {
        var address = _addressInput.Text;
        if(string.IsNullOrWhiteSpace(address)) {
            address = DefaultAddress;
        }

        GD.Print($"Joining game session at {address} ...");
    }

    #endregion
}
