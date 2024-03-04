using Godot;
using System;

public partial class XRayManager : Node
{
    [Export] public AnimationPlayer helperPlayer;
    [Export] public AnimationPlayer scanScreenPlayer;
    [Export] public Node2D xRayArea;
    [Export] public Node2D skeleton;
    //Make an enum for all the stuff with children and use it in line 32 (if statement)

    private Godot.Collections.Array<Sprite2D> arrayPublic = new Godot.Collections.Array<Sprite2D>();
    private Godot.Collections.Array<Sprite2D> array;
    private Godot.Collections.Array<Sprite2D> arrayFull;

    public void StartScan()
    {
        helperPlayer.Play("LowerDown");
    }

    public void PullUpScanScreen()
    {
        scanScreenPlayer.Play("SlideIn");
    }

    public void Scan()
    {
        Node2D parent = (Node2D)skeleton.GetParent();
        parent.Visible = true;
        foreach(var node in skeleton.GetChildren())
        {
            if (node.Name == "Ribs")
            {
                foreach(var rib in node.GetChildren())
                {
                    Sprite2D ribSprite = (Sprite2D)rib;
                    arrayPublic.Add(ribSprite);
                }

            }
        }

        array = arrayPublic.Duplicate();

        arrayFull = array.Duplicate();
        array.Shuffle();

        RandomSelection();
    }



    void RandomSelection()
    {
        //Shuffle bag random selection
        if (array.Count == 0)
        {
            array = arrayFull.Duplicate();
            array.Shuffle();
        }

        var randomObject = array[0];
        array.RemoveAt(0);



        uint randomIndex = GD.Randi() % (uint)arrayPublic.Count;
        GD.Print(arrayPublic[(int)randomIndex].Name);
    }
}
