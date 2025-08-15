using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace MonoTetris.Logic;

public class Map(int width, int height)
{
    private readonly int[,] _matrix = new int[width, height];

    public readonly int Width = width;
    public readonly int Height = height;

    
    public int[,] GetMatrix()
    {
        return _matrix;
    }

    public int Get(int left, int top)
    {
        return _matrix[left, top];
    }

    public bool IsEmptyLine(int lineY)
    {
        int[] line = new int [Width];
        
        for (int x = 0; x < Width; x++)
        {
            line[x] = _matrix[x, lineY];
        }

        return Array.TrueForAll(line, i => i == 0);
    }
    
    public bool IsFilledLine(int lineY)
    {
        int[] line = new int [Width];
        
        for (int x = 0; x < Width; x++)
        {
            line[x] = _matrix[x, lineY];
        }

        return Array.TrueForAll(line, i => i != 0);
    }

    public void RemoveLine(int lineY)
    {
        for (int x = 0; x < Width; x++)
        {
            _matrix[x, lineY] = 0;
        }
    }
    
    
    public void GravitateLines()
    {
        for (int y = Height - 1; y >= 0; y--)
        {
            var yOffset = 0;
            while (IsEmptyLine(y) && yOffset < y)
            {
                OffsetDownToLine(y);
                yOffset++;
            }
        }
    }

    public void OffsetDownToLine(int lineY)
    {
        for (int y = lineY; y >= 0; y--)
        {
            for (int x = 0; x < Width; x++)
            {
                _matrix[x, y] = (y > 0) ? _matrix[x, y - 1] : 0;
            }
        }
    }

    public void SaveMatrix(int left, int top, int[,] shapeMatrix, int colorId)
    {
        int shapeWidth = shapeMatrix.GetUpperBound(0) + 1;
        int shapeHeight = shapeMatrix.GetUpperBound(1) + 1;

        for (int y = 0; y < shapeHeight; y++)
        {
            bool[] line = new bool [shapeWidth];
            for (int x = 0; x < shapeWidth; x++)
            {
                if (shapeMatrix[x, y] != 0 && left + x >= 0 && left + x < Width && top + y >= 0 && top + y < Height)
                {
                    _matrix[left + x, top + y] = colorId;
                }
            }
        }
    }
    
    public bool CheckCollision(int left, int top, int[,] shapeMatrix)
    {
        int shapeWidth = shapeMatrix.GetUpperBound(0) + 1;
        int shapeHeight = shapeMatrix.GetUpperBound(1) + 1;

        for (int y = 0; y < shapeHeight; y++)
        {
            bool[] line = new bool [shapeWidth];
            for (int x = 0; x < shapeWidth; x++)
            {
                if (shapeMatrix[x, y] != 0
                    && (left + x < 0 || left + x >= Width || top + y < 0 || top + y >= Height))
                {
                    return true;
                }

                line[x] = shapeMatrix[x, y] == 0 || _matrix[left + x, top + y] == 0;
            
            }
            
            // Check collision in each line
            if (Array.TrueForAll(line, i => i == true) == false)
            {
                return true;
            }
        }

        // No collisions
        return false;
    }
}