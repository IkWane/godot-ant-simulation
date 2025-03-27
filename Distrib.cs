using Godot;
using System;
using System.Collections.Generic;

public partial class Distrib : Node2D
{
    [Export]
    public uint nbPoints;
    [Export]
    public float radius;
    private List<Vector2> points;
    private const float ratio = 2.39996322972f;
    public override void _Ready()
    {
        points = new List<Vector2>();
        float maxDist = Mathf.Sqrt(nbPoints);
        for (int i = 1; i <= nbPoints; i++)
        {
            float angle = ratio * i;
            float distance = Mathf.Sqrt(i) * radius / maxDist;
            points.Add(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance);
        }
    }

    public override void _Draw()
    {
        foreach (Vector2 item in points)
        {
            DrawCircle(item, 3f, Colors.AliceBlue);
        }
    }
}
