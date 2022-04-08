using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Objs;
using RayWrapper.Vars;

namespace PixelCounter;

public class ImageSelection : GhostObject
{
    public List<string> files = new();

    public ImageSelection()
    {
        Text.Style.SetDefaultFont(Raylib.LoadFont("Assets/CascadiaCode.otf"));

        Text txt = new("Drop files to load", new Vector2(300, 300))
            { isVisible = new Actionable<bool>(() => !files.Any()) };

        ListFileItem.onClick = s =>
        {
            var name = ListFileItem.GetFileName(s);
            Program.tb.AddTab(name, new PixelCounter(s, name));
        };
        ListFileItem lfi = new(500);
        ListView lv = new(new Vector2(10, 100), lfi, 12);

        RegisterGameObj(txt, lv);
    }

    protected override void UpdateCall()
    {
        files.AddRange(Raylib.GetDroppedFilesAndClear().Select(s => s.Replace("\\", "/")));
        files.RemoveAll(f => !f.Contains(".png"));
        files = files.Union(files).ToList();
        ListFileItem.files.AddRange(files.Except(ListFileItem.files));
    }

    protected override void RenderCall()
    {
    }
}