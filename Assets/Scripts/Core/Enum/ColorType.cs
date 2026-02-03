using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorType
{
    WHITE = 0,
    RED = 1,
    YELLOW = 2,
    BLUE = 3,
}

public static class ColorVisualMap
{
    public static Color GetBaseColor(ColorType colorType)
    {
        switch (colorType)
        {
            case ColorType.RED: return Color.red;
            case ColorType.BLUE: return Color.blue;
            case ColorType.YELLOW: return Color.yellow;
            default: return Color.white;
        }
    }
}
