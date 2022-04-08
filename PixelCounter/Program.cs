using System.Numerics;
using PixelCounter;
using Raylib_CsLo;
using RayWrapper;
using RayWrapper.Objs;
using RayWrapper.Vars;

new GameBox(new Program(), new Vector2(1280, 720), "Pixel Counter");

public partial class Program : GameLoop
{
    public static TabView tb;

    public override void Init()
    {
        tb = new TabView(new Vector2(0, 0), 1280);
        tb.AddTab("Main Tab", new ImageSelection());
        RegisterGameObj(tb);
    }

    public override void UpdateLoop()
    {
    }

    public override void RenderLoop()
    {
    }
}

public static class Ext
{
    public static string GetString(this Color c) => $"({c.r}, {c.g}, {c.b}, {c.a})";
    public static string GetStringComplex(this Color c) => $"({c.r:000}, {c.g:000}, {c.b:000}, {c.a:000})";
}