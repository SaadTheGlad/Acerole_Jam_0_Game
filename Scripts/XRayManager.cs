using Godot;
using Godot.Collections;
using System;
using System.Security;

[GlobalClass]
public partial class XRayManager : Node
{
    [Export(PropertyHint.Range, "0,100,")] public float probabilityOfGeneralAbberation = 20f;
    [Export(PropertyHint.Range, "0,100,")] public float probabilityOfSkeletonToOrgansAbberation = 50f;
    //[Export(PropertyHint.Range, "0,100,")] public float probabilityOfOrganAbberation = 50f;
    RandomNumberGenerator random = new RandomNumberGenerator();

    [ExportGroup("Animation Players")]
    [Export] public AnimationPlayer helperPlayer;
    [Export] public AnimationPlayer scanScreenPlayer;
    [Export] public AnimationPlayer judgingPlayer;
    [ExportGroup("Main X-Ray Variables")]
    [Export] public Node2D xRayArea;
    [Export] public Node2D skeleton;
    [Export] public Node2D organs;
    [ExportCategory("Colour")]
    [Export] public Color veryTranslucentColour;
    [Export] public Color highlightColour;
    [Export] public Color normalBoneColour;
    [Export] public Color organsHighLightColour;
    [Export] public Color transparentColour;
    [ExportGroup("Misc")]
    [Export] public Button selectAnomalyButton;

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

    public Sprite2D currentSelectedBone;
    public Sprite2D currentSelectedOrgan;    
    public Sprite2D anomalyBone;
    public Sprite2D anomalyOrgan;

    Vector2 ogSkeletonPos;
    Vector2 ogOrgansPos;
    Vector2 OUTOFSCREENPOS = new Vector2(-405.115f, 4878.437f);

    Dictionary<String, Color> nameAndOrganColourDictionary = new Dictionary<string, Color>();

    bool isDG;
    bool canRoll = true;
    bool skeletonOrOrgan = false;

    public override void _Ready()
    {
        ogSkeletonPos = skeleton.Position;
        ogOrgansPos = organs.Position;
    }

    public override void _EnterTree()
    {
        SignalsManager.Instance.DialogueEnded += EnableScan;
        SignalsManager.Instance.Admitted += DisableScan;
        SignalsManager.Instance.ResetScan += ResetScan;
        SignalsManager.Instance.CreatedNPC += RollAbberation;
        SignalsManager.Instance.EnableNPC += EnableRolling;

    }

    public override void _ExitTree()
    {
        SignalsManager.Instance.DialogueEnded -= EnableScan;
        SignalsManager.Instance.Admitted -= DisableScan;
        SignalsManager.Instance.ResetScan -= ResetScan;
        SignalsManager.Instance.CreatedNPC -= RollAbberation;
        SignalsManager.Instance.EnableNPC -= EnableRolling;

    }

    public void EnableRolling() => canRoll = true;

    public void RollAbberation()
    {
        if(canRoll)
        {
            float randomGeneralValue = random.RandiRange(0, 100);
            if (randomGeneralValue <= probabilityOfGeneralAbberation)
            {
                isDG = true;

                float randomSpecificValue = random.RandiRange(0, 100);
                if (randomSpecificValue <= probabilityOfSkeletonToOrgansAbberation)
                {
                    skeletonOrOrgan = false;
                    GD.Print("Abberation in Organs");

                }
                else
                {
                    skeletonOrOrgan = true;
                    GD.Print("Abberation in Skeleton");

                }

            }
            else
            {
                isDG = false;
            }
            GD.Print("Doppelganger State: " + isDG);



            canRoll = false;


        }


    }

    public void ResetScan()
    {
        skeleton.Visible = false;
        organs.Visible = false;
        selectAnomalyButton.Visible = false;
        hasScannedOrgans = false;
        hasScannedSkeleton = false;
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
                            selectAnomalyButton.Visible = true;
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
                            currentSelectedOrgan = organsArrayPublic[i];
                            selectAnomalyButton.Visible = true;
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
        organs.Position = OUTOFSCREENPOS;

        if (hasScannedSkeleton)
        {
            return;
        }

        int randomValue = random.RandiRange(0, 100);


        foreach (var node in skeleton.GetChildren())
        {
            if (true)
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

        if (skeletonOrOrgan && isDG)
        {
            int randomIndex = RandomSelectionSkeleton();
            while (skeletonArrayPublic[randomIndex].IsInGroup("Avoid"))
            {
                randomIndex = RandomSelectionSkeleton();
            }
            currentSkeletonIndex = randomIndex;
            Sprite2D current = skeletonArrayPublic[randomIndex];
            current.SelfModulate = veryTranslucentColour;
            anomalyBone = current;
            GD.Print(current.Name);
            hasScannedSkeleton = true;
        }

    }
    public void ScanOrgans()
    {
        skeleton.Visible = false;
        skeleton.Position = OUTOFSCREENPOS;
        organs.Visible = true;
        organs.Position = ogOrgansPos;

        if (hasScannedOrgans)
        {
            return;
        }

        int randomValue = random.RandiRange(0, 100);

        
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
                        if(!nameAndOrganColourDictionary.ContainsKey(sprite.Name))
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
                                if (!nameAndOrganColourDictionary.ContainsKey(spriteSecondary.Name))
                                    nameAndOrganColourDictionary.Add(spriteSecondary.Name, spriteSecondary.SelfModulate);

                            }
                        }
                    }
                }

            }
            if (node is Sprite2D spriteHigher)
            {
                organsArrayPublic.Add(spriteHigher);
                if (!nameAndOrganColourDictionary.ContainsKey(spriteHigher.Name))
                    nameAndOrganColourDictionary.Add(spriteHigher.Name, spriteHigher.SelfModulate);

            }
        }

        organsArray = organsArrayPublic.Duplicate();
        organsArrayFull = organsArray.Duplicate();
        organsArray.Shuffle();

        if (!skeletonOrOrgan && isDG)
        {
            int randomIndex = RandomSelectionOrgans();
            while (organsArrayPublic[randomIndex].IsInGroup("Avoid"))
            {
                randomIndex = RandomSelectionOrgans();
            }
            currentOrgansIndex = randomIndex;
            Sprite2D current = organsArrayPublic[randomIndex];
            current.SelfModulate = veryTranslucentColour;
            anomalyOrgan = current;
            GD.Print(current.Name);
            hasScannedOrgans = true;
        }

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
