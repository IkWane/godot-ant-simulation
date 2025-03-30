using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Distrib : Node2D
{
    [Export]
    public int radius;
    [Export]
    public float cellSize;
    private List<Vector2I> points;
    public override void _Ready()
    {
        points = [Vector2I.Zero]; // Initializes the list with the point in the middle of the circle
        float t1 = radius / 16;
        float t2;
        int x = radius;
        int y = 0;
        bool xHasDecrease = false;
        while (x >= y)
        {
            points.Add(new Vector2I(x, y));
            if (y != 0)
            {
                // Adds points within the circle starting from the first octant's points and mirrors them around the circle
                for (int xi = x -1; xi > 0; xi--)
                {
                    points.Add(new Vector2I(xi, y));
                    points.Add(new Vector2I(-xi, y));
                    points.Add(new Vector2I(xi, -y));
                    points.Add(new Vector2I(-xi, -y));
                }
            }
            points.Add(new Vector2I(x, -y));
            points.Add(new Vector2I(-x, -y));
            points.Add(new Vector2I(-x, y));

            points.Add(new Vector2I(y, x));
            if (xHasDecrease) // If x has decreased it means a new line in the grid is available for filling
            {
                // Adds points within that aren't covered by the first octant's for loop
                for (int xi = y - 1; xi > 0; xi--)
                {
                    points.Add(new Vector2I(xi, x));
                    points.Add(new Vector2I(-xi, x));
                    points.Add(new Vector2I(xi, -x));
                    points.Add(new Vector2I(-xi, -x));
                }
                xHasDecrease = false; // resets the variable when operation complete
            }
            points.Add(new Vector2I(y, -x));
            points.Add(new Vector2I(-y, -x));
            points.Add(new Vector2I(-y, x));

            // Adds the points in the middle cross of the circle, they where ignored before to prevent adding the same point multiple times
            for (int i = radius - 1; i > 0; i--)
            {
                points.Add(new Vector2I(i, 0));
                points.Add(new Vector2I(0, i));
                points.Add(new Vector2I(-i, 0));
                points.Add(new Vector2I(0, -i));
            }

            // Algorithm taken from "https://en.wikipedia.org/wiki/Midpoint_circle_algorithm#Jesko's_Method"
            y++;
            t1 += y;
            t2 = t1 - x;
            if (t2 >= 0)
            {
                t1 = t2;
                x--;
                xHasDecrease = true;
            }
        }
        Debug.WriteLine(points.Count);
    }

    public override void _Draw()
    {
        foreach (Vector2 item in points)
        {
            DrawCircle(item * cellSize, 2f, Colors.AliceBlue);
        }
        DrawCircle(Vector2.Zero, 1f, Colors.Red);
    }
}
