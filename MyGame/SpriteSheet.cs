using System.Drawing;


namespace MyGame
{
class SpriteSheet
{
private Bitmap sheet;
public int FrameWidth => Config.FrameW;
public int FrameHeight => Config.FrameH;


public SpriteSheet(string path)
{
sheet = new Bitmap(path);
}


public Bitmap GetFrame(int col, int row)
{
var bmp = new Bitmap(FrameWidth, FrameHeight);
using (var g = Graphics.FromImage(bmp))
{
var src = new Rectangle(col * FrameWidth, row * FrameHeight, FrameWidth, FrameHeight);
g.DrawImage(sheet, new Rectangle(0, 0, FrameWidth, FrameHeight), src, GraphicsUnit.Pixel);
}
return bmp;
}
}
}