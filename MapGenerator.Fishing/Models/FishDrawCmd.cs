namespace MapGenerator.Fishing.Models;

/// <summary>Minimal draw command for the fishing canvas, interpreted by fishing.js.</summary>
public class FishDrawCmd
{
    public string T { get; set; } = "";
    public double X { get; set; }
    public double Y { get; set; }
    public double W { get; set; }
    public double H { get; set; }
    public double A1 { get; set; }  // arc start angle
    public double A2 { get; set; }  // arc end angle
    public double Alpha { get; set; } = 1.0;
    public string C { get; set; } = "";
    public string? S { get; set; }   // text or sprite key
    public string? F { get; set; }   // font
    public string? Al { get; set; }  // text align
    public double Pct { get; set; }  // bar fill 0–1
    public double R { get; set; }    // circle radius

    public static FishDrawCmd Fill(double x, double y, double w, double h, string color, double alpha = 1.0) =>
        new() { T = "fill", X = x, Y = y, W = w, H = h, C = color, Alpha = alpha };

    public static FishDrawCmd Circle(double cx, double cy, double r, string color, double alpha = 1.0) =>
        new() { T = "circle", X = cx, Y = cy, R = r, C = color, Alpha = alpha };

    public static FishDrawCmd Line(double x1, double y1, double x2, double y2, string color, double lineWidth = 1.5) =>
        new() { T = "line", X = x1, Y = y1, W = x2, H = y2, C = color, Alpha = lineWidth };

    public static FishDrawCmd Text(string s, double x, double y, string color, string? font = null, string? align = null) =>
        new() { T = "text", S = s, X = x, Y = y, C = color, F = font, Al = align };

    public static FishDrawCmd Bar(double x, double y, double w, double h, double pct, string color) =>
        new() { T = "bar", X = x, Y = y, W = w, H = h, Pct = Math.Clamp(pct, 0, 1), C = color };

    public static FishDrawCmd Sprite(string key, double x, double y, double w, double h) =>
        new() { T = "sprite", S = key, X = x, Y = y, W = w, H = h };

    public static FishDrawCmd Arc(double cx, double cy, double radius, double a1, double a2, string color, double lineWidth = 2, double alpha = 1.0) =>
        new() { T = "arc", X = cx, Y = cy, W = radius, A1 = a1, A2 = a2, C = color, H = lineWidth, Alpha = alpha };
}
