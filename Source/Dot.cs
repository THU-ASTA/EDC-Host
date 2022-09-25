using System;
using OpenCvSharp;

namespace EdcHost;

/// <summary>
/// A 2-dimenstion dot
/// </summary>
public class Dot
{
    /// <summary>
    /// The x-coordinate of the dot
    /// </summary>
    public int x
    {
        get => this._x;
        set => this._x = value;
    }
    /// <summary>
    /// The y-coordinate of the dot
    /// </summary>
    public int y
    {
        get => this._y;
        set => this._y = value;
    }

    private int _x;
    private int _y;


    /// <summary>
    /// Get the Euclidean distance between two dots.
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns>The distance</returns>
    public static int Distance(Dot A, Dot B)
    {
        return (int)Math.Sqrt((A.x - B.x) * (A.x - B.x)
            + (A.y - B.y) * (A.y - B.y));
    }

    public Dot(int x, int y)
    {
        this._x = x;
        this._y = y;
    }

    public Dot(Point point)
    {
        this._x = point.X;
        this._y = point.Y;
    }

    public static bool operator ==(Dot a, Dot b)
    {
        return (a.x == b.x) && (a.y == b.y);
    }

    public static bool operator !=(Dot a, Dot b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    /// <summary>
    /// Convert the dot to a Point object
    /// </summary>
    /// <returns>The Point object</returns>
    public Point ToPoint()
    {
        return new Point(this._x, this._y);
    }
}