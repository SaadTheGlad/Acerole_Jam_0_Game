using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Security;

[GlobalClass]
public partial class XRayManager : Node
{
    [Export] public NPCCreator npcCreator;
    [Export] public DialogueNPCSignal signal;

    [ExportGroup("Probabilities")]
    [Export(PropertyHint.Range, "0,100,")] public float probabilityOfGeneralAbberation = 20f;
    [Export(PropertyHint.Range, "0,90,")] public float probabilityOfSkeletonToOrgansAbberation = 45f;

    RandomNumberGenerator random = new RandomNumberGenerator();

    [ExportGroup("Animation Players")]
    [Export] public AnimationPlayer helperPlayer;
    [Export] public AnimationPlayer scanScreenPlayer;
    [Export] public AnimationPlayer judgingPlayer;
    [ExportGroup("Main X-Ray Variables")]
    [Export] public Node2D xRayArea;
    [Export] public Node2D maleSkeleton;
    [Export] public Node2D maleOrgans;
    [Export] public Node2D femaleSkeleton;
    [Export] public Node2D femaleOrgans;
    [ExportCategory("Colour")]
    [Export] public Color veryTranslucentColour;
    [Export] public Color highlightColour;
    [Export] public Color normalBoneColour;
    [Export] public Color organsHighLightColour;
    [Export] public Color transparentColour;
    [Export] public ColourName[] weirdColours;
    [ExportGroup("Misc")]
    [Export] public Button selectAnomalyButton;

    [Export] public Sprite2D[] hideables;
    Godot.Collections.Dictionary<string, Color> originalColours = new Godot.Collections.Dictionary<string, Color>();

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

    Godot.Collections.Dictionary<string, Color> nameAndOrganDictionary = new Godot.Collections.Dictionary<string, Color>();
    private List<Sprite2D> duplicateObjects = new List<Sprite2D>();

    bool isDG;
    bool canRoll = true;
    bool skeletonOrOrgan = false;
    bool physiqueAbberation = false;
    string offColourOrganName;

    public override void _Ready()
    {
        ogSkeletonPos = maleSkeleton.Position;
        ogOrgansPos = maleOrgans.Position;
    }
    public override void _EnterTree()
    {
        SignalsManager.Instance.DialogueEnded += EnableScan;
        SignalsManager.Instance.Admitted += DisableScan;
        SignalsManager.Instance.DisposedOf += DisableScan;
        SignalsManager.Instance.CameIn += DisableScan;
        SignalsManager.Instance.ResetScan += ResetScan;
        SignalsManager.Instance.CreatedNPC += RollAbberation;
        SignalsManager.Instance.EnableNPC += EnableRolling;

    }
    public override void _ExitTree()
    {
        SignalsManager.Instance.DialogueEnded -= EnableScan;
        SignalsManager.Instance.Admitted -= DisableScan;
        SignalsManager.Instance.DisposedOf -= DisableScan;
        SignalsManager.Instance.CameIn -= DisableScan;
        SignalsManager.Instance.ResetScan -= ResetScan;
        SignalsManager.Instance.CreatedNPC -= RollAbberation;
        SignalsManager.Instance.EnableNPC -= EnableRolling;

    }

    public override void _Process(double delta)
    {
        //GD.Print(signal.isTalking);
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
                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.EnableAbberation);


