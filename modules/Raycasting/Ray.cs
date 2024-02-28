using Godot;
using System;

[Serializable]
public partial struct Ray
{
	private Vector3 _origin;
	private Vector3 _direction;

	public Ray(Vector3 origin, Vector3 directon)
	{
		_origin = origin;
		_direction = directon.Normalized();
	}

	public Vector3 origin
	{
		get { return _origin; }
		set { _origin = value; }
	}

	public Vector3 direction
	{
		get { return _direction; }
		set { _direction = value.Normalized(); }
	}

	public Vector3 GetPoint(float distance)
	{
		return _origin + _direction * distance;
	}

}

