using System.Drawing;

namespace HookLib.Models;

public class MouseValue
{
    public Point Point { get; } = new Point(-1, -1);
    public int Data { get; } = -1;

    public MouseValue(Point point, int data)
    {
        Point = point;
        Data = data;
    }
}