using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    ColorChange = 0,
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
