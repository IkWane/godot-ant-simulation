using Godot;
using System;

public partial class AntSpawner : Node2D
{
    [Export]
    public PackedScene antScene;
    [Export]
    public PheromoneGrid grid;
    [Export]
    public uint maxAnts;
    Random rng = new Random();
    uint antAmount = 0;
    float foodStorage = 0f;

    public override void _Ready()
    {
        for (int i = 0; i < 10; i++)
        { 
            spawnAnt();
        }
    }

    private void spawnAnt() 
    {
        Ant newAnt = antScene.Instantiate<Ant>();
        GetNode("/root/main").CallDeferred("add_child", newAnt);
        newAnt.Position = this.Position;
        newAnt.Rotation = rng.NextSingle() * 3.1415926535f * 2f;
        newAnt.grid = grid;
        antAmount += 1;
    }
    public override void _Process(double delta)
    {
        if (foodStorage > 5f)
        {
            spawnAnt();
            foodStorage = 0f;
        }
    }

    public void collectFood(Area2D area)
    {
        Ant ant = area.GetParent<Ant>();
        if (ant.hasFood)
        {
            ant.Rotation = 3.1415926535f + ant.Rotation;
            foodStorage += ant.foodStorage;
            ant.foodStorage = 0f;
            ant.hasFood = false;
            ant.distance = 0;
            ant.QueueRedraw();
        } else {
            ant.distance = 0;
        }
    }

    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, ((CircleShape2D)GetChild(0).GetChild<CollisionShape2D>(0).Shape).Radius, Colors.Red);
    }
}
