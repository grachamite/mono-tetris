namespace MonoTetris.Logic;

public class Shape
{
    public int Width  { get; set; } 
    public int Height  { get; set; } 
    public string Code { get; set; }
    public int[] Matrix { get; set; }
    public bool Classic { get; set; }
    
}