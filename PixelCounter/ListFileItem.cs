using System.Numerics;
using Raylib_CsLo;
using RayWrapper;
using RayWrapper.Objs;

namespace PixelCounter;

public class ListFileItem : IListItem
{
    public static List<string> files = new();

    // public static ImageObj trash; // todo: when i get button image to exist
    // public static ImageObj open;
    public static Vector2? itemSizeReal;
    public static Vector2 itemSize;
    public static Action<string> onClick;

    public Label.Style labelStyle = new() { drawHover = true };

    public ListFileItem(int width)
    {
        if (itemSizeReal is null)
        {
            itemSizeReal = new Vector2(width, 40);
            itemSize = new Vector2(width - 20, 40);
        }

        // trash = new ImageObj("Assets/Images/delete.png");
        // open = new ImageObj("Assets/Images/open.png");
    }

    public int ListSize() => files.Count;
    public Vector2 ItemSize() => itemSizeReal ?? Vector2.Zero;

    public void Render(Vector2 offset, int item)
    {
        var rect = RectWrapper.AssembleRectFromVec(offset, itemSize);
        labelStyle.Draw(GetFileName(files[item]), rect);
        if (rect.IsMouseIn() && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) onClick.Invoke(files[item]);
    }

    public static string GetFileName(string file)
    {
        var span = new Span<char>(file.ToCharArray());
        return span[(span.LastIndexOf('/') + 1)..].ToString();
    }
}