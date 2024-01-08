using Godot;

using System;

public partial class Player : Node
{
    #region Godot Lifecycle

    public override void _Ready()
    {
        GD.Print("Player ready!");
    }

    #endregion
}
