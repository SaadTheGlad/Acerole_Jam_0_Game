using Godot;
using System;
using System.Security;

public partial class XRayManager : Node
{
    [Export(PropertyHint.Range, "0,100,")] public float percentageOfAbberation = 20f;
    RandomNumberGenerator random = new RandomNumberGenerator();

    [ExportGroup("Animation Players")]
    [Export] public AnimationPlayer helperPlayer;
    [Export] public AnimationPlayer scanScreenPlayer;
    [Export] public AnimationPlayer judgingPlayer;
    [ExportGroup("Main X-Ray Variables")]
    [Export] public Node2D xRayArea;
    [Export] public Node2D skeleton;
    [Export] public Node2D organs;
    [Export] public Color veryTranslucentColour;
    [Export] public Color highlightColour;

    private Godot.Collections.Array<Sprite2D> skeletonArrayPublic = new Godot.Collections.Array<Sprite2D>();
    private Godot.Collections.Array<Sprite2D> skeletonArray;
    private Godot.Collections.Array<Sprite2D> skeletonArrayFull;

    private Godot.Collections.Array<Sprite2D> organsArrayPublic = new Godot.Collections.Array<Sprite2D>();
    private Godot.Collections.Array<Sprite2D> organsArray;
    private Godot.Collections.Array<Sprite2D> organsArrayFull;

    bool canStartScan;
    bool hasScannedSkeleton = false;
    bool hasScannedOrgans = false;
    int currentSkeletonIndex = -1000;
    int currentOrgansIndex = -1000;

    bool isOnSkeleton = false;

    public override void _EnterTree()
    {
        SignalsManager.Instance.DialogueEnded += EnableScan;
    }

    public override void _ExitTree()
    {
        SignalsManager.Instance.DialogueEnded -= EnableScan;
    }

    public void EnableScan() => canStartScan = true;

    public void StartScan()
    {
        if(canStartScan)
         helperPlayer.Play("LowerDown");
    }

    public void PullUpScanScreen()
    {
        scanScreenPlayer.Play("SlideIn");
    }

    public void CloseScanScreen()
    {
        scanScreenPlayer.Play("SlideOut");
    }

