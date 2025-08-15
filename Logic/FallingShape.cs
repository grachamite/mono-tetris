namespace MonoTetris.Logic;

public class FallingShape (int left, int top, Shape shape, int colorId)
{
    private const int SStatesCount = 4;

    public int Left = left;
    public int Top = top;
    public readonly int Width = shape.Width;
    public readonly int Height = shape.Height;
    public readonly int ColorId = colorId;
    
    private int _state = 0;

    public int NextState()
    {
        _state++;
        if (_state >= SStatesCount)
        {
            _state = 0;
        }

        return _state;
    }

    public int PrevState()
    {
        _state--;
        if (_state < 0)
        {
            _state = SStatesCount - 1;
        }

        return _state;
    }

    public int[,] GetCurrentStateMatrix()
    {
        int[,] matrix = new int[5, 5];
        int[] remap = {0, colorId};
        
        for (var y = 0; y < shape.Height; y++)
        {
            for (var x = 0; x < shape.Width; x++)
            {
                matrix[x, y] = remap[shape.Matrix[
                    _state * (shape.Width * shape.Height) + x + shape.Width * y
                ]];
            }
        }

        return matrix;
    }

    public string GetShapeCode()
    {
        return shape.Code;
    }
}