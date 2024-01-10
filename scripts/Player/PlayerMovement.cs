using Godot;

public partial class PlayerMovement : CharacterBody3D
{
    [Export]
    private float _speed = 5.0f;

    #region Godot Lifecycle

    public override void _PhysicsProcess(double delta)
    {
        var input = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        Velocity = new Vector3(input.X * _speed, Velocity.Y, input.Y * 0.0f);

        MoveAndSlide();
    }

    #endregion
}
