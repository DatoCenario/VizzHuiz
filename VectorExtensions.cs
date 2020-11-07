using System;
using System.Numerics;

static class VectorExtensions
{
    public static Vector2 Rotate(this Vector2 vector, float angle)
    {
        float cos = (float)Math.Cos(angle);
        float sin = (float)Math.Sin(angle);

        return new Vector2(vector.X * cos - vector.Y * sin, vector.X * sin + vector.Y * cos);
    }
}