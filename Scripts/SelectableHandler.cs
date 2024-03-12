using Godot;
using System;
using System.Data;

public partial class SelectableHandler : Node2D
{
    [Export] public float gravitySpeed = 10f;
    [Export] public float yGround = 418f;
    [Export] public PackedScene handbook;
    [Export] public Node2D handBookSpawnPoint;
    bool selected;
    Vector2 mouseOffset;

    Node2D handBookObject;
    AnimationPlayer playerGlobal;

    bool hasOpened;
    bool canClose;

    public override void _Process(double delta)
    {
        if(selected)
        {
            FollowMouse();
        }
        else
        {
            FauxGravity((float)delta);
        }


    }

    void FollowMouse()
    {
        GlobalPosition = GetGlobalMousePosition() + mouseOffset;
    }

    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventMouseButton mouseEvent)
        {
            if(mouseEvent.ButtonIndex == MouseButton.Middle)
            {
                if (hasOpened)
                {
                    BringDownHandbook();
                    hasOpened = false;
                }
            }
        }
    }

    void OnArea2DInputEvent(Node viewport, InputEvent @event, int shapeIndex)
    {
        if(@event is InputEventMouseButton mouseEvent)
        {
            if(mouseEvent.ButtonIndex == MouseButton.Left)
            {
                if (mouseEvent.Pressed)
                {
                    //mouse down
                    mouseOffset = GlobalPosition - GetGlobalMousePosition();
                    selected = true;
                }
                else
                {
                    //mouse up
                    selected = false;
                }

            }

            if(mouseEvent.ButtonIndex == MouseButton.Right)
            {
                selected = false;
                if(!hasOpened)
                {
                    BringUpHandbook();
                    hasOpened = true;
                }
            }

        }

    }

    void BringUpHandbook()
    {
        handBookObject = (Node2D)handbook.Instantiate();
        handBookSpawnPoint.AddChild(handBookObject);
        Node2D helper = (Node2D)handBookObject.GetChild(0);
        foreach(var n in helper.GetChildren())
        {
            if(n is AnimationPlayer player)
            {
                playerGlobal = player;
                playerGlobal.Play("Open");
            }
        }
    }

    void BringDownHandbook()
    {
        playerGlobal.Play("Close");
    }

    void FauxGravity(float delta)
    {
        if(GlobalPosition.Y <= yGround)
        {
            Translate(Vector2.Down * gravitySpeed * delta);
        }
    }

}
