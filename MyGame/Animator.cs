using System;
using System.Drawing;


namespace MyGame
{
class Animator
{
private SpriteSheet sheet;
private int currentFrame;
private double timer;
public int TotalFrames { get; private set; } = Config.Columns;
public int Row { get; private set; } = 0;
public double FrameTimeMs { get; set; } = 100;


public Animator(SpriteSheet sheet) { this.sheet = sheet; }


public void SetRow(int row, int totalFrames = Config.Columns)
{
if (row != Row)
{
Row = row;
TotalFrames = totalFrames;
currentFrame = 0;
timer = 0;
}
}


public void Update(double elapsedMs)
{
timer += elapsedMs;
if (timer >= FrameTimeMs)
{
timer = 0;
currentFrame++;
if (currentFrame >= TotalFrames) currentFrame = 0;
}
}


public Bitmap CurrentFrame()
{
return sheet.GetFrame(currentFrame, Row);
}
}
}





