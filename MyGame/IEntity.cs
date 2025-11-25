
// ------------------ IEntity.cs ------------------
using System.Drawing;


namespace MyGame
{
interface IEntity
{
Rectangle Bounds { get; }
void Update(double elapsedMs);
void Draw(Graphics g);
}
}