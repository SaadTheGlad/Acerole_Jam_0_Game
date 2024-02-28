using Godot;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

public static class Raycast
{
    #region 3D Methods
    //using a from and to and allowing ignoring RID
    public static void Raycast3D(Vector3 from, Vector3 to, out HitInfo hitInfo, PhysicsDirectSpaceState3D spaceState,Rid toIgnore)
	{

		//use global coordinates, not local to node
		var query = PhysicsRayQueryParameters3D.Create(from, to);
		query.Exclude = new Godot.Collections.Array<Rid> { toIgnore };
		var result = spaceState.IntersectRay(query);

		hitInfo = new HitInfo();

		if (result.Count > 0)
		{
			hitInfo.point = (Vector3)result["position"];
			hitInfo.normal = (Vector3)result["normal"];
			hitInfo.colliderVariant = result["collider"];
			hitInfo.RID = toIgnore;
			hitInfo.shapeIndex = (int)result["shape"];

			hitInfo.collider = hitInfo.colliderVariant.As<GodotObject>();
			hitInfo.colliderNode = hitInfo.collider as Node3D;
		}
		else
		{
			hitInfo = null;
		}

	}

	//using a ray and allowing ignoring RID
	public static void Raycast3D(Ray ray, out HitInfo hitInfo, PhysicsDirectSpaceState3D spaceState, float maxRayLength, Rid toIgnore)
	{
		Vector3 from = ray.origin;
		Vector3 to = from + ray.direction * maxRayLength;

		//use global coordinates, not local to node
		var query = PhysicsRayQueryParameters3D.Create(from, to);
		query.Exclude = new Godot.Collections.Array<Rid> { toIgnore };
		var result = spaceState.IntersectRay(query);

		hitInfo = new HitInfo();

		if (result.Count > 0)
		{
			hitInfo.point = (Vector3)result["position"];
			hitInfo.normal = (Vector3)result["normal"];
			hitInfo.colliderVariant = result["collider"];
			hitInfo.RID = toIgnore;
			hitInfo.shapeIndex = (int)result["shape"];

			hitInfo.collider = hitInfo.colliderVariant.As<GodotObject>();
			hitInfo.colliderNode = hitInfo.collider as Node3D;
		}
		else
		{
			hitInfo = null;
		}

	}

	//using a from and to and not caring bout RID
	public static void Raycast3D(Vector3 from, Vector3 to, out HitInfo hitInfo, PhysicsDirectSpaceState3D spaceState)
	{

		//use global coordinates, not local to node
		var query = PhysicsRayQueryParameters3D.Create(from, to);
		var result = spaceState.IntersectRay(query);

		hitInfo = new HitInfo();

		if (result.Count > 0)
		{
			hitInfo.point = (Vector3)result["position"];
			hitInfo.normal = (Vector3)result["normal"];
			hitInfo.colliderVariant = result["collider"];
			hitInfo.shapeIndex = (int)result["shape"];

			hitInfo.collider = hitInfo.colliderVariant.As<GodotObject>();
			hitInfo.colliderNode = hitInfo.collider as Node3D;
		}
		else
		{
			hitInfo = null;
		}

	}

	//using a ray and not caring about RID
	public static void Raycast3D(Ray ray, out HitInfo hitInfo, PhysicsDirectSpaceState3D spaceState, float maxRayLength)
	{
		Vector3 from = ray.origin;
		Vector3 to = from + ray.direction * maxRayLength;

		//use global coordinates, not local to node
		var query = PhysicsRayQueryParameters3D.Create(from, to);
		var result = spaceState.IntersectRay(query);

		hitInfo = new HitInfo();

		if (result.Count > 0)
		{
			hitInfo.point = (Vector3)result["position"];
			hitInfo.normal = (Vector3)result["normal"];
			hitInfo.colliderVariant = result["collider"];
			hitInfo.shapeIndex = (int)result["shape"];

			hitInfo.collider = hitInfo.colliderVariant.As<GodotObject>();
			hitInfo.colliderNode = hitInfo.collider as Node3D;
		}
		else
		{
			hitInfo = null;
		}

	}
    #endregion

