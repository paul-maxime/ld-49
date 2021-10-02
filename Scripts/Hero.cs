using Godot;
using System;

public class Hero : KinematicBody2D
{
	[Export] public int speed = 200;

	public Vector2 velocity = new Vector2();

	public override void _Ready()
	{
		GetNode<AnimationPlayer>("AnimationPlayer").Play("Idle");
	}

	private StaticBody2D GetClosestPotion()
	{
		var potions = GetTree().GetNodesInGroup("selectable");
		if (potions.Count == 0)
		{
			return null;
		}
		StaticBody2D nearestPotion = (StaticBody2D)potions[0];
		foreach (StaticBody2D potion in potions)
		{
			if (potion.GlobalPosition.DistanceTo(this.GlobalPosition) < nearestPotion.GlobalPosition.DistanceTo(this.GlobalPosition))
			{
				nearestPotion = potion;
			}
		}
		return nearestPotion;
	}

	private void HidePotionsOutline()
	{
		var potions = GetTree().GetNodesInGroup("selectable");
		foreach (StaticBody2D potion in potions)
		{
			potion.GetNode<Sprite>("Outline").Visible = false;
		}
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event.IsActionPressed("ui_accept"))
		{
			var closestPotion = GetClosestPotion();
			if (closestPotion != null && closestPotion.GlobalPosition.DistanceTo(this.GlobalPosition) < 80)
			{
				closestPotion.QueueFree();
			}
		}
	}

	public void GetVelocity()
	{
		velocity = new Vector2();

		if (Input.IsActionPressed("ui_right"))
			velocity.x += 1;

		if (Input.IsActionPressed("ui_left"))
			velocity.x -= 1;

		if (Input.IsActionPressed("ui_down"))
			velocity.y += 1;

		if (Input.IsActionPressed("ui_up"))
			velocity.y -= 1;

		var animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		if (velocity != new Vector2())
		{
			HidePotionsOutline();
			var closestPotion = GetClosestPotion();
			if (closestPotion != null && closestPotion.GlobalPosition.DistanceTo(this.GlobalPosition) < 80)
			{
				closestPotion.GetNode<Sprite>("Outline").Visible = true;
			}
			if (velocity.y > 0)
			{
				animationPlayer.Play("Walk");
			}
			else if (velocity.y < 0)
			{
				animationPlayer.Play("Walk back");
			}
			if (velocity.x > 0)
			{
				animationPlayer.Play("Walk right");
			}
			else if (velocity.x < 0)
			{
				animationPlayer.Play("Walk left");
			}
		}
		else
		{
			if (animationPlayer.CurrentAnimation == "Walk")
			{
				animationPlayer.Play("Idle");
			}
			else if (animationPlayer.CurrentAnimation == "Walk back")
			{
				animationPlayer.Play("Idle back");
			}
			else if (animationPlayer.CurrentAnimation == "Walk left")
			{
				animationPlayer.Play("Idle left");
			}
			else if (animationPlayer.CurrentAnimation == "Walk right")
			{
				animationPlayer.Play("Idle right");
			}
		}

		velocity = velocity.Normalized() * speed;
	}

	public override void _PhysicsProcess(float delta)
	{
		GetVelocity();
		velocity = MoveAndSlide(velocity);
	}
}
