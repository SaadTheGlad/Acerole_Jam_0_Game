using Godot;
using System;

public partial class XRayManager : Node
{
    [Export] public AnimationPlayer helperPlayer;
    [Export] public AnimationPlayer scanScreenPlayer;
    [Export] public Node2D xRayArea;
    [Export] public Node2D skeleton;
    //Make an enum for all the stuff with children and use it in line 32 (if statement)
    [Export] public BoneGroups boneGroups;
    [Export] public Color tempHighlightColour;
    [Export] public Color white;

    private Godot.Collections.Array<Sprite2D> arrayPublic = new Godot.Collections.Array<Sprite2D>();
    private Godot.Collections.Array<Sprite2D> array;
    private Godot.Collections.Array<Sprite2D> arrayFull;

    bool hasScanned = false;
    int currentIndex = -1000;

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
        if (true)
        {
            if (currentIndex != -1000)
            {
                arrayPublic[currentIndex].SelfModulate = white;
            }
            
            Node2D parent = (Node2D)skeleton.GetParent();
            parent.Visible = true;
            foreach (var node in skeleton.GetChildren())
            {
                if (true/*node.Name == boneGroups.ToString()*/)
                {
                    foreach (var bone in node.GetChildren())
                    {
                        if(bone is Sprite2D sprite)
                        {
                            sprite = (Sprite2D)bone;
                            arrayPublic.Add(sprite);
                            //GD.Print(sprite.Name);
                        }
                        else
                        {
                            GD.Print(bone.Name);
                            foreach(var secondaryBone in bone.GetChildren())
                            {
                                if(secondaryBone is Sprite2D spriteSecondary)
                                {
                                    spriteSecondary = (Sprite2D)secondaryBone;
                                    arrayPublic.Add(spriteSecondary);
                                    //GD.Print(spriteSecondary.Name);
                                }
                            }
                        }
                    }

                }
                if(node is Sprite2D spriteHigher)
                {
                    arrayPublic.Add(spriteHigher);
                    GD.Print(spriteHigher.Name);
                }
            }



            array = arrayPublic.Duplicate();

            arrayFull = array.Duplicate();
            array.Shuffle();



            int randomIndex = RandomSelection();
            //while (arrayPublic[randomIndex].Name == "Sternum")
            //{
            //    randomIndex = RandomSelection();
            //}
            currentIndex = randomIndex;
            Sprite2D current = arrayPublic[randomIndex];
            current.SelfModulate = tempHighlightColour;


            hasScanned = true;
        }


        

    }



    int RandomSelection()
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
        return (int)randomIndex;
    }
}

public enum BoneGroups
{ 
    All,
    Neck,
    Skull,
    MinuesOneBackbone,
    Ribs,
    Backbone,
    Pelvis,
    Left,
    Right
}

public enum Avoid
{
    Sternum,
}
