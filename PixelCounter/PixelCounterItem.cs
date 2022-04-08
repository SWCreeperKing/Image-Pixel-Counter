using System.Numerics;
using Raylib_CsLo;
using RayWrapper;
using RayWrapper.Objs;
using RayWrapper.Vars;

namespace PixelCounter;

public class PixelCounterItem : IListItem
{
    public static Vector2? itemSizeReal;
    public static Vector2 itemSize;

    public Label.Style labelStyle = new();

    private Func<int> _size;
    private Func<int, (Color, int)> _getItem;
    private string? _hoverText = "";
    private Tooltip _tooltip;

    public PixelCounterItem(int width, Func<int> size, Func<int, (Color, int)> getItem)
    {
        _size = size;
        _getItem = getItem;
        _tooltip = new GameBox.DefaultTooltip(new Actionable<string?>(() => _hoverText));

        if (itemSizeReal is not null) return;
        itemSizeReal = new Vector2(width, 40);
        itemSize = new Vector2(width - 20, 40);
    }

    public int ListSize() => _size.Invoke();

    public Vector2 ItemSize() => itemSizeReal ?? Vector2.Zero;

    public void Render(Vector2 offset, int item)
    {
        var rect = RectWrapper.AssembleRectFromVec(offset, itemSize);
        var cRect = new Rectangle(offset.X + 6, offset.Y + 6, 40 - 12, 40 - 12);
        var (color, i) = _getItem.Invoke(item);
        labelStyle.Draw($"\t\t\t{i:###,###}", rect);
        cRect.Grow(2).DrawHallowRect(Raylib.BLACK);
        cRect.Draw(color);
        
        if (!cRect.IsMouseIn()) return;
        _hoverText = color.GetString();
        _tooltip.Draw();
    }
}