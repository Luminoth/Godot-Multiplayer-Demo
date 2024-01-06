using Godot;

public partial class Player : CharacterBody3D
{
    [Export]
    private float _speed = 5.0f;

    #region Godot Lifecycle

    public override void _Ready()
    {
        GD.Print("Player ready!");
    }

    public override void _PhysicsProcess(double delta)
    {
        var input = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        Velocity = new Vector3(input.X, Velocity.Y, input.Y) * _speed;

        MoveAndSlide();
    }

    #endregion
}