    #region 2D Methods
    //using a from and to and allowing ignoring RID
    public static void Raycast2D(Vector2 from, Vector2 to, out HitInfo2D hitInfo, PhysicsDirectSpaceState2D spaceState, Rid toIgnore)
    {

        //use global coordinates, not local to node
        var query = PhysicsRayQueryParameters2D.Create(from, to);
        query.Exclude = new Godot.Collections.Array<Rid> { toIgnore };
        var result = spaceState.IntersectRay(query);

        hitInfo = new HitInfo2D();

        if (result.Count > 0)
        {
            hitInfo.point = (Vector2)result["position"];
            hitInfo.normal = (Vector2)result["normal"];
            hitInfo.colliderVariant = result["collider"];
            hitInfo.RID = toIgnore;
            hitInfo.shapeIndex = (int)result["shape"];

            hitInfo.collider = hitInfo.colliderVariant.As<GodotObject>();
            hitInfo.colliderNode = hitInfo.collider as Node2D;
        }
        else
        {
            hitInfo = null;
        }

    }

    //using a ray and allowing ignoring RID
    public static void Raycast2D(Ray2D ray, out HitInfo2D hitInfo, PhysicsDirectSpaceState2D spaceState, float maxRayLength, Rid toIgnore)
    {
        Vector2 from = ray.origin;
        Vector2 to = from + ray.direction * maxRayLength;

        //use global coordinates, not local to node
        var query = PhysicsRayQueryParameters2D.Create(from, to);
        query.Exclude = new Godot.Collections.Array<Rid> { toIgnore };
        var result = spaceState.IntersectRay(query);

        hitInfo = new HitInfo2D();

        if (result.Count > 0)
        {
            hitInfo.point = (Vector2)result["position"];
            hitInfo.normal = (Vector2)result["normal"];
            hitInfo.colliderVariant = result["collider"];
            hitInfo.RID = toIgnore;
            hitInfo.shapeIndex = (int)result["shape"];

            hitInfo.collider = hitInfo.colliderVariant.As<GodotObject>();
            hitInfo.colliderNode = hitInfo.collider as Node2D;
        }
        else
        {
            hitInfo = null;
        }

    }

    //using a from and to and not caring bout RID
    public static void Raycast2D(Vector2 from, Vector2 to, out HitInfo2D hitInfo, PhysicsDirectSpaceState2D spaceState)
    {

        //use global coordinates, not local to node
        var query = PhysicsRayQueryParameters2D.Create(from, to);
        var result = spaceState.IntersectRay(query);

        hitInfo = new HitInfo2D();

        if (result.Count > 0)
        {
            hitInfo.point = (Vector2)result["position"];
            hitInfo.normal = (Vector2)result["normal"];
            hitInfo.colliderVariant = result["collider"];
            hitInfo.shapeIndex = (int)result["shape"];

            hitInfo.collider = hitInfo.colliderVariant.As<GodotObject>();
            hitInfo.colliderNode = hitInfo.collider as Node2D;
        }
        else
        {
            hitInfo = null;
        }

    }

    //using a ray and not caring about RID
    public static void Raycast2D(Ray2D ray, out HitInfo2D hitInfo, PhysicsDirectSpaceState2D spaceState, float maxRayLength)
    {
        Vector2 from = ray.origin;
        Vector2 to = from + ray.direction * maxRayLength;

        //use global coordinates, not local to node
        var query = PhysicsRayQueryParameters2D.Create(from, to);
        var result = spaceState.IntersectRay(query);

        hitInfo = new HitInfo2D();

        if (result.Count > 0)
        {
            hitInfo.point = (Vector2)result["position"];
            hitInfo.normal = (Vector2)result["normal"];
            hitInfo.colliderVariant = result["collider"];
            hitInfo.shapeIndex = (int)result["shape"];

            hitInfo.collider = hitInfo.colliderVariant.As<GodotObject>();
            hitInfo.colliderNode = hitInfo.collider as Node2D;
        }
        else
        {
            hitInfo = null;
        }

    }
    #endregion
}

[Serializable]
public class HitInfo
{
	public Vector3 point;
	public Vector3 normal;
	public Variant colliderVariant;
	public Rid RID;
	public int shapeIndex;

	public GodotObject collider;
	public Node3D colliderNode;
}

[Serializable]
public class HitInfo2D
{
    public Vector2 point;
    public Vector2 normal;
    public Variant colliderVariant;
    public Rid RID;
    public int shapeIndex;

    public GodotObject collider;
    public Node2D colliderNode;
}