                float randomSpecificValue = random.RandiRange(0, 100);
                if (randomSpecificValue < 45f)
                {
                    skeletonOrOrgan = false;
                    physiqueAbberation = false;
                    GD.Print("Abberation in Organs");

                }
                else if (randomSpecificValue < 90f)
                {
                    skeletonOrOrgan = true;
                    physiqueAbberation = false;
                    GD.Print("Abberation in Skeleton");

                }
                else if (randomSpecificValue <= 100f)
                {
                    physiqueAbberation = true;
                    GD.Print("Abberation in Physique");
                }

            }
            else
            {
                // make it so there is a chance they can lose an organ here and that's what you base it on
                isDG = false;
                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.DisableAbberation);

            }
            GD.Print("Doppelganger State: " + isDG);



            canRoll = false;


        }


    }
    public void ResetScan()
    {
        //Make the stuf visible and variables and shit
        // do it for female
        maleSkeleton.Visible = false;
        femaleSkeleton.Visible = false;
        maleOrgans.Visible = false;
        femaleOrgans.Visible = false;
        selectAnomalyButton.Visible = false;
        hasScannedOrgans = false;
        hasScannedSkeleton = false;

        EnableObjects();
        
        //Reset bones and organs colours
        foreach(Sprite2D b in skeletonArrayPublic)
        {
            b.SelfModulate = normalBoneColour;
        }
        foreach(Sprite2D o in organsArrayPublic)
        {
            o.SelfModulate = nameAndOrganDictionary[o.Name];
        }

        //Rerotate the things
        foreach (Sprite2D b in skeletonArrayPublic)
        {
            b.RotationDegrees = 0f;
        }
        foreach (Sprite2D o in organsArrayPublic)
        {
            o.RotationDegrees = 0f;
        }

        //Removes the duplicate objects
        foreach (Sprite2D n in duplicateObjects)
        {
            duplicateObjects.Remove(n);
            n.QueueFree();
        }

    }
    public void EnableScan() => canStartScan = true;
    public void DisableScan() => canStartScan = false;
    public void StartScan()
    {
        if(canStartScan && !signal.isTalking)
        {
            helperPlayer.Play("LowerDown");
            if(judgingPlayer.AssignedAnimation == "Open")
            {
                judgingPlayer.Play("Close");
            }

            canStartScan = false;
        }
    }
    public void ChangeOut()
    {
        if(physiqueAbberation)
        {
            SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.ChangeOut);
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

                if(currentSelectedOrgan != null && nameAndOrganDictionary.Count != 0)
                {

                    currentSelectedOrgan.SelfModulate = nameAndOrganDictionary[currentSelectedOrgan.Name];

                }


                //for skeleton
                if(skeletonArray != null)
                {
                    for (int i = skeletonArrayPublic.Count - 1; i > -1; i--)
                    {
                        Vector2 mousePos = skeletonArrayPublic[i].ToLocal(inputEventMouse.GlobalPosition);
                        if (skeletonArrayPublic[i].GetRect().HasPoint(mousePos) && skeletonArrayPublic[i].IsPixelOpaque(mousePos) && skeletonArrayPublic[i].Visible)
                        {
                            currentSelectedBone = skeletonArrayPublic[i];
                            selectAnomalyButton.Visible = true;
                            skeletonArrayPublic[i].SelfModulate = highlightColour;

                            SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.SetName, currentSelectedBone.Name);

                            if(anomalyBone != null)
                            {
                                // in an anomaly case
                                if(currentSelectedBone != null)
                                {
                                    //in an anomaly case and selected a bone
                                    if(currentSelectedBone == anomalyBone)
                                    {
                                        //in an anomaly case and selected the correct bone
                                        SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.HasSelected);
                                        SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.SelectedCorrect);
                                    }
                                    else
                                    {
                                        //in an anomaly case and selected the incorrect bone
                                        SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.HasSelected);
                                        SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.SelectedIncorrect);
                                    }
                                }
                                else
                                {
                                    //in an anomaly case and selected no bone    

                                }

                            }
                            else if(currentSelectedBone != null)
                            {
                                // not an anomaly and selected bone
                                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.HasSelected);
                                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.SelectedIncorrect);
                            }
                            else
                            {
                                //not an anomaly and selected nothing.
                                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.HasSelected);
                                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.SelectedIncorrect);
                            }
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
                        if (organsArrayPublic[i].GetRect().HasPoint(mousePos) && organsArrayPublic[i].IsPixelOpaque(mousePos) && organsArrayPublic[i].Visible)
                        {
                            currentSelectedOrgan = organsArrayPublic[i];
                            selectAnomalyButton.Visible = true;
                            organsArrayPublic[i].SelfModulate = organsHighLightColour;
                            SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.SetName, currentSelectedOrgan.Name);

                            if (anomalyOrgan != null)
                            {
                                // in an anomaly case
                                if (currentSelectedOrgan != null)
                                {
                                    //in an anomaly case and selected a organ
                                    if (currentSelectedOrgan == anomalyOrgan)
                                    {
                                        //in an anomaly case and selected the correct organ
                                        SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.HasSelected);
                                        SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.SelectedCorrect);
                                    }
                                    else
                                    {
                                        //in an anomaly case and selected the incorrect bonorgane
                                        SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.HasSelected);
                                        SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.SelectedIncorrect);
                                    }
                                }
                                else
                                {
                                    //in an anomaly case and selected no organ    

                                }

                            }
                            else if (currentSelectedOrgan != null)
                            {
                                // not an anomaly and selected organ
                                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.HasSelected);
                                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.SelectedIncorrect);
                            }
                            else
                            {
                                //not an anomaly and selected nothing.
                                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.HasSelected);
                                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.SelectedIncorrect);
                            }
                            return;

                        }

                    }

                }

            }
        }
    }
    public void ScanSkeleton()
    {
        if(npcCreator.manOrWoman)
        {
            //man
            maleSkeleton.Visible = true;
            femaleSkeleton.Visible = false;
            maleSkeleton.Position = ogSkeletonPos;
            femaleSkeleton.Position = OUTOFSCREENPOS;
            femaleOrgans.Visible = false;
            femaleOrgans.Position = OUTOFSCREENPOS;
            maleOrgans.Visible = false;
            maleOrgans.Position = OUTOFSCREENPOS;

        }
        else
        {
            maleSkeleton.Visible = false;
            femaleSkeleton.Visible = true;
            maleSkeleton.Position = OUTOFSCREENPOS;
            femaleSkeleton.Position = ogSkeletonPos;
            femaleOrgans.Visible = false;
            femaleOrgans.Position = OUTOFSCREENPOS;
            maleOrgans.Visible = false;
            maleOrgans.Position = OUTOFSCREENPOS;
        }


        if (hasScannedSkeleton)
        {
            return;
        }

        int randomValue = random.RandiRange(0, 100);

        if(npcCreator.manOrWoman)
        {
            //it's a man
            GD.Print("man");
            foreach (var node in maleSkeleton.GetChildren())
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
        }
        else
        {
            GD.Print("woman");
            foreach (var node in femaleSkeleton.GetChildren())
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
        }



        skeletonArray = skeletonArrayPublic.Duplicate();
        skeletonArrayFull = skeletonArray.Duplicate();
        skeletonArray.Shuffle();

        if (skeletonOrOrgan && isDG && !physiqueAbberation)
        {
            int randomIndex = RandomSelectionSkeleton();
            Sprite2D current = Abberate(randomIndex, skeletonArrayPublic);

            currentSkeletonIndex = randomIndex;
            anomalyBone = current;
            GD.Print(current.Name);
            hasScannedSkeleton = true;
        }

    }
    public void ScanOrgans()
    {
        if(organsArrayPublic != null)
        {
            organsArrayPublic.Clear();

        }
        if(organsArray!= null)
        {
            organsArray.Clear();

        }

        if (npcCreator.manOrWoman)
        {
            //man
            maleSkeleton.Visible = false;
            femaleSkeleton.Visible = false;
            maleSkeleton.Position = OUTOFSCREENPOS;
            femaleSkeleton.Position = OUTOFSCREENPOS;
            femaleOrgans.Visible = false;
            femaleOrgans.Position = OUTOFSCREENPOS;
            maleOrgans.Visible = true;
            maleOrgans.Position = ogOrgansPos;

        }
        else
        {
            maleSkeleton.Visible = false;
            femaleSkeleton.Visible = false;
            maleSkeleton.Position = OUTOFSCREENPOS;
            femaleSkeleton.Position = OUTOFSCREENPOS;
            femaleOrgans.Visible = true;
            femaleOrgans.Position = ogOrgansPos;
            maleOrgans.Visible = false;
            maleOrgans.Position = OUTOFSCREENPOS;
        }


        if (hasScannedOrgans)
        {
            return;
        }

        int randomValue = random.RandiRange(0, 100);

        if(npcCreator.manOrWoman)
        {
            //man
            foreach (var node in maleOrgans.GetChildren())
            {
                if (true/*node.Name == boneGroups.ToString()*/)
                {
                    foreach (var bone in node.GetChildren())
                    {
                        if (bone is Sprite2D sprite)
                        {
                            sprite = (Sprite2D)bone;
                            organsArrayPublic.Add(sprite);
                            if (!nameAndOrganDictionary.ContainsKey(sprite.Name))
                                nameAndOrganDictionary.Add(sprite.Name, sprite.SelfModulate);
                        }
                        else
                        {
                            foreach (var secondaryBone in bone.GetChildren())
                            {
                                if (secondaryBone is Sprite2D spriteSecondary)
                                {
                                    spriteSecondary = (Sprite2D)secondaryBone;
                                    organsArrayPublic.Add(spriteSecondary);
                                    if (!nameAndOrganDictionary.ContainsKey(spriteSecondary.Name))
                                        nameAndOrganDictionary.Add(spriteSecondary.Name, spriteSecondary.SelfModulate);

                                }
                            }
                        }
                    }

                }
                if (node is Sprite2D spriteHigher)
                {
                    organsArrayPublic.Add(spriteHigher);
                    if (!nameAndOrganDictionary.ContainsKey(spriteHigher.Name))
                        nameAndOrganDictionary.Add(spriteHigher.Name, spriteHigher.SelfModulate);

                }
            }
        }
        else
        {
            //woman
            foreach (var node in femaleOrgans.GetChildren())
            {
                if (true/*node.Name == boneGroups.ToString()*/)
                {
                    foreach (var bone in node.GetChildren())
                    {
                        if (bone is Sprite2D sprite)
                        {
                            sprite = (Sprite2D)bone;
                            organsArrayPublic.Add(sprite);
                            if (!nameAndOrganDictionary.ContainsKey(sprite.Name))
                                nameAndOrganDictionary.Add(sprite.Name, sprite.SelfModulate);
                        }
                        else
                        {
                            foreach (var secondaryBone in bone.GetChildren())
                            {
                                if (secondaryBone is Sprite2D spriteSecondary)
                                {
                                    spriteSecondary = (Sprite2D)secondaryBone;
                                    organsArrayPublic.Add(spriteSecondary);
                                    if (!nameAndOrganDictionary.ContainsKey(spriteSecondary.Name))
                                        nameAndOrganDictionary.Add(spriteSecondary.Name, spriteSecondary.SelfModulate);

                                }
                            }
                        }
                    }

                }
                if (node is Sprite2D spriteHigher)
                {
                    organsArrayPublic.Add(spriteHigher);
                    if (!nameAndOrganDictionary.ContainsKey(spriteHigher.Name))
                        nameAndOrganDictionary.Add(spriteHigher.Name, spriteHigher.SelfModulate);

                }
            }
        }


        organsArray = organsArrayPublic.Duplicate();
        organsArrayFull = organsArray.Duplicate();
        organsArray.Shuffle();

        if (!skeletonOrOrgan && isDG && !physiqueAbberation)
        {
            int randomIndex = RandomSelectionOrgans();
            Sprite2D current = Abberate(randomIndex, organsArrayPublic);

            currentOrgansIndex = randomIndex;
            anomalyOrgan = current;
            GD.Print(current.Name);
            hasScannedOrgans = true;
        }

    }
    Sprite2D Abberate(int randomIndex, Godot.Collections.Array<Sprite2D> array)
    {

        float randomValue = random.RandiRange(0, 100);
        if (randomValue <= 25f)
        {
            //Make missing
            while (array[randomIndex].IsInGroup("Avoid"))
            {
                randomIndex = RandomSelectionOrgans();
            }

            Sprite2D current = array[randomIndex];
            SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.MakeMissing);
            MakeMissing(current);
            return current;

        }
        else if (randomValue < 50f)
        {
            //Make colourable
            while (array[randomIndex].IsInGroup("Avoid"))
            {
                randomIndex = RandomSelectionOrgans();
            }

            Sprite2D current = array[randomIndex];
            SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.MakeDiscoloured);
            ReColour(current);
            return current;
        }
        else if (randomValue < 75f)
        {
            //transform thing
            while (array[randomIndex].IsInGroup("Avoid") || array[randomIndex].IsInGroup("NoRotate"))
            {
                randomIndex = RandomSelectionOrgans();
            }
            Sprite2D current = array[randomIndex];
            SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.MakeRotated);
            AlterTransformOfObject(current, 180f);
            return current;
        }
        else if (randomValue <= 100f)
        {
            //Duplicate
            while (array[randomIndex].IsInGroup("Avoid") || array[randomIndex].IsInGroup("Ribs"))
            {
                randomIndex = RandomSelectionOrgans();
            }

            Sprite2D current = array[randomIndex];
            SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.MakeDuplicated);
            AddObject(current);
            return current;

        }
        else
        {
            Sprite2D current = array[randomIndex];
            GD.Print("something went seriously wrong");
            return current;
        }



    }
    void MakeMissing(Sprite2D current)
    {
        current.SelfModulate = veryTranslucentColour;
    }
    void AlterTransformOfObject(Sprite2D current, float angle)
    {
        current.RotationDegrees = angle;
    }
    void AddObject(Sprite2D current)
    {
        Sprite2D duplicatedObject = (Sprite2D)current.Duplicate();
        duplicateObjects.Add(duplicatedObject);
        current.GetParent().AddChild(duplicatedObject);
        AlterTransformOfObject(duplicatedObject, random.RandiRange(30, 120));
    }
    void ReColour(Sprite2D current)
    {
        int randomIndex = random.RandiRange(0, weirdColours.Length - 1);
        current.SelfModulate = weirdColours[randomIndex].colour;
        SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.SendColour, weirdColours[randomIndex].colourName);
        offColourOrganName = current.Name;
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
    bool visible = true;
    void ClearObjects()
    {
        visible = !visible;

        Color transparent = new Color(0f, 0f, 0f, 0f);

        foreach (Sprite2D sprite in hideables)
        {
            //Populates the dictionary
            if (!originalColours.ContainsKey(sprite.Name))
            {
                originalColours.Add(sprite.Name, sprite.SelfModulate);
            }
        }

        if(visible)
        {
            foreach(Sprite2D sprite in hideables)
            {
                sprite.Visible = true;
                sprite.SelfModulate = originalColours[sprite.Name];
            }
        }
        else
        {
            foreach(Sprite2D sprite in hideables)
            {
                sprite.Visible = false;
                sprite.SelfModulate = transparent;
                
            }
        }

    }
    void EnableObjects()
    {
        foreach (Sprite2D sprite in hideables)
        {
            sprite.Visible = true;
            if(originalColours.ContainsKey(sprite.Name))
                sprite.SelfModulate = originalColours[sprite.Name];
        }

    }

}
