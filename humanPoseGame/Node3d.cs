using Godot;
using System;
using System.Net.Sockets;
using System.IO;
using System.Text.Json;

public partial class Node3d : Node3D
{
	public CharacterBody3D leftHand;
	public CharacterBody3D rightHand;

	private TcpClient client;
	private StreamReader reader;

	[Export] public float PushForce = 3.0f; // How hard the hands push

	public override void _Ready()
	{
		leftHand = GetNode<CharacterBody3D>("left");
		rightHand = GetNode<CharacterBody3D>("right");

		try
		{
			client = new TcpClient("127.0.0.1", 5050);
			reader = new StreamReader(client.GetStream());
			GD.Print("Connected to Python");
		}
		catch (Exception e)
		{
			GD.PrintErr("Failed to connect: ", e.Message);
			CleanupAndFree();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		leftHand.MoveAndSlide();
		rightHand.MoveAndSlide();
		
		// Handle pushing physics objects with the hands
		PushRigidBodies(leftHand);
		PushRigidBodies(rightHand);
	}

	public override void _Process(double delta)
	{
		if (reader == null || reader.EndOfStream) return;

		try
		{
			string json = reader.ReadLine();
			if (json != null)
			{
				var poseData = JsonSerializer.Deserialize<HandPose>(json);
				Vector3 nextLeft = new Vector3((float)poseData.left[0] * 20, (float)poseData.left[1] * 20, 0);
				Vector3 nextRight = new Vector3((float)poseData.right[0] * 20, (float)poseData.right[1] * 20, 0);

				Vector3 leftVelocity = nextLeft - leftHand.Position;
				Vector3 rightVelocity = nextRight - rightHand.Position;

				leftHand.Velocity = leftVelocity * 2;
				rightHand.Velocity = rightVelocity * 2;
			}
		}
		catch (Exception e)
		{
			GD.PrintErr("Error reading pose data: ", e.Message);
			CleanupAndFree();
		}
	}

	// Handle pushing RigidBodies in the vicinity of the hand
	private void PushRigidBodies(CharacterBody3D hand)
	{
		for (int i = 0; i < hand.GetSlideCollisionCount(); i++)
		{
			KinematicCollision3D collision = hand.GetSlideCollision(i);
			if (collision.GetCollider() is RigidBody3D body)
			{
				Vector3 pushDir = -collision.GetNormal(); // Direction to push
				body.ApplyImpulse(pushDir * PushForce, collision.GetPosition() - body.GlobalPosition);
			}
		}
	}

	private void CleanupAndFree()
	{
		try
		{
			reader?.Close();
			client?.Close();
		}
		catch (Exception e)
		{
			GD.PrintErr("Error during cleanup: ", e.Message);
		}
		finally
		{
			GD.Print("Hands are freed");
			QueueFree();
		}
	}

	private class HandPose
	{
		public double[] left { get; set; }
		public double[] right { get; set; }
	}
}
