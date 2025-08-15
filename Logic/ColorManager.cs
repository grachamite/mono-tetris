using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace MonoTetris.Logic;

public static class ColorManager
{
    private static readonly Color[] SColors = new[]
    {
        Color.Red,
        Color.Blue,
        Color.Green,
        Color.Cyan,
        Color.Magenta,
        Color.Yellow,
        Color.Orange,
        Color.Violet
    };
    
    const float Fade = 0.8f;
    
    public static int GetRandomColorIndex()
    {
        var random = new Random();
        return random.Next(SColors.Length);
    }

    public static Color GetColor(int colorIndex)
    {
        return SColors[colorIndex];
    }
    
    public static Color GetFadeColor(int colorIndex)
    {
        var color = SColors[colorIndex];
        color.R = (byte) (color.R * Fade);
        color.G = (byte) (color.G * Fade);
        color.B = (byte) (color.B * Fade);
        
        return color;
    }
}