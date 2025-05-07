using Godot;
using System;

public partial class Testplayer : CharacterBody3D
{
	[Export] public float Speed = 5.0f;
	[Export] public float JumpVelocity = 6.0f;
	[Export] public float PushForce = .5f;

	private Vector3 _velocity = Vector3.Zero;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 input = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		Vector3 direction = new Vector3(input.X, 0, input.Y);
		direction = GlobalTransform.Basis * direction;
		direction.Y = 0;
		direction = direction.Normalized();

		_velocity.X = direction.X * Speed;
		_velocity.Z = direction.Z * Speed;

		if (IsOnFloor() && Input.IsActionJustPressed("ui_accept"))
			_velocity.Y = JumpVelocity;

		_velocity.Y -= (float)(ProjectSettings.GetSetting("physics/3d/default_gravity")) * (float)delta;

		Velocity = _velocity;
		MoveAndSlide();

		PushRigidBodies();
	}

	private void PushRigidBodies()
	{
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			KinematicCollision3D collision = GetSlideCollision(i);
			if (collision.GetCollider() is RigidBody3D body)
			{
				// Direction from player to object (horizontal only)
				Vector3 pushDir = (body.GlobalTransform.Origin - GlobalTransform.Origin);
				//Vector3 pushDir = (body.GlobalTransform.Origin - GlobalTransform.Origin + body.CenterOfMass);
				//pushDir.Y = 0;
				pushDir = pushDir.Normalized();

				//body.ApplyTorqueImpulse(pushDir * PushForce);
				//GD.Print("Push");
				
				body.ApplyImpulse(pushDir * PushForce, collision.GetPosition() - body.GlobalPosition);
			}
		}
	}
}
