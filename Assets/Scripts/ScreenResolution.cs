using System;
using UnityEngine;

[Serializable]
public class ScreenResolution
{
    public int Width;
    public int Height;
    public ScreenResolution(int width, int height)
    {
        Width = width;
        Height = height;
    }
    public override string ToString()
    {
        return $"{Width}:{Height}";
    }
}