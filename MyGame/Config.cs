namespace MyGame
{
static class Config
{
// Ruta al spritesheet. En el contenedor está en /mnt/data/player.png
// Puedes dejar esta ruta absoluta o copiar player.png al root del proyecto y usar "player.png"
    public static string SpriteSheetPath = "player.png";


public const int FrameW = 48;
public const int FrameH = 48;
public const int Columns = 9;
public const int Rows = 8;
public const int Scale = 2;
}
}