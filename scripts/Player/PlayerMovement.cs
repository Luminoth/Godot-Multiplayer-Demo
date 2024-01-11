using Godot;

public partial class PlayerMovement : CharacterBody3D
{
    [Export]
    private float _speed = 5.0f;

    private Player _player;

    #region Godot Lifecycle

    public override void _EnterTree()
    {
        _player = GetParent<Player>();
    }

    public override void _PhysicsProcess(double delta)
    {
        var direction = _player.Input.Direction;

        GD.Print($"Player {_player.ClientId} moves {direction}");

        Velocity = new Vector3(direction.X * _speed, Velocity.Y, direction.Y * 0.0f);
        MoveAndSlide();
    }

    #endregion
}
