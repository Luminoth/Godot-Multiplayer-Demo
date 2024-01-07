using Godot;

using System;

public partial class Level : Node
{
    public override void _Ready()
    {
        GD.Print("Level loading ...");
        GD.Print($"Peer ID: {ClientManager.Instance.UniqueId}");
        GD.Print($"Is Server: {ServerManager.Instance.IsServer}");
        GD.Print($"Is Dedicated Server: {ServerManager.Instance.IsDedicatedServer}");
    }
}
