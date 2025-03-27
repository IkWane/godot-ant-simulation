using Godot;
using System;
using System.Diagnostics;
using System.Linq;

public partial class Ant : Node2D
{
	private Random rng;
	private float timer;
	int random_amplitude = 314159265 / 50;
	float size;
	public bool hasFood = false;
	bool hasTrail = false;
	public PheromoneGrid grid;
	public float foodStorage = 0f;
	public int distance = 0;
	public int nbPoints;
	public int radius;
	public override void _Ready()
	{
		rng = new Random();
		size = ((CircleShape2D)GetChild(0).GetChild<CollisionShape2D>(0).Shape).Radius;
		QueueRedraw();
	}

	public override void _Process(double delta)
	{
		Rotation = Rotation + rng.Next(-random_amplitude, random_amplitude) / 100000000f;
		Vector2 sideVector = Transform.Y * size * 2f;
		Vector2 frontVector = Transform.X * size;
		distance += 1;

		if (!hasFood)
		{
			float foodAmount = grid.getFoodValueAt(this.GlobalPosition + frontVector * 2f);
			if (foodAmount > 0.0f)
			{
				float foodTaken = Mathf.Min(foodAmount, 1f);
				grid.addFoodValueAt(this.GlobalPosition + frontVector * 2f, -foodTaken);
				foodStorage += foodTaken;
				hasFood = true;
				Rotation += 3.1415926535f;
				distance = 0;
				QueueRedraw();
			}
		}

		float[] values = new float[3];

		if (hasFood)
		{
			for (int i = 1; i < 15; i++)
			{
				values[0] += grid.getToHomeValueAt(this.GlobalPosition + (frontVector + sideVector) * i);
				values[1] += grid.getToHomeValueAt(this.GlobalPosition + (frontVector - sideVector) * i);
				values[2] += grid.getToHomeValueAt(this.GlobalPosition + frontVector * 2f * i);
			}
		} else {
			for (int i = 1; i < 15; i++)
			{	
				Vector2 pos = this.GlobalPosition + (frontVector + sideVector) * i;
				values[0] += grid.getToFoodValueAt(pos);
				values[0] += grid.getFoodValueAt(pos) * 50f;

				pos = this.GlobalPosition + (frontVector - sideVector) * i;
				values[1] += grid.getToFoodValueAt(pos);
				values[1] += grid.getFoodValueAt(pos) * 50f;

				pos = this.GlobalPosition + frontVector * 2f * i;
				values[2] += grid.getToFoodValueAt(pos);
				values[2] += grid.getFoodValueAt(pos) * 50f;
			}
		}

		var (value, index) = values.Select((n, i) => (n, i)).Max();
		float total = values[0] + values[1] + values[2];
		hasTrail = value > 0f;
		switch (index)
		{
			case 0:
				Rotation += value / total * 0.05f;
				break;

			case 1:
				Rotation -= value / total * 0.05f;
				break;

			case 2:
				break;

			default:
				break;
		}

		if (GlobalPosition.X < grid.GlobalPosition.X || GlobalPosition.X > grid.GlobalPosition.X + grid.gridSize.X * grid.cellSize.X)
		{
			Rotation = 3.1415926535f - Rotation;
			this.Position = new Vector2(Mathf.Clamp(Position.X, grid.GlobalPosition.X, grid.GlobalPosition.X + grid.gridSize.X * grid.cellSize.X), Position.Y);
		}
		if (GlobalPosition.Y < grid.GlobalPosition.Y || GlobalPosition.Y > grid.GlobalPosition.Y + grid.gridSize.Y * grid.cellSize.Y)
		{
			Rotation = -Rotation;
			this.Position = new Vector2(Position.X, Mathf.Clamp(Position.Y, grid.GlobalPosition.Y, grid.GlobalPosition.Y + grid.gridSize.Y * grid.cellSize.Y));
		}

		this.Position += Transform.X * size * 0.25f;

		if (timer > 0)
		{
			timer -= 0.016f;
		} else {
			if (hasFood && distance < 10000)
			{
				grid.addToFoodValueAt(this.GlobalPosition, 1/(1 + distance * 0.5f));
			} else {
				grid.addToHomeValueAt(this.GlobalPosition, 1/(1 + distance * 0.5f));
			}
			timer = 0.1f;
		}

	}

	public override void _Draw()
	{
		Color color;
		if (hasFood)
		{
			color = Colors.Orange;
		} else {
			color = Colors.Red;
		}
		DrawCircle(Vector2.Zero, size, color);
		DrawCircle(Vector2.Right * size, size/1.5f, Colors.Black);
	}

}
