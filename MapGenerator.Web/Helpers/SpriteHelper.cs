using System.Text;

namespace MapGenerator.Web.Helpers;

public static class SpriteHelper
{
    public static string ToSvgDataUri(string[] pixels, int cols = 16)
    {
        int rows = pixels.Length / cols;
        var sb = new StringBuilder();
        sb.Append($"<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 {cols} {rows}'>");
        for (int i = 0; i < pixels.Length; i++)
        {
            if (string.IsNullOrEmpty(pixels[i])) continue;
            int x = i % cols, y = i / cols;
            sb.Append($"<rect x='{x}' y='{y}' width='1' height='1' fill='{pixels[i]}'/>");
        }
        sb.Append("</svg>");
        return "data:image/svg+xml;base64," + Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString()));
    }
}
