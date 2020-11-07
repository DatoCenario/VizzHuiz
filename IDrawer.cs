using System;
using System.Threading;
using System.Numerics;

public interface IDrawer
{
    public void DrawLine(Vector2 start, Vector2 end);
    public void DrawString(string str, Vector2 pos);
}