using Godot;
using System;

[Serializable]
public partial struct Ray2D
{
    private Vector2 _origin;
    private Vector2 _direction;

    public Ray2D(Vector2 origin, Vector2 directon)
    {
        _origin = origin;
        _direction = directon.Normalized();
    }

    public Vector2 origin
    {
        get { return _origin; }
        set { _origin = value; }
    }

    public Vector2 direction
    {
        get { return _direction; }
        set { _direction = value.Normalized(); }
    }

    public Vector2 GetPoint(float distance)
    {
        return _origin + _direction * distance;
    }

}