    public void OpenJudging()
    {
        judgingPlayer.Play("Open");
        helperPlayer.Play("RiseUp");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton inputEventMouse)
        {
            if (inputEventMouse.Pressed && inputEventMouse.ButtonIndex == MouseButton.Left)
            {
                //for sprites
                if(skeletonArray != null)
                {
                    foreach (Sprite2D bone in skeletonArrayPublic)
                    {
                        //We're looking for this sprite, check if it's opaque and if not break the loop;
                        if (skeletonArrayPublic.IndexOf(bone) == currentSkeletonIndex)
                        {
                            Vector2 mousePos = bone.ToLocal(inputEventMouse.GlobalPosition);

                            if (bone.GetRect().HasPoint(mousePos) && bone.IsPixelOpaque(mousePos))
                            {
                                bone.Modulate = highlightColour;
                            }
                            break;
                        }



                    }
                }

                if (organsArray!= null)
                {
                    foreach (Sprite2D organ in organsArrayPublic)
                    {
                        //We're looking for this sprite, check if it's opaque and if not break the loop;
                        if (organsArrayPublic.IndexOf(organ) == currentOrgansIndex)
                        {
                            Vector2 mousePos = organ.ToLocal(inputEventMouse.GlobalPosition);

                            if (organ.GetRect().HasPoint(mousePos) && organ.IsPixelOpaque(mousePos))
                            {
                                organ.Modulate = highlightColour;
                            }
                            break;
                        }



                    }
                }

            }
        }
    }

    public void ScanSkeleton()
    {
        organs.Visible = false;
        skeleton.Visible = true;

        if (hasScannedSkeleton)
        {
            return;
        }

        float randomValue = random.RandfRange(0f, 100f);
        if (randomValue <= percentageOfAbberation/*20%*/)
        {

            foreach (var node in skeleton.GetChildren())
            {
                if (true/*node.Name == boneGroups.ToString()*/)
                {
                    foreach (var bone in node.GetChildren())
                    {
                        if (bone is Sprite2D sprite)
                        {
                            sprite = (Sprite2D)bone;
                            skeletonArrayPublic.Add(sprite);
                        }
                        else
                        {
                            foreach (var secondaryBone in bone.GetChildren())
                            {
                                if (secondaryBone is Sprite2D spriteSecondary)
                                {
                                    spriteSecondary = (Sprite2D)secondaryBone;
                                    skeletonArrayPublic.Add(spriteSecondary);
                                }
                            }
                        }
                    }

                }
                if (node is Sprite2D spriteHigher)
                {
                    skeletonArrayPublic.Add(spriteHigher);
                }
            }

            skeletonArray = skeletonArrayPublic.Duplicate();
            skeletonArrayFull = skeletonArray.Duplicate();
            skeletonArray.Shuffle();

            int randomIndex = RandomSelectionSkeleton();
            while (skeletonArrayPublic[randomIndex].IsInGroup("Avoid"))
            {
                randomIndex = RandomSelectionSkeleton();
            }
            currentSkeletonIndex = randomIndex;
            Sprite2D current = skeletonArrayPublic[randomIndex];
            current.Modulate = veryTranslucentColour;
            hasScannedSkeleton = true;
        }
        else
        {
            GD.Print("Safe Skeleton");
        }

        GD.Print("Skeleton percentage: " + randomValue + "%");
    }

    public void ScanOrgans()
    {
        skeleton.Visible = false;
        organs.Visible = true;

        if (hasScannedOrgans)
        {
            return;
        }

        float randomValue = random.RandfRange(0f, 100f);
        if(randomValue <= percentageOfAbberation)
        {
            foreach (var node in organs.GetChildren())
            {
                if (true/*node.Name == boneGroups.ToString()*/)
                {
                    foreach (var bone in node.GetChildren())
                    {
                        if (bone is Sprite2D sprite)
                        {
                            sprite = (Sprite2D)bone;
                            organsArrayPublic.Add(sprite);
                        }
                        else
                        {
                            foreach (var secondaryBone in bone.GetChildren())
                            {
                                if (secondaryBone is Sprite2D spriteSecondary)
                                {
                                    spriteSecondary = (Sprite2D)secondaryBone;
                                    organsArrayPublic.Add(spriteSecondary);
                                }
                            }
                        }
                    }

                }
                if (node is Sprite2D spriteHigher)
                {
                    organsArrayPublic.Add(spriteHigher);
                }
            }

            organsArray = organsArrayPublic.Duplicate();
            organsArrayFull = organsArray.Duplicate();
            organsArray.Shuffle();

            int randomIndex = RandomSelectionOrgans();
            while (organsArrayPublic[randomIndex].IsInGroup("Avoid"))
            {
                randomIndex = RandomSelectionOrgans();
            }
            currentOrgansIndex = randomIndex;
            Sprite2D current = organsArrayPublic[randomIndex];
            current.Modulate = veryTranslucentColour;
            hasScannedOrgans = true;
        }
        else
        {
            GD.Print("Safe Organs");
        }

        GD.Print("Organs percentage: " + randomValue + "%");
    }

    int RandomSelectionSkeleton()
    {
        //Shuffle bag random selection
        if (skeletonArray.Count == 0)
        {
            skeletonArray = skeletonArrayFull.Duplicate();
            skeletonArray.Shuffle();
        }

        var randomObject = skeletonArray[0];
        skeletonArray.RemoveAt(0);

        uint randomIndex = GD.Randi() % (uint)skeletonArrayPublic.Count;
        return (int)randomIndex;
    }

    int RandomSelectionOrgans()
    {
        //Shuffle bag random selection
        if (organsArray.Count == 0)
        {  
            organsArray = organsArrayFull.Duplicate();
            organsArray.Shuffle();
        }

        var randomObject = organsArray[0];
        organsArray.RemoveAt(0);

        uint randomIndex = GD.Randi() % (uint)organsArrayPublic.Count;
        return (int)randomIndex;
    }
}
