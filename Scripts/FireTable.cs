using Godot;

public class FireTable : Table
{
	public override void Interact()
	{
		GetNode<AnimationPlayer>("AnimationPlayer").Play("Working");
		GetNode<Timer>("Timer").Start();
		isWorking = true;
	}
}
