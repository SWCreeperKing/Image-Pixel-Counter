using System.Numerics;
using Raylib_CsLo;
using RayWrapper;
using RayWrapper.Objs;
using RayWrapper.Vars;

namespace PixelCounter;

public class PixelCounter : GhostObject
{
    public Dictionary<Color, int> pixelCounter = new();
    public List<(Color, int)> orderedPixelCounter = new();

    private Image _img;
    private Texture _texture;
    private Rectangle _imgRec;
    private Rectangle _newImgRect;
    private Color[] _imageData;
    private Vector2 _imageSize;
    private bool _isDone;
    private bool _lastOrder;
    private bool _closed;

    public PixelCounter(string file, string name)
    {
        Button close = new(new Vector2(20, 70), " X ")
        {
            style =
            {
                BackColor = Raylib.RED
            }
        };
        close.Clicked += () =>
        {
            new AlertConfirm("Close Tab", "Are you sure you want to close this tab?")
            {
                onResult = r =>
                {
                    if (r is "yes") Program.tb.RemoveTab(name);
                    Raylib.UnloadImage(_img);
                    Raylib.UnloadTexture(_texture);
                    _closed = true;
                }
            }.Show();
        };

        _img = Raylib.LoadImage(file);
        _texture = _img.Texture();

        unsafe
        {
            _imageData = new Color[_img.height * _img.width];
            var data = Raylib.LoadImageColors(_img);
            _imageSize = _img.Size();
            for (var y = 0; y < _imageSize.Y; y++)
            for (var x = 0; x < _imageSize.X; x++)
            {
                _imageData[y * (int) _imageSize.X + x] = data[y * (int) _imageSize.X + x];
            }

            Text txt = new(new Actionable<string>(() => $"size of arr: {_imageData.Length} | size: {_imageSize} | colors: {orderedPixelCounter.Count}"),
                new Vector2(325, 70));
            RegisterGameObj(txt);
        }

        PixelCounterItem counterItem = new(250, () => orderedPixelCounter.Count, i => orderedPixelCounter[i]);
        ListView lv = new(new Vector2(20, 120), counterItem, 12);

        var window = new Rectangle(325, 110, 730, 585);
        var scale = Math.Min(window.width / _imageSize.X, window.height / _imageSize.Y);

        _newImgRect = new Rectangle((window.width - _imageSize.X * scale) * .5f,
            (window.height - _imageSize.Y * scale) * 0.5f, _imageSize.X * scale, _imageSize.Y * scale);
        _newImgRect.MoveBy(window.Pos());

        _imgRec = new Rectangle(0, 0, _imageSize.X, _imageSize.Y);

        Button copy = new(new Vector2(20, 680), "Copy Results") { isVisible = new Actionable<bool>(() => _isDone) };
        copy.Clicked += () =>
        {
            if (!_isDone) return;
            var list = string.Join("\n",
                orderedPixelCounter.Select(t => $"{t.Item1.GetStringComplex()}: {t.Item2:###,###}"));
            Raylib.SetClipboardText($"{name}'s make up in pixels:\n(r, g, b, a): amount\n```\n{list}```");
        };

        Task.Run(Count);

        RegisterGameObj(close, lv, copy);
    }

    public async Task Count()
    {
        for (var y = 0; y < _imageSize.Y; y++)
        {
            for (var x = 0; x < _imageSize.X; x++)
            {
                var color = _imageData[y * (int) _imageSize.X + x];
                lock (pixelCounter)
                {
                    if (pixelCounter.ContainsKey(color)) pixelCounter[color]++;
                    else pixelCounter.Add(color, 1);
                }
            }

            await Task.Delay(1);
        }

        _isDone = true;
    }

    protected override void UpdateCall()
    {
        if (_lastOrder) return;
        if (_isDone) _lastOrder = true;
        lock (pixelCounter)
        {
            orderedPixelCounter =
                pixelCounter.OrderByDescending(kv => kv.Value)
                    .ThenBy(kv => kv.Key.r)
                    .ThenBy(kv => kv.Key.g)
                    .ThenBy(kv => kv.Key.b)
                    .ThenBy(kv => kv.Key.a)
                    .Select(kv => (kv.Key, kv.Value)).ToList();
        }
    }

    protected override void RenderCall()
    {
        Raylib.DrawTexturePro(_texture, _imgRec, _newImgRect, Vector2.Zero, 0, Raylib.WHITE);
    }

    ~PixelCounter()
    {
        if (_closed) return;
        Raylib.UnloadImage(_img);
        Raylib.UnloadTexture(_texture);
    }
}