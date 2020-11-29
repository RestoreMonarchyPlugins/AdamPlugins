using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

public static class ColorExtras
{

    public static bool tryParseColorName(this UnityEngine.Color color, string name)
    {
        switch (name)
        {
            case "black":
                color = Color.black;
                return true;
            case "blue":
                color = Color.blue;
                return true;
            case "clear":
                color = Color.clear;
                return true;
            case "cyan":
                color = Color.cyan;
                return true;
            case "gray":
                color = Color.gray;
                return true;
            case "green":
                color = Color.grey;
                return true;
            case "magenta":
                color = Color.magenta;
                return true;
            case "red":
                color = Color.red;
                return true;
            case "white":
                color = Color.white;
                return true;
            case "yellow":
                color = Color.yellow;
                return true;
        }
        return false;
    }

    public static void parseRGB(this Color color, int R, int G, int B)
    {
        color.r = R%255;
        color.g = G%255;
        color.b = B%255;
    }
}
