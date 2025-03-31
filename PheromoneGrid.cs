using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class PheromoneGrid : Node2D
{
    [Export]
    public Vector2I gridSize;
    [Export]
    public Vector2 cellSize;
    [Export]
    public float decayTime;
    List<float> toHomeGrid = new List<float>();
    List<float> toFoodGrid = new List<float>();
    List<float> foodGrid = new List<float>();
    public override void _Ready()
    {
        toHomeGrid.InsertRange(0, Enumerable.Repeat(0f, gridSize.X * gridSize.Y));
        toFoodGrid.InsertRange(0, Enumerable.Repeat(0f, gridSize.X * gridSize.Y));
        foodGrid.InsertRange(0, Enumerable.Repeat(0f, gridSize.X * gridSize.Y));
    }
    
    public override void _Process(double delta)
	{
		for (int i = 0; i < gridSize.X * gridSize.Y; i++)
        {
            if (toHomeGrid[i] > 0)
            {
                toHomeGrid[i] -= 0.0000001f;
            } 
            if (toFoodGrid[i] > 0)
            {
                toHomeGrid[i] -= 0.0000001f;
            }
        }

        if (Input.IsActionPressed("Click"))
        {
            addFoodValueAt(GetGlobalMousePosition(), 1f);
        }
	}

    Vector2I getGridPos(Vector2 globalPos) 
    {
        return new Vector2I((int)Mathf.Floor(globalPos.X / cellSize.X), (int)Mathf.Floor(globalPos.Y / cellSize.Y));
    }

    int getIndexAt(Vector2I gridPos) 
    {
        return Mathf.Clamp(gridPos.X, 0, gridSize.X) + Mathf.Clamp(gridPos.Y, 0, gridSize.Y) * gridSize.X;
    }

    public float getToHomeValueAt(Vector2 globalPos)
    {
        int index = getIndexAt(getGridPos(globalPos));
        if (index >= toHomeGrid.Count) return 0; 
        return toHomeGrid[index];
    }
    public float getToFoodValueAt(Vector2 globalPos)
    {
        int index = getIndexAt(getGridPos(globalPos));
        if (index >= toHomeGrid.Count) return 0;
        return toFoodGrid[index];
    }
    public float getFoodValueAt(Vector2 globalPos)
    {
        int index = getIndexAt(getGridPos(globalPos));
        if (index >= toHomeGrid.Count) return 0;
        return foodGrid[index];
    }

    public void setToHomeValueAt(Vector2 globalPos, float value) 
    {
        int index = getIndexAt(getGridPos(globalPos));

        if (index >= toHomeGrid.Count) return;

        toHomeGrid[index] = value;
    }
    public void setToFoodValueAt(Vector2 globalPos, float value) 
    {
        int index = getIndexAt(getGridPos(globalPos));

        if (index >= toFoodGrid.Count) return;

        toFoodGrid[index] = value;
    }
    public void setFoodValueAt(Vector2 globalPos, float value) 
    {
        int index = getIndexAt(getGridPos(globalPos));

        if (index >= foodGrid.Count) return;

        foodGrid[index] = value;
        QueueRedraw();
    }
    public void addToHomeValueAt(Vector2 globalPos, float value) 
    {
        int index = getIndexAt(getGridPos(globalPos));

        if (index >= toHomeGrid.Count) return;

        toHomeGrid[index] += value;
    }
    public void addToFoodValueAt(Vector2 globalPos, float value) 
    {
        int index = getIndexAt(getGridPos(globalPos));

        if (index >= toFoodGrid.Count) return;

        toFoodGrid[index] += value;
    }
    public void addFoodValueAt(Vector2 globalPos, float value) 
    {
        int index = getIndexAt(getGridPos(globalPos));

        if (index >= foodGrid.Count) return;

        foodGrid[index] += value;
        QueueRedraw();
    }

    public List<Vector2I> generateOffsetsCircle(int radius)
    {
        List<Vector2I> offsets = [Vector2I.Zero]; // Initializes the list with the point in the middle of the circle
        float t1 = radius / 16;
        float t2;
        int x = radius;
        int y = 0;
        bool xHasDecrease = false;
        while (x >= y)
        {
            offsets.Add(new Vector2I(x, y));
            if (y != 0)
            {
                // Adds points within the circle starting from the first octant's points and mirrors them around the circle
                for (int xi = x -1; xi > 0; xi--)
                {
                    offsets.Add(new Vector2I(xi, y));
                    offsets.Add(new Vector2I(-xi, y));
                    offsets.Add(new Vector2I(xi, -y));
                    offsets.Add(new Vector2I(-xi, -y));
                }
            }
            offsets.Add(new Vector2I(x, -y));
            offsets.Add(new Vector2I(-x, -y));
            offsets.Add(new Vector2I(-x, y));

            offsets.Add(new Vector2I(y, x));
            if (xHasDecrease) // If x has decreased it means a new line in the grid is available for filling
            {
                // Adds points within that aren't covered by the first octant's for loop
                for (int xi = y - 1; xi > 0; xi--)
                {
                    offsets.Add(new Vector2I(xi, x));
                    offsets.Add(new Vector2I(-xi, x));
                    offsets.Add(new Vector2I(xi, -x));
                    offsets.Add(new Vector2I(-xi, -x));
                }
                xHasDecrease = false; // resets the variable when operation complete
            }
            offsets.Add(new Vector2I(y, -x));
            offsets.Add(new Vector2I(-y, -x));
            offsets.Add(new Vector2I(-y, x));

            // Adds the points in the middle cross of the circle, they where ignored before to prevent adding the same point multiple times
            for (int i = radius - 1; i > 0; i--)
            {
                offsets.Add(new Vector2I(i, 0));
                offsets.Add(new Vector2I(0, i));
                offsets.Add(new Vector2I(-i, 0));
                offsets.Add(new Vector2I(0, -i));
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
        return offsets;
    }

    public float getToHomeValueAtBatch(Vector2 globalPos, List<Vector2I> offsets)
    {
        Vector2I gpos = getGridPos(globalPos);
        float sum = 0;
        foreach (Vector2I offset in offsets)
        {
            int index = getIndexAt(gpos + offset);
            if (index >= foodGrid.Count) continue;
            sum += toHomeGrid[index];
        }
        return sum / offsets.Count;
    }
    public float getToFoodValueAtBatch(Vector2 globalPos, List<Vector2I> offsets)
    {
        Vector2I gpos = getGridPos(globalPos);
        float sum = 0;
        foreach (Vector2I offset in offsets)
        {
            int index = getIndexAt(gpos + offset);
            if (index >= foodGrid.Count) continue;
            sum += toFoodGrid[index];
        }
        return sum / offsets.Count;
    }
    public float getFoodValueAtBatch(Vector2 globalPos, List<Vector2I> offsets)
    {
        Vector2I gpos = getGridPos(globalPos);
        float sum = 0;
        foreach (Vector2I offset in offsets)
        {
            int index = getIndexAt(gpos + offset);
            if (index >= foodGrid.Count) continue;
            sum += foodGrid[index];
        }
        return sum / offsets.Count;
    }

    public override void _Draw()
    {
        DrawRect(new Rect2(0, 0, gridSize.X * cellSize.X, gridSize.Y * cellSize.Y), Colors.Aqua, false, 5);
        for (int Y = 0; Y < gridSize.Y; Y++)
        {
            for (int X = 0; X < gridSize.X; X++)
            {
                int index = getIndexAt(new Vector2I(X, Y));
                if (foodGrid[index] > 0)
                {
                    float a = Mathf.Clamp(foodGrid[index], 0, 1);
                    Color color = new Color(1, 1, 0, a);
                    DrawCircle(new Vector2(X * cellSize.X + cellSize.X/2, Y * cellSize.Y + cellSize.Y/2), cellSize.X/2, color);
                }
                // if (toHomeGrid[index].concentration > 0)
                // {
                //     float a = Mathf.Clamp(toHomeGrid[index].concentration, 0, 1);
                //     Color color = new Color(1, 0, 0, a);
                //     DrawCircle(new Vector2(X * cellSize.X + cellSize.X/2, Y * cellSize.Y + cellSize.Y/2), 5f, color);
                // }
            }
        }
    }
}
