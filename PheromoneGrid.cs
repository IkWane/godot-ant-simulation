using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class PheromoneGrid : Node2D
{
    public struct Cell {
        public float concentration;
        public float decay;
        public Cell() {
            concentration = 0;
            decay = 0;
        }
    }

    [Export]
    public Vector2I gridSize;
    [Export]
    public Vector2 cellSize;
    [Export]
    public float decayTime;
    List<Cell> toHomeGrid = new List<Cell>();
    List<Cell> toFoodGrid = new List<Cell>();
    List<float> foodGrid = new List<float>();
    public override void _Ready()
    {
        toHomeGrid.InsertRange(0, Enumerable.Repeat(new Cell(), gridSize.X * gridSize.Y));
        toFoodGrid.InsertRange(0, Enumerable.Repeat(new Cell(), gridSize.X * gridSize.Y));
        foodGrid.InsertRange(0, Enumerable.Repeat(0f, gridSize.X * gridSize.Y));
    }
    
    public override void _Process(double delta)
	{
		for (int i = 0; i < gridSize.X * gridSize.Y; i++)
        {
            Cell toHomeCell = toHomeGrid[i];
            if (toHomeCell.decay > 0)
            {
                toHomeCell.decay -= 0.016f;
            } else {
                toHomeCell.concentration = 0;
            }
            toHomeGrid[i] = toHomeCell;

            Cell toFoodCell = toFoodGrid[i];
            if (toFoodCell.decay > 0)
            {
                toFoodCell.decay -= 0.016f;
            } else {
                toFoodCell.concentration = 0;
            }
            toFoodGrid[i] = toFoodCell;
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

        if (index >= toHomeGrid.Capacity) return -1;
        
        return toHomeGrid[index].concentration;
    }
    public float getToFoodValueAt(Vector2 globalPos)
    {
        int index = getIndexAt(getGridPos(globalPos));

        if (index >= toFoodGrid.Capacity) return -1;
        
        return toFoodGrid[index].concentration;
    }
    public float getFoodValueAt(Vector2 globalPos)
    {
        int index = getIndexAt(getGridPos(globalPos));

        if (index >= foodGrid.Capacity) return -1;
        return foodGrid[index];
    }

    public void setToHomeValueAt(Vector2 globalPos, float value) 
    {
        int index = getIndexAt(getGridPos(globalPos));

        if (index >= toHomeGrid.Capacity) return;
        Cell toHomeCell = toHomeGrid[index];
        toHomeCell.concentration = value;
        toHomeCell.decay = decayTime;
        toHomeGrid[index] = toHomeCell;
    }
    public void setToFoodValueAt(Vector2 globalPos, float value) 
    {
        int index = getIndexAt(getGridPos(globalPos));

        if (index >= toFoodGrid.Capacity) return;

        Cell toFoodCell = toFoodGrid[index];
        toFoodCell.concentration = value;
        toFoodCell.decay = decayTime;
        toFoodGrid[index] = toFoodCell;
    }
    public void setFoodValueAt(Vector2 globalPos, float value) 
    {
        int index = getIndexAt(getGridPos(globalPos));

        if (index >= foodGrid.Capacity) return;

        foodGrid[index] = value;
        QueueRedraw();
    }
    public void addToHomeValueAt(Vector2 globalPos, float value) 
    {
        int index = getIndexAt(getGridPos(globalPos));

        if (index >= toHomeGrid.Capacity) return;

        Cell toHomeCell = toHomeGrid[index];
        toHomeCell.concentration += value;
        toHomeCell.decay = decayTime;
        toHomeGrid[index] = toHomeCell;
    }
    public void addToFoodValueAt(Vector2 globalPos, float value) 
    {
        int index = getIndexAt(getGridPos(globalPos));

        if (index >= toFoodGrid.Capacity) return;

        Cell toFoodCell = toFoodGrid[index];
        toFoodCell.concentration += value;
        toFoodCell.decay = decayTime;
        toFoodGrid[index] = toFoodCell;
    }
    public void addFoodValueAt(Vector2 globalPos, float value) 
    {
        int index = getIndexAt(getGridPos(globalPos));

        if (index >= foodGrid.Capacity) return;

        foodGrid[index] += value;
        QueueRedraw();
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
