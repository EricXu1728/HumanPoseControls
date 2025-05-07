using Godot;
using System;

public partial class Dummy : RigidBody3D
{
	private Vector3 _previousLinearVelocity = Vector3.Zero;
	private Vector3 _previousAngularVelocity = Vector3.Zero;
	private const float SignificantChangeThreshold = 3.0f; // Adjust as needed

	public override void _PhysicsProcess(double delta)
	{
		//GD.Print(GetPosition());
	}

	public override void _IntegrateForces(PhysicsDirectBodyState3D state)
	{
		base._IntegrateForces(state);

		// Check for significant changes in velocity or torque
		Vector3 linearVelocity = state.LinearVelocity;
		Vector3 angularVelocity = state.AngularVelocity;

		if ((linearVelocity - _previousLinearVelocity).Length() > SignificantChangeThreshold ||
			(angularVelocity - _previousAngularVelocity).Length() > SignificantChangeThreshold)
		{
			GD.Print("pow");
			SpawnPowEffect();
		}

		_previousLinearVelocity = linearVelocity;
		_previousAngularVelocity = angularVelocity;
	}

	private void SpawnPowEffect()
	{
		PackedScene powScene = GD.Load<PackedScene>("res://pow.tscn");
		if (powScene != null)
		{
			Node3D powInstance = powScene.Instantiate<Node3D>();
			GetParent().AddChild(powInstance);
			powInstance.GlobalTransform = GlobalTransform; // Position the effect at the Dummy's location

			// Calculate the velocity direction
			Vector3 hitDirection = (_previousLinearVelocity - _previousLinearVelocity.Normalized()).Normalized();
			Vector3 upwardsVelocity = hitDirection + new Vector3(0, 1, 0); // Add a small upwards component

			// Call the set_velocity method in the pow.gd script
			if (powInstance is Node powNode && powNode.HasMethod("set_velocity"))
			{
				powNode.Call("set_velocity", upwardsVelocity * 5.0f); // Adjust multiplier for desired speed
			}
		}
	}
}
