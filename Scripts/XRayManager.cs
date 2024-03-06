using Godot;
using Godot.Collections;
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
    [Export] public Color normalBoneColour;
    [Export] public Color organsHighLightColour;
    [Export] public Color transparentColour;

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

    Sprite2D currentSelectedBone;
    Sprite2D currentSelectedOrgan;

    Vector2 ogSkeletonPos;
    Vector2 ogOrgansPos;
    Vector2 outOfScreenPos = new Vector2(-405.115f, 4878.437f);

    Dictionary<String, Color> nameAndOrganColourDictionary = new Dictionary<string, Color>();

    public override void _Ready()
    {
        ogSkeletonPos = skeleton.Position;
        ogOrgansPos = organs.Position;
    }

    public override void _EnterTree()
    {
        SignalsManager.Instance.DialogueEnded += EnableScan;
        SignalsManager.Instance.Admitted += DisableScan;
    }

    public override void _ExitTree()
    {
        SignalsManager.Instance.DialogueEnded -= EnableScan;
        SignalsManager.Instance.Admitted -= DisableScan;


    }

    public void EnableScan() => canStartScan = true;
    public void DisableScan() => canStartScan = false;

    public void StartScan()
    {
        if(canStartScan)
        {
            helperPlayer.Play("LowerDown");
            if(judgingPlayer.AssignedAnimation == "Open")
            {
                judgingPlayer.Play("Close");
            }

            canStartScan = false;
        }
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
                if(currentSelectedBone != null)
                {
                    currentSelectedBone.SelfModulate = normalBoneColour;
                }

                if(currentSelectedOrgan != null && nameAndOrganColourDictionary.Count != 0)
                {
                    currentSelectedOrgan.SelfModulate = nameAndOrganColourDictionary[currentSelectedOrgan.Name];
                }


                //for skeleton
                if(skeletonArray != null)
                {
                    for (int i = skeletonArrayPublic.Count - 1; i > -1; i--)
                    {
                        Vector2 mousePos = skeletonArrayPublic[i].ToLocal(inputEventMouse.GlobalPosition);
                        if (skeletonArrayPublic[i].GetRect().HasPoint(mousePos) && skeletonArrayPublic[i].IsPixelOpaque(mousePos))
                        {
                            currentSelectedBone = skeletonArrayPublic[i];
                            skeletonArrayPublic[i].SelfModulate = highlightColour;
                            return;

                        }

                    }

                }

                // for organs
                if (organsArrayPublic != null)
                {
                    for (int i = organsArrayPublic.Count - 1; i > -1; i--)
                    {

                        Vector2 mousePos = organsArrayPublic[i].ToLocal(inputEventMouse.GlobalPosition);
                        if (organsArrayPublic[i].GetRect().HasPoint(mousePos) && organsArrayPublic[i].IsPixelOpaque(mousePos))
                        {
                            GD.Print(organsArrayPublic[i].Name);
                            currentSelectedOrgan = organsArrayPublic[i];
                            organsArrayPublic[i].SelfModulate = organsHighLightColour;
                            return;

                        }

                    }

                }

            }
        }
    }

    public void ScanSkeleton()
    {
        skeleton.Visible = true;
        skeleton.Position = ogSkeletonPos;
        organs.Visible = false;
        organs.Position = outOfScreenPos;

        if (hasScannedSkeleton)
        {
            return;
        }

        float randomValue = random.RandfRange(0f, 100f);


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

        if (randomValue <= percentageOfAbberation/*20%*/)
        {
            int randomIndex = RandomSelectionSkeleton();
            while (skeletonArrayPublic[randomIndex].IsInGroup("Avoid"))
            {
                randomIndex = RandomSelectionSkeleton();
            }
            currentSkeletonIndex = randomIndex;
            Sprite2D current = skeletonArrayPublic[randomIndex];
            current.SelfModulate = veryTranslucentColour;
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
        skeleton.Position = outOfScreenPos;
        organs.Visible = true;
        organs.Position = ogOrgansPos;

        if (hasScannedOrgans)
        {
            return;
        }

        float randomValue = random.RandfRange(0f, 100f);

        
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
                        nameAndOrganColourDictionary.Add(sprite.Name, sprite.SelfModulate);
                    }
                    else
                    {
                        foreach (var secondaryBone in bone.GetChildren())
                        {
                            if (secondaryBone is Sprite2D spriteSecondary)
                            {
                                spriteSecondary = (Sprite2D)secondaryBone;
                                organsArrayPublic.Add(spriteSecondary);
                                nameAndOrganColourDictionary.Add(spriteSecondary.Name, spriteSecondary.SelfModulate);

                            }
                        }
                    }
                }

            }
            if (node is Sprite2D spriteHigher)
            {
                organsArrayPublic.Add(spriteHigher);
                nameAndOrganColourDictionary.Add(spriteHigher.Name, spriteHigher.SelfModulate);

            }
        }

        organsArray = organsArrayPublic.Duplicate();
        organsArrayFull = organsArray.Duplicate();
        organsArray.Shuffle();

        if (randomValue <= percentageOfAbberation)
        {
            int randomIndex = RandomSelectionOrgans();
            while (organsArrayPublic[randomIndex].IsInGroup("Avoid"))
            {
                randomIndex = RandomSelectionOrgans();
            }
            currentOrgansIndex = randomIndex;
            Sprite2D current = organsArrayPublic[randomIndex];
            current.SelfModulate = veryTranslucentColour;
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
